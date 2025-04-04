using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SoundManager : MonoBehaviour
{


    #region Variables

    public static SoundManager Instance { get; private set; }

    [Header("--------------[ Audio Sources ]")]
    private AudioSource bgmSource;                      // BGM 재생용 AudioSource
    private AudioSource sfxSource;                      // SFX 재생용 AudioSource

    [Header("--------------[ Audio Clips ]")]
    [SerializeField] private AudioClip bgmClip;         // BGM 클립
    [SerializeField] private AudioClip sfxClip;         // SFX 클립

    [Header("--------------[ Sound Settings ]")]
    private float bgmVolume = 0.3f;                     // BGM 볼륨 크기
    private float sfxVolume = 0.5f;                     // SFX 볼륨 크기
    private bool isBgmOn = true;                        // BGM 상태
    private bool isSfxOn = true;                        // SFX 상태
    private readonly Color onColor = new Color(0f, 200f / 255f, 0f);    // On 상태 색상
    private readonly Color offColor = Color.red;                        // Off 상태 색상

    [Header("--------------[ ETC ]")]
    private OptionManager optionManager;                // OptionManager 참조

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSoundManager();
            LoadSoundSettings();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion


    #region Scene Management

    // 씬 로드 완료 시 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindButtonManager();
        UpdateAudioStates();
    }

    // ButtonManager를 찾는 함수
    private void FindButtonManager()
    {
        optionManager = FindObjectOfType<OptionManager>();
        if (optionManager == null)
        {
            return;
        }

        UpdateAudioStates();
    }

    // 오디오 상태 업데이트 함수
    private void UpdateAudioStates()
    {
        UpdateBgmState();
        UpdateSfxState();
    }

    #endregion


    #region Sound Management

    // 오디오 소스 초기화 함수
    private void InitializeSoundManager()
    {
        InitializeBGM();
        InitializeSFX();
    }

    // BGM 초기화 함수
    private void InitializeBGM()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;
    }

    // SFX 초기화 함수
    private void InitializeSFX()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = sfxClip;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    // BGM 재생 함수
    public void PlayBGM()
    {
        if (bgmSource?.clip == null)
        {
            return;
        }

        if (bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.Play();
        UpdateBgmState();
    }

    // SFX 재생 함수
    public void PlaySfx()
    {
        if (sfxClip == null)
        {
            return;
        }

        if (optionManager == null)
        {
            FindButtonManager();
        }

        if (IsSfxOn())
        {
            sfxSource.PlayOneShot(sfxClip, sfxVolume);
        }
    }

    // BGM 상태 업데이트 함수
    public void UpdateBgmState()
    {
        if (optionManager == null || bgmSource == null)
        {
            return;
        }

        bgmSource.mute = !IsBgmOn();
    }

    // SFX 상태 업데이트 함수
    public void UpdateSfxState()
    {
        if (optionManager == null || sfxSource == null)
        {
            return;
        }

        sfxSource.mute = !IsSfxOn();
    }

    #endregion


    #region Sound Settings

    // Bgm 토글 함수
    public void ToggleBgm()
    {
        isBgmOn = !isBgmOn;
        SaveSoundSettings();
        UpdateBgmState();
    }

    // Sfx 토글 함수
    public void ToggleSfx()
    {
        isSfxOn = !isSfxOn;
        SaveSoundSettings();
        UpdateSfxState();
    }

    // Bgm On/Off 확인 함수
    public bool IsBgmOn()
    {
        return isBgmOn;
    }

    // Sfx On/Off 확인 함수
    public bool IsSfxOn()
    {
        return isSfxOn;
    }

    // 사운드 저장 함수
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 사운드 로드 함수
    private void LoadSoundSettings()
    {
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }

    // 사운드 버튼 색 변경 함수
    public void UpdateButtonColor(Image buttonImage, bool isOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isOn ? onColor : offColor;
        }
    }

    // 사운드 버튼 텍스트 변경 함수
    public void UpdateButtonText(TextMeshProUGUI buttonText, bool isOn)
    {
        if (buttonText != null)
        {
            buttonText.text = isOn ? "On" : "Off";
        }
    }

    #endregion


}
