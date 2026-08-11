using System;
using UnityEngine;
using GoogleMobileAds.Api;

// AdMob 전면광고 매니저. "3판마다 1회" 빈도로만 노출한다 - 결과 화면에서
// 재시작/타이틀로 나가는, 유저가 이미 "한 판 끝났다"고 인지한 자연스러운
// 전환점에서만 부른다(GameManager는 판이 끝났다는 사실만 알리고, 실제로
// 지금 보여줄지는 전부 이 클래스가 판단한다).
//
// 원칙: 광고가 없거나 로드 실패해도 게임 진행(씬 전환)은 절대 막히지 않는다.
// GoogleMobileAds 타입은 이 클래스 밖으로 새어나가지 않는다 - 나중에 SDK를
// 교체하더라도 이 파일만 고치면 된다.
public class AdManager : MonoBehaviour
{

    public static AdManager Instance { get; private set; }

    // TODO: 실제 발급받은 프로덕션 ID로 이미 채워져 있음. AdMob 콘솔에서 ID를 다시
    // 발급받는 경우에만 아래 PROD 값을 바꾸면 된다. App ID(ca-app-pub-...~...)는 코드가
    // 아니라 Assets > Google Mobile Ads > Settings 에디터 창에 입력해야 빌드에 반영된다.
    //
    // 에디터뿐 아니라 Development Build(기기 테스트 빌드)에서도 테스트 ID를 쓴다 -
    // 그렇지 않으면 기기로 테스트할 때마다 실제 광고 인벤토리를 소모/클릭하게 되어
    // AdMob 정책상 무효 트래픽 위험이 있다. 스토어에 올라가는 릴리스 빌드에서만
    // 실제 ID가 쓰인다.
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712"; // 구글 공식 테스트 전면광고 ID
#else
    private const string AD_UNIT_ID = "ca-app-pub-6387288948977074/9398020001"; // 실제 전면광고 단위 ID
#endif

    private const int GAMES_PER_AD = 3;                  // 몇 판마다 광고 1회
    private const long MIN_INTERVAL_SECONDS = 60;         // 연속 노출 방지용 최소 간격

    private const string KEY_GAMES_SINCE_LAST_AD = "AdManager_GamesSinceLastAd";
    private const string KEY_PENDING_AD = "AdManager_PendingAd";
    private const string KEY_LAST_SHOWN_UNIX_TIME = "AdManager_LastShownUnixTime";

    private InterstitialAd interstitialAd;
    private Action pendingOnComplete;

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion


    #region Initialize

    public void Initialize()
    {
        // 에디터에서도 그대로 초기화/로드를 시도한다 - 실제로 뜨는지 콘솔 로그로 눈으로 확인하기 위함.
        // (에디터는 네이티브 광고 SDK가 없어 실제 창은 안 뜰 수 있지만, 최소한 로드 성공/실패 로그는 찍힌다)
        Debug.Log("[AdManager] Initialize - MobileAds.Initialize() 호출, AD_UNIT_ID=" + AD_UNIT_ID);
        MobileAds.Initialize(status => Debug.Log("[AdManager] MobileAds.Initialize 완료"));
        LoadInterstitial();
    }

    #endregion


    #region Game Progress

    // GameManager.GameOver()에서 한 판이 끝날 때마다 호출된다.
    public void NotifyGameCompleted()
    {
        int countBeforeReset = PlayerPrefs.GetInt(KEY_GAMES_SINCE_LAST_AD, 0) + 1;
        bool reachedThreshold = countBeforeReset >= GAMES_PER_AD;
        int countToSave = reachedThreshold ? 0 : countBeforeReset;

        if (reachedThreshold)
        {
            PlayerPrefs.SetInt(KEY_PENDING_AD, 1);
        }

        PlayerPrefs.SetInt(KEY_GAMES_SINCE_LAST_AD, countToSave);
        PlayerPrefs.Save();

        Debug.Log($"[AdManager] NotifyGameCompleted - 이번 판까지 누적 {countBeforeReset}/{GAMES_PER_AD}판" +
            (reachedThreshold ? " -> 기준 도달! 대기 플래그 ON (다음 전환 때 노출 시도)" : " (아직 광고 없음)"));
    }

