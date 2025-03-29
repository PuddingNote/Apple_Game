using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float bgmVolume = 0.2f;                     // BGM 볼륨 크기
    private float sfxVolume = 0.3f;                     // SFX 볼륨 크기

    [Header("--------------[ ETC ]")]
    private ButtonManager buttonManager;                // buttonManager 참조

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudio();
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
    }

    // ButtonManager를 찾는 함수
    private void FindButtonManager()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager == null)
        {
            return;
        }

        UpdateAudioStates();
    }

    // 오디오 상태 업데이트 함수
    private void UpdateAudioStates()
    {
        UpdateBGMState();
        UpdateSFXState();
    }

    #endregion


    #region Audio Management

    // 오디오 소스 초기화 함수
    private void InitializeAudio()
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
        UpdateBGMState();
    }

    // SFX 재생 함수
    public void PlaySFX()
    {
        if (sfxClip == null)
        {
            return;
        }

        if (buttonManager == null)
        {
            FindButtonManager();
        }

        if (buttonManager.IsSFXOn())
        {
            sfxSource.PlayOneShot(sfxClip, sfxVolume);
        }
    }

    // BGM 상태 업데이트 함수
    public void UpdateBGMState()
    {
        if (buttonManager == null || bgmSource == null)
        {
            return;
        }

        bgmSource.mute = !buttonManager.IsBGMOn();
    }

    // SFX 상태 업데이트 함수
    public void UpdateSFXState()
    {
        if (buttonManager == null || sfxSource == null)
        {
            return;
        }

        sfxSource.mute = !buttonManager.IsSFXOn();
    }

    #endregion


}
