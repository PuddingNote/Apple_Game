using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("--------------[ Audio Sources ]")]
    private AudioSource bgmSource;          // BGM 재생용 AudioSource
    private AudioSource sfxSource;          // 효과음 재생용 AudioSource

    [Header("--------------[ Audio Clips ]")]
    public AudioClip bgmClip;               // 배경음악
    public AudioClip sfxClip;               // 효과음

    [Header("--------------[ Sound Settings ]")]
    private float bgmVolume = 0.3f;         // BGM 볼륨
    private float sfxVolume = 0.5f;         // 효과음 볼륨

    private ButtonManager buttonManager;    // ButtonManager 참조

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();

            // 씬 변경 이벤트 리스너 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindButtonManager();
        PlayBGM();
    }

    // 씬 로드 완료 시 호출되는 이벤트 핸들러
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 변경 시 ButtonManager 참조 갱신
        FindButtonManager();
    }

    // ButtonManager 찾기
    private void FindButtonManager()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager != null)
        {
            UpdateBGMState();
            UpdateSFXState();
        }
        else
        {
            // 한 프레임 후에 다시 시도 (씬 로딩 타이밍 문제 해결)
            Invoke("RetryFindButtonManager", 0.1f);
        }
    }

    // ButtonManager 찾기 재시도
    private void RetryFindButtonManager()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager != null)
        {
            UpdateBGMState();
            UpdateSFXState();
        }
    }

    // 오디오 소스 초기화
    private void InitializeAudio()
    {
        // BGM 설정
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        // SFX 설정
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    // BGM 재생
    public void PlayBGM()
    {
        if (bgmSource.clip == null) return;

        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
        UpdateBGMState();
    }

    // 효과음 재생
    public void PlaySFX()
    {
        if (sfxClip == null) return;

        if (buttonManager == null)
        {
            FindButtonManager();
            if (buttonManager == null) return;
        }

        if (buttonManager.IsSFXOn())
        {
            sfxSource.PlayOneShot(sfxClip, sfxVolume);
        }
    }

    // BGM 상태 업데이트
    public void UpdateBGMState()
    {
        if (buttonManager != null)
        {
            bgmSource.mute = !buttonManager.IsBGMOn();
        }
    }

    // 효과음 상태 업데이트
    public void UpdateSFXState()
    {
        if (buttonManager != null)
        {
            sfxSource.mute = !buttonManager.IsSFXOn();
        }
    }
}
