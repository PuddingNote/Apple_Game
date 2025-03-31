using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{


    #region Variables

    [Header("--------------[ UI References ]")]
    private Canvas mainCanvas;                      // Canvas ����

    private GameObject escPanel;                    // �Ͻ����� �г�
    private GameObject optionPanel;                 // �ɼ� �г�
    private GameObject helpPanel;                   // ���� �г�
    private GameObject creditPanel;                 // ũ���� �г�

    [Header("--------------[ Buttons ]")]
    private Button optionButton;                    // �ɼ� ��ư
    private Button bgmButton;                       // BGM ��ư
    private Button sfxButton;                       // SFX ��ư

    private Image bgmButtonImage;                   // BGM ��ư �̹���
    private Image sfxButtonImage;                   // SFX ��ư �̹���

    private TextMeshProUGUI bgmButtonText;          // BGM ��ư Text
    private TextMeshProUGUI sfxButtonText;          // SFX ��ư Text

    [Header("--------------[ Sound Settings ]")]    
    private bool isBgmOn = true;                    // BGM ����
    private bool isSfxOn = true;                    // SFX ����
    private readonly Color onColor = new Color(0f, 200f / 255f, 0f);    // On ���� ����
    private readonly Color offColor = Color.red;                        // Off ���� ����

    [Header("--------------[ ETC ]")]
    private string currentSceneName;                                    // ���� Ȱ��ȭ�� ���� �̸�
    private Vector3 buttonPressScale = new Vector3(0.95f, 0.95f, 1f);   // ��ư�� ������ ���� ũ�� ��ȭ��

    #endregion


    #region Unity Methods

    private void Awake()
    {
        mainCanvas = FindObjectOfType<Canvas>();
        LoadSoundSettings();
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        InitializePausePanel();

        if (currentSceneName == "TitleScene")
        {
            InitializeMainButton();
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

    #endregion


    #region Initialize

    // ���� ��ư �ʱ�ȭ �Լ�
    private void InitializeMainButton()
    {
        // ���� ���� ��ư ����
        Button startButton = mainCanvas.transform.Find("Start Group/Start Button").GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
            AddButtonClickFeedback(startButton);
        }

        // ���� ���� ��ư ����
        Button quitButton = mainCanvas.transform.Find("Start Group/Quit Button").GetComponent<Button>();
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            AddButtonClickFeedback(quitButton);
        }
    }

    // �Ͻ����� �г� ���� �ʱ�ȭ �Լ�
    private void InitializePausePanel()
    {
        escPanel = mainCanvas.transform.Find("Pause Panel").gameObject;

        SetupButton(escPanel, "BackGround/Back Button", () => escPanel.SetActive(false));
        SetupButton(escPanel, "BackGround/Title Button", GoTitle);
        SetupButton(escPanel, "BackGround/Quit Button", QuitGame);

        escPanel.SetActive(false);
    }

    // �ɼ� �г� ���� �ʱ�ȭ �Լ�
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

            AddButtonClickFeedback(bgmButton);
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

            AddButtonClickFeedback(sfxButton);
        }

        // ���� ��ư
        SetupButton(optionPanel, "BackGround/���� Button", () => helpPanel.SetActive(true));

        // ũ���� ��ư
        SetupButton(optionPanel, "BackGround/ũ���� Button", () => creditPanel.SetActive(true));

        // �ڷΰ��� ��ư
        SetupButton(optionPanel, "BackGround/Back Button", () => optionPanel.SetActive(false));

        optionPanel.SetActive(false);
    }

    // �ɼ� ��ư �ʱ�ȭ �Լ�
    private void InitializeOptionButton()
    {
        optionButton = mainCanvas.transform.Find("Start Group/Option Button").GetComponent<Button>();
        if (optionButton != null)
        {
            optionButton.onClick.AddListener(() => optionPanel.SetActive(true));

            AddButtonClickFeedback(optionButton);
        }
    }

    // ���� �г� ���� �ʱ�ȭ �Լ�
    private void InitializeHelpPanel()
    {
        helpPanel = mainCanvas.transform.Find("Help Panel").gameObject;
        helpPanel.SetActive(false);

        SetupButton(helpPanel, "BackGround/Back Button", () => helpPanel.SetActive(false));
    }

    // ũ���� �г� ���� �ʱ�ȭ �Լ�
    private void InitializeCreditPanel()
    {
        creditPanel = mainCanvas.transform.Find("Credit Panel").gameObject;
        creditPanel.SetActive(false);

        SetupButton(creditPanel, "BackGround/Back Button", () => creditPanel.SetActive(false));
    }

    #endregion


    #region Button Utilities

    // ��ư ���� ���� �Լ�
    private void SetupButton(GameObject panel, string buttonPath, UnityEngine.Events.UnityAction action)
    {
        Transform buttonTransform = panel.transform.Find(buttonPath);
        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(action);

                AddButtonClickFeedback(button);
            }
        }
    }

    // ��ư Ŭ�� ȿ�� �߰� �Լ�
    private void AddButtonClickFeedback(Button button)
    {
        if (!Application.isMobilePlatform && !Application.isEditor)
        {
            return;
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
            trigger.triggers = new List<EventTrigger.Entry>(2);
        }

        // ������ �ٿ�, �� �̺�Ʈ �߰�
        if (trigger.triggers.Count == 0)
        {
            var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };

            pointerDown.callback.AddListener((data) => { OnButtonDown(button.transform); });
            pointerUp.callback.AddListener((data) => { OnButtonUp(button.transform); });

            trigger.triggers.Add(pointerDown);
            trigger.triggers.Add(pointerUp);
        }
    }

    // ��ư ���� ȿ�� �Լ�
    private void OnButtonDown(Transform buttonTransform)
    {
        buttonTransform.localScale = buttonPressScale;
    }

    // ��ư �� ȿ�� �Լ�
    private void OnButtonUp(Transform buttonTransform)
    {
        buttonTransform.localScale = Vector3.one;
    }

    #endregion


    #region Input Handling

    // ESC(�ڷΰ���) �Է� ó�� �Լ�
    private void HandleEscInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) && !(Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)))
        {
            return;
        }

        if (currentSceneName == "TitleScene")
        {
            if (!optionPanel.activeSelf && !escPanel.activeSelf)
            {
                escPanel.SetActive(true);
            }
        }
        else if (currentSceneName == "GameScene")
        {
            GameObject setNumberPanel = GameObject.Find("SetNumber Panel");
            if ((setNumberPanel == null || !setNumberPanel.activeSelf) && !escPanel.activeSelf)
            {
                escPanel.SetActive(true);
            }
        }
    }

    // ESC(�ڷΰ���) �г� Ȱ��ȭ ���� Ȯ�� �Լ�
    public bool IsActiveEscPanel()
    {
        return escPanel.activeSelf;
    }

    #endregion


    #region Scene Management

    // ���� ���� �Լ�
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Ÿ��Ʋ�� �̵� �Լ�
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // ���� ���� �Լ�
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion


    #region Sound Management

    // BGM ��� �Լ�
    private void ToggleBGM()
    {
        isBgmOn = !isBgmOn;
        UpdateButtonColor(bgmButtonImage, isBgmOn);
        UpdateButtonText(bgmButtonText, isBgmOn);
        SaveSoundSettings();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateBGMState();
        }
    }

    // SFX ��� �Լ�
    private void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        UpdateButtonColor(sfxButtonImage, isSfxOn);
        UpdateButtonText(sfxButtonText, isSfxOn);
        SaveSoundSettings();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateSFXState();
        }
    }

    // ��ư ���� ������Ʈ �Լ�
    private void UpdateButtonColor(Image buttonImage, bool isOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isOn ? onColor : offColor;
        }
    }

    // ��ư �ؽ�Ʈ ������Ʈ �Լ�
    private void UpdateButtonText(TextMeshProUGUI buttonText, bool isOn)
    {
        if (buttonText != null)
        {
            buttonText.text = isOn ? "On" : "Off";
        }
    }

    // BGM ���� Ȯ�� �Լ�
    public bool IsBGMOn()
    {
        return isBgmOn;
    }

    // SFX ���� Ȯ�� �Լ�
    public bool IsSFXOn()
    {
        return isSfxOn;
    }

    // ���� ���� ���� �Լ�
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ���� ���� �ҷ����� �Լ�
    private void LoadSoundSettings()
    {
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }

    #endregion


}
