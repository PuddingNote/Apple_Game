using UnityEngine;

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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        PlayBGM();
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
        //sfxSource.clip = sfxClip;
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
            buttonManager = FindObjectOfType<ButtonManager>();
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
