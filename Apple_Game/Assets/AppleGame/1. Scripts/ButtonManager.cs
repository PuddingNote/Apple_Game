using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    [Header("--------------[ ETC ]")]
    private Canvas mainCanvas;              // Canvas ����

    private Button optionButton;            // �ɼ� ��ư

    private GameObject escPanel;            // �Ͻ����� �г�
    private GameObject optionPanel;         // �ɼ� �г�
    private GameObject helpPanel;           // ���� �г�
    private GameObject creditPanel;         // ũ���� �г�

    // ����
    private Button bgmButton;               // BGM ��ư
    private Button sfxButton;               // SFX ��ư
    private Image bgmButtonImage;           // BGM ��ư �̹���
    private Image sfxButtonImage;           // SFX ��ư �̹���
    private TextMeshProUGUI bgmButtonText;  // BGM ��ư �ؽ�Ʈ
    private TextMeshProUGUI sfxButtonText;  // SFX ��ư �ؽ�Ʈ

    private bool isBgmOn = true;            // BGM ����
    private bool isSfxOn = true;            // SFX ����

    private Color onColor = new Color(0f, 200f / 255f, 0f);     // On ���� ����
    private Color offColor = new Color(255f / 255f, 0f, 0f);    // Off ���� ����

    private void Awake()
    {
        mainCanvas = FindObjectOfType<Canvas>();

        LoadSoundSettings();
    }

    private void Start()
    {
        InitializePausePanel();

        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            InitializeOptionPanel();
            InitializeOptionButton();
            InitializeHelpPanel();
            InitializeCreditPanel();
        }
    }

    private void Update()
    {
        HandleEscInput();
    }

    // �Ͻ����� �г� ���� �ʱ�ȭ
    private void InitializePausePanel()
    {
        escPanel = mainCanvas.transform.Find("Pause Panel").gameObject;

        Transform backButtonTransform = escPanel.transform.Find("BackGround/Back Button");
        if (backButtonTransform != null)
        {
            Button backButton = backButtonTransform.GetComponent<Button>();
            backButton.onClick.AddListener(() => escPanel.SetActive(false));
        }

        Transform titleButtonTransform = escPanel.transform.Find("BackGround/Title Button");
        if (titleButtonTransform != null)
        {
            Button titleButton = titleButtonTransform.GetComponent<Button>();
            titleButton.onClick.AddListener(() => GoTitle());
        }

        Transform quitButtonTransform = escPanel.transform.Find("BackGround/Quit Button");
        if (quitButtonTransform != null)
        {
            Button quitButton = quitButtonTransform.GetComponent<Button>();
            quitButton.onClick.AddListener(() => QuitGame());
        }

        escPanel.SetActive(false);
    }

    // �ɼ� �г� ���� �ʱ�ȭ
    private void InitializeOptionPanel()
    {
        optionPanel = mainCanvas.transform.Find("Option Panel").gameObject;

        // BGM On/Off ��ư
        Transform bgmButtonTransform = optionPanel.transform.Find("BackGround/Sound Objects/����� Button");
        if (bgmButtonTransform != null)
        {
            bgmButton = bgmButtonTransform.GetComponent<Button>();
            bgmButtonImage = bgmButtonTransform.GetComponent<Image>();
            bgmButtonText = bgmButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
            bgmButton.onClick.AddListener(ToggleBGM);
            UpdateButtonColor(bgmButtonImage, isBgmOn);
            UpdateButtonText(bgmButtonText, isBgmOn);
        }

        // SFX On/Off ��ư
        Transform sfxButtonTransform = optionPanel.transform.Find("BackGround/Sound Objects/ȿ���� Button");
        if (sfxButtonTransform != null)
        {
            sfxButton = sfxButtonTransform.GetComponent<Button>();
            sfxButtonImage = sfxButtonTransform.GetComponent<Image>();
            sfxButtonText = sfxButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
            sfxButton.onClick.AddListener(ToggleSFX);
            UpdateButtonColor(sfxButtonImage, isSfxOn);
            UpdateButtonText(sfxButtonText, isSfxOn);
        }

        // ����
        Transform helpButtonTransform = optionPanel.transform.Find("BackGround/���� Button");
        if (helpButtonTransform != null)
        {
            Button helpButton = helpButtonTransform.GetComponent<Button>();
            helpButton.onClick.AddListener(() => helpPanel.SetActive(true));
        }

        // ũ����
        Transform creditButtonTransform = optionPanel.transform.Find("BackGround/ũ���� Button");
        if (creditButtonTransform != null)
        {
            Button creditButton = creditButtonTransform.GetComponent<Button>();
            creditButton.onClick.AddListener(() => creditPanel.SetActive(true));
        }

        Transform backButtonTransform = optionPanel.transform.Find("BackGround/Back Button");
        if (backButtonTransform != null)
        {
            Button backButton = backButtonTransform.GetComponent<Button>();
            backButton.onClick.AddListener(() => optionPanel.SetActive(false));
        }

        optionPanel.SetActive(false);
    }

    // �ɼ� ��ư �ʱ�ȭ
    private void InitializeOptionButton()
    {
        optionButton = mainCanvas.transform.Find("Start Group/Option Button").GetComponent<Button>();

        if (optionButton != null)
        {
            optionButton.onClick.AddListener(() => optionPanel.SetActive(true));
        }
    }

    // ���� �г� ���� �ʱ�ȭ
    private void InitializeHelpPanel()
    {
        helpPanel = mainCanvas.transform.Find("Help Panel").gameObject;
        helpPanel.SetActive(false);

        Transform backButtonTransform = helpPanel.transform.Find("BackGround/Back Button");
        if (backButtonTransform != null)
        {
            Button backButton = backButtonTransform.GetComponent<Button>();
            backButton.onClick.AddListener(() => helpPanel.SetActive(false));
        }
    }

    // ũ���� �г� ���� �ʱ�ȭ
    private void InitializeCreditPanel()
    {
        creditPanel = mainCanvas.transform.Find("Credit Panel").gameObject;
        creditPanel.SetActive(false);

        Transform backButtonTransform = creditPanel.transform.Find("BackGround/Back Button");
        if (backButtonTransform != null)
        {
            Button backButton = backButtonTransform.GetComponent<Button>();
            backButton.onClick.AddListener(() => creditPanel.SetActive(false));
        }
    }

    // ESC(�ڷΰ���) �Է� ó��
    private void HandleEscInput()
    {
        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
        bool androidBackPressed = Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape);

        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            if ((escapePressed || androidBackPressed) && !optionPanel.activeSelf)
            {
                if (!escPanel.activeSelf)
                {
                    escPanel.SetActive(true);
                }
            }
        }
        else if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if ((escapePressed || androidBackPressed))
            {
                if (!escPanel.activeSelf)
                {
                    escPanel.SetActive(true);
                }
            }
        }
    }

    // ESC(�ڷΰ���) �г� Ȱ��ȭ ���� Ȯ��
    public bool IsActiveEscPanel()
    {
        if (escPanel.activeSelf)
        {
            return true;
        }
        return false;
    }

    #region ����
    // BGM ���
    private void ToggleBGM()
    {
        isBgmOn = !isBgmOn;
        UpdateButtonColor(bgmButtonImage, isBgmOn);
        UpdateButtonText(bgmButtonText, isBgmOn);
        SaveSoundSettings();

        // SoundManager ������Ʈ
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateBGMState();
        }
    }

    // SFX ���
    private void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        UpdateButtonColor(sfxButtonImage, isSfxOn);
        UpdateButtonText(sfxButtonText, isSfxOn);
        SaveSoundSettings();

        // SoundManager ������Ʈ
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateSFXState();
        }
    }

    // ��ư ���� ������Ʈ
    private void UpdateButtonColor(Image buttonImage, bool isOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isOn ? onColor : offColor;
        }
    }

    // ��ư �ؽ�Ʈ ������Ʈ
    private void UpdateButtonText(TextMeshProUGUI buttonText, bool isOn)
    {
        if (buttonText != null)
        {
            buttonText.text = isOn ? "On" : "Off";
        }
    }

    // BGM ���� Ȯ��
    public bool IsBGMOn()
    {
        return isBgmOn;
    }

    // SFX ���� Ȯ��
    public bool IsSFXOn()
    {
        return isSfxOn;
    }

    // ���� ���� ����
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ���� ���� �ҷ�����
    private void LoadSoundSettings()
    {
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }
    #endregion

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
}
