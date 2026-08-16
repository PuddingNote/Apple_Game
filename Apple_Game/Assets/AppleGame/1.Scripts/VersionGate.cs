using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// 강제 업데이트 게이트 - 원격 version.json을 조회해서 현재 앱 버전이 minVersion보다 낮으면
// UpdateRequiredView로 화면을 막는다.
//
// 핵심 원칙(fail-open): 오프라인/타임아웃/호스팅 장애/JSON 오류/버전 파싱 실패 등
// "확인에 실패한 경우"는 전부 그냥 통과시킨다. 오타 하나로 전체 사용자가 게임을
// 못 켜는 사고가, 일부가 구버전을 쓰는 것보다 훨씬 나쁘다.
public class VersionGate : MonoBehaviour
{

    // 배포 후 바꿀 일이 있다면 이 URL만 수정하면 된다.
    // 여러 게임이 공유하는 PuddingNote.github.io 저장소(applegame 폴더)로 옮겨왔다.
    // 이 저장소가 public이어야 raw.githubusercontent.com으로 바로 접근 가능하다.
    private const string VERSION_URL = "https://raw.githubusercontent.com/PuddingNote/PuddingNote.github.io/main/applegame/version.json";
    private const int TIMEOUT_SECONDS = 5;

    public static bool IsBlocking { get; private set; }

    [Serializable]
    private class VersionInfo
    {
        public string minVersion;
        public string storeUrl;
        public string message;
    }

    public void Initialize()
    {
        StartCoroutine(CheckVersion());
    }

    private IEnumerator CheckVersion()
    {
        // 캐시 무효화용 쿼리 파라미터 - raw.githubusercontent.com은 CDN 캐시가 걸려 있어서,
        // 없으면 방금 push한 최신 내용이 몇 분간 안 보일 수 있다.
        string requestUrl = VERSION_URL + "?t=" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        using (UnityWebRequest request = UnityWebRequest.Get(requestUrl))
        {
            request.timeout = TIMEOUT_SECONDS;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("[VersionGate] 조회 실패(오프라인/타임아웃/404 등) -> 통과: " + request.error);
                yield break;
            }

            VersionInfo info;
            try
            {
                info = JsonUtility.FromJson<VersionInfo>(request.downloadHandler.text);
            }
            catch
            {
                Debug.Log("[VersionGate] JSON 파싱 오류 -> 통과: " + request.downloadHandler.text);
                yield break;
            }

            if (info == null || string.IsNullOrWhiteSpace(info.minVersion))
            {
                Debug.Log("[VersionGate] minVersion 비어있음/파싱 실패 -> 통과");
                yield break;
            }

            Debug.Log($"[VersionGate] 현재 버전={Application.version}, minVersion={info.minVersion}");

            if (!AppVersion.IsLowerThan(Application.version, info.minVersion))
            {
                Debug.Log("[VersionGate] 최신 버전이거나 비교 불가 -> 통과");
                yield break;
            }

            Debug.Log("[VersionGate] 구버전 확인 -> 차단 화면 표시");

            string message = string.IsNullOrWhiteSpace(info.message)
                ? "새로운 버전이 나왔습니다.\n업데이트 후 이용해 주세요."
                : info.message;

            ShowBlockingView(message, info.storeUrl);
        }
    }

    private void ShowBlockingView(string message, string storeUrl)
    {
        IsBlocking = true;
        UpdateRequiredView.Show(transform, message, storeUrl);
    }

}
