using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
    private float bgmVolume = 0.3f;                     // BGM ���� ũ��
    private float sfxVolume = 0.5f;                     // SFX ���� ũ��
    private bool isBgmOn = true;                        // BGM ����
    private bool isSfxOn = true;                        // SFX ����
    private readonly Color onColor = new Color(0f, 200f / 255f, 0f);    // On ���� ����
    private readonly Color offColor = Color.red;                        // Off ���� ����

    [Header("--------------[ ETC ]")]
    private OptionManager optionManager;                // OptionManager ����

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

    // �� �ε� �Ϸ� �� ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindButtonManager();
        UpdateAudioStates();
    }

    // ButtonManager�� ã�� �Լ�
    private void FindButtonManager()
    {
        optionManager = FindObjectOfType<OptionManager>();
        if (optionManager == null)
        {
            return;
        }

        UpdateAudioStates();
    }

    // ����� ���� ������Ʈ �Լ�
    private void UpdateAudioStates()
    {
        UpdateBgmState();
        UpdateSfxState();
    }

    #endregion


    #region Sound Management

    // ����� �ҽ� �ʱ�ȭ �Լ�
    private void InitializeSoundManager()
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
        sfxSource.clip = sfxClip;
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
        UpdateBgmState();
    }

    // SFX ��� �Լ�
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

    // BGM ���� ������Ʈ �Լ�
    public void UpdateBgmState()
    {
        if (optionManager == null || bgmSource == null)
        {
            return;
        }

        bgmSource.mute = !IsBgmOn();
    }

    // SFX ���� ������Ʈ �Լ�
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

    // Bgm ��� �Լ�
    public void ToggleBgm()
    {
        isBgmOn = !isBgmOn;
        SaveSoundSettings();
        UpdateBgmState();
    }

    // Sfx ��� �Լ�
    public void ToggleSfx()
    {
        isSfxOn = !isSfxOn;
        SaveSoundSettings();
        UpdateSfxState();
    }

    // Bgm On/Off Ȯ�� �Լ�
    public bool IsBgmOn()
    {
        return isBgmOn;
    }

    // Sfx On/Off Ȯ�� �Լ�
    public bool IsSfxOn()
    {
        return isSfxOn;
    }

    // ���� ���� �Լ�
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ���� �ε� �Լ�
    private void LoadSoundSettings()
    {
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }

    // ���� ��ư �� ���� �Լ�
    public void UpdateButtonColor(Image buttonImage, bool isOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isOn ? onColor : offColor;
        }
    }

    // ���� ��ư �ؽ�Ʈ ���� �Լ�
    public void UpdateButtonText(TextMeshProUGUI buttonText, bool isOn)
    {
        if (buttonText != null)
        {
            buttonText.text = isOn ? "On" : "Off";
        }
    }

    #endregion


}
