using UnityEngine;
using UnityEngine.SceneManagement;

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
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();

            // �� ���� �̺�Ʈ ������ ���
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ������ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        FindButtonManager();
        PlayBGM();
    }

    // �� �ε� �Ϸ� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� ���� �� ButtonManager ���� ����
        FindButtonManager();
    }

    // ButtonManager ã��
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
            // �� ������ �Ŀ� �ٽ� �õ� (�� �ε� Ÿ�̹� ���� �ذ�)
            Invoke("RetryFindButtonManager", 0.1f);
        }
    }

    // ButtonManager ã�� ��õ�
    private void RetryFindButtonManager()
    {
        buttonManager = FindObjectOfType<ButtonManager>();
        if (buttonManager != null)
        {
            UpdateBGMState();
            UpdateSFXState();
        }
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
            FindButtonManager();
            if (buttonManager == null) return;
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
