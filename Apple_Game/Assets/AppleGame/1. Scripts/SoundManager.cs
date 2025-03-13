using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("--------------[ Audio Sources ]")]
    private AudioSource bgmSource;          // BGM ����� AudioSource
    private AudioSource sfxSource;          // ȿ���� ����� AudioSource

    [Header("--------------[ Audio Clips ]")]
    public AudioClip bgmClip;               // �������
    public AudioClip sfxClip;               // ȿ����

    [Header("--------------[ Sound Settings ]")]
    private float bgmVolume = 0.3f;         // BGM ����
    private float sfxVolume = 0.5f;         // ȿ���� ����

    private ButtonManager buttonManager;    // ButtonManager ����

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

    // ����� �ҽ� �ʱ�ȭ
    private void InitializeAudio()
    {
        // BGM ����
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        // SFX ����
        sfxSource = gameObject.AddComponent<AudioSource>();
        //sfxSource.clip = sfxClip;
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    // BGM ���
    public void PlayBGM()
    {
        if (bgmSource.clip == null) return;

        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
        UpdateBGMState();
    }

    // ȿ���� ���
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

    // BGM ���� ������Ʈ
    public void UpdateBGMState()
    {
        if (buttonManager != null)
        {
            bgmSource.mute = !buttonManager.IsBGMOn();
        }
    }

    // ȿ���� ���� ������Ʈ
    public void UpdateSFXState()
    {
        if (buttonManager != null)
        {
            sfxSource.mute = !buttonManager.IsSFXOn();
        }
    }
}