    #endregion


    #region Interstitial

    // OptionManager의 씬 전환(재시작/타이틀로) 직전에 호출된다.
    // 조건이 안 맞거나 광고가 준비 안 됐으면 onComplete를 즉시 호출해서 씬 전환이 막히지 않게 한다.
    // 에디터에서도 그대로 실행한다 - 네이티브 창은 안 뜰 수 있어도 판정 로그는 남아야
    // 실제로 조건이 맞았는지 콘솔에서 확인할 수 있다.
    public void MaybeShowInterstitial(Action onComplete)
    {
        bool isReady = interstitialAd != null && interstitialAd.CanShowAd();
        bool isEligible = IsEligibleToShow();

        if (!isEligible || !isReady)
        {
            Debug.Log($"[AdManager] MaybeShowInterstitial - 노출 안 함 (eligible={isEligible}, ready={isReady})");

            if (!isReady)
            {
                LoadInterstitial(); // 다음 기회를 위해 다시 시도
            }

            onComplete?.Invoke();
            return;
        }

        Debug.Log("[AdManager] MaybeShowInterstitial - 조건 충족, 전면광고 Show() 호출");

        pendingOnComplete = onComplete;
        RegisterShowCallbacks(interstitialAd);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PauseBGM();
        }

        interstitialAd.Show();
    }

    private bool IsEligibleToShow()
    {
        bool isPending = PlayerPrefs.GetInt(KEY_PENDING_AD, 0) == 1;
        if (!isPending)
        {
            return false;
        }

        long lastShown = GetLastShownUnixTime();
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return (now - lastShown) >= MIN_INTERVAL_SECONDS;
    }

    private void LoadInterstitial()
    {
        AdRequest adRequest = new AdRequest();

        InterstitialAd.Load(AD_UNIT_ID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                // 로드 실패해도 게임엔 영향 없음 - 다음 시도 때 다시 로드한다.
                Debug.Log("[AdManager] LoadInterstitial 실패: " + (error != null ? error.GetMessage() : "unknown"));
                return;
            }

            Debug.Log("[AdManager] LoadInterstitial 성공 - 다음 전환 때 노출 준비 완료");
            interstitialAd = ad;
        });
    }

    private void RegisterShowCallbacks(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += OnInterstitialClosed;
        ad.OnAdFullScreenContentFailed += OnInterstitialFailed;
    }

    private void ClearShowCallbacks(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed -= OnInterstitialClosed;
        ad.OnAdFullScreenContentFailed -= OnInterstitialFailed;
    }

    // 끝까지 봤을 때만 "노출 완료"로 취급한다 - 쿨다운/대기 플래그를 여기서 정리한다.
    private void OnInterstitialClosed()
    {
        Debug.Log("[AdManager] 전면광고 닫힘 - 대기 플래그 OFF, 쿨다운 갱신");

        ClearShowCallbacks(interstitialAd);

        PlayerPrefs.SetInt(KEY_PENDING_AD, 0);
        SetLastShownUnixTime(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        PlayerPrefs.Save();

        FinishShowing();
    }

    // 노출 자체가 실패한 경우 - 실제로 본 게 아니므로 대기 플래그/쿨다운은 그대로 두고
    // 다음 기회에 다시 시도한다.
    private void OnInterstitialFailed(AdError error)
    {
        Debug.Log("[AdManager] 전면광고 노출 실패: " + error.GetMessage());
        ClearShowCallbacks(interstitialAd);
        FinishShowing();
    }

    private void FinishShowing()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ResumeBGM();
        }

        interstitialAd.Destroy();
        interstitialAd = null;
        LoadInterstitial(); // 다음 노출을 위해 미리 로드

        Action callback = pendingOnComplete;
        pendingOnComplete = null;
        callback?.Invoke();
    }

    private static long GetLastShownUnixTime()
    {
        string saved = PlayerPrefs.GetString(KEY_LAST_SHOWN_UNIX_TIME, "0");
        return long.TryParse(saved, out long result) ? result : 0;
    }

    private static void SetLastShownUnixTime(long unixTime)
    {
        PlayerPrefs.SetString(KEY_LAST_SHOWN_UNIX_TIME, unixTime.ToString());
    }

    #endregion

}
