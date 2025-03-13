using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    [Header("--------------[ UI References ]")]
    private Canvas mainCanvas;              // Canvas ����
    private GameObject escPanel;            // �Ͻ����� �г�
    private GameObject optionPanel;         // �ɼ� �г�
    private GameObject helpPanel;           // ���� �г�
    private GameObject creditPanel;         // ũ���� �г�

    [Header("--------------[ Buttons ]")]
    private Button optionButton;            // �ɼ� ��ư
    private Button bgmButton;               // BGM ��ư
    private Button sfxButton;               // SFX ��ư
    private Image bgmButtonImage;           // BGM ��ư �̹���
    private Image sfxButtonImage;           // SFX ��ư �̹���
    private TextMeshProUGUI bgmButtonText;  // BGM ��ư �ؽ�Ʈ
    private TextMeshProUGUI sfxButtonText;  // SFX ��ư �ؽ�Ʈ

    [Header("--------------[ Sound Settings ]")]
    private bool isBgmOn = true;            // BGM ����
    private bool isSfxOn = true;            // SFX ����
    private Color onColor = new Color(0f, 200f / 255f, 0f); // On ���� ����
    private Color offColor = Color.red;                     // Off ���� ����

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

        SetupButton(escPanel, "BackGround/Back Button", () => escPanel.SetActive(false));
        SetupButton(escPanel, "BackGround/Title Button", GoTitle);
        SetupButton(escPanel, "BackGround/Quit Button", QuitGame);

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

        // ���� ��ư
        SetupButton(optionPanel, "BackGround/���� Button", () => helpPanel.SetActive(true));

        // ũ���� ��ư
        SetupButton(optionPanel, "BackGround/ũ���� Button", () => creditPanel.SetActive(true));

        // �ڷΰ��� ��ư
        SetupButton(optionPanel, "BackGround/Back Button", () => optionPanel.SetActive(false));

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

        SetupButton(helpPanel, "BackGround/Back Button", () => helpPanel.SetActive(false));
    }

    // ũ���� �г� ���� �ʱ�ȭ
    private void InitializeCreditPanel()
    {
        creditPanel = mainCanvas.transform.Find("Credit Panel").gameObject;
        creditPanel.SetActive(false);

        SetupButton(creditPanel, "BackGround/Back Button", () => creditPanel.SetActive(false));
    }

    // ��ư ���� ���� �޼���
    private void SetupButton(GameObject panel, string buttonPath, UnityEngine.Events.UnityAction action)
    {
        Transform buttonTransform = panel.transform.Find(buttonPath);
        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(action);
            }
        }
    }

    // ESC(�ڷΰ���) �Է� ó��
    private void HandleEscInput()
    {
        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
        bool androidBackPressed = Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape);

        if (!escapePressed && !androidBackPressed) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "TitleScene" && !optionPanel.activeSelf && !escPanel.activeSelf)
        {
            escPanel.SetActive(true);
        }
        else if (currentScene == "GameScene" && !escPanel.activeSelf)
        {
            escPanel.SetActive(true);
        }
    }

    // ESC(�ڷΰ���) �г� Ȱ��ȭ ���� Ȯ��
    public bool IsActiveEscPanel()
    {
        return escPanel.activeSelf;
    }

    // ���� ����
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Ÿ��Ʋ�� �̵�
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // ���� ����
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

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
}
