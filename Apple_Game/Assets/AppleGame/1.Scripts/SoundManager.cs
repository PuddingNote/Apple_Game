using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{


    #region Variables

    public static SoundManager Instance { get; private set; }

    [Header("--------------[ Audio Sources ]")]
    private AudioSource bgmSource;                      // BGM ����� AudioSource
    private AudioSource sfxSource;                      // SFX ����� AudioSource

    [Header("--------------[ Audio Clips ]")]
    [SerializeField] private AudioClip bgmClip;         // BGM Ŭ��
    [SerializeField] private AudioClip sfxClip;         // SFX Ŭ��

    [Header("--------------[ Sound Settings ]")]
    private float bgmVolume = 0.2f;                     // BGM ���� ũ��
    private float sfxVolume = 0.3f;                     // SFX ���� ũ��

    [Header("--------------[ ETC ]")]
    private ButtonManager buttonManager;                // buttonManager ����

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

    // �� �ε� �Ϸ� �� ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindButtonManager();
    }

    // ButtonManager�� ã�� �Լ�
    private void FindButtonManager()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager == null)
        {
            return;
        }

        UpdateAudioStates();
    }

    // ����� ���� ������Ʈ �Լ�
    private void UpdateAudioStates()
    {
        UpdateBGMState();
        UpdateSFXState();
    }

    #endregion


    #region Audio Management

    // ����� �ҽ� �ʱ�ȭ �Լ�
    private void InitializeAudio()
    {
        InitializeBGM();
        InitializeSFX();
    }

    // BGM �ʱ�ȭ �Լ�
    private void InitializeBGM()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;
    }

    // SFX �ʱ�ȭ �Լ�
    private void InitializeSFX()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    // BGM ��� �Լ�
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

    // SFX ��� �Լ�
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

    // BGM ���� ������Ʈ �Լ�
    public void UpdateBGMState()
    {
        if (buttonManager == null || bgmSource == null)
        {
            return;
        }

        bgmSource.mute = !buttonManager.IsBGMOn();
    }

    // SFX ���� ������Ʈ �Լ�
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
