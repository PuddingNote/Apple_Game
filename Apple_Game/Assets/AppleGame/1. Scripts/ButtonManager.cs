using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    [Header("--------------[ UI References ]")]
    private Canvas mainCanvas;              // Canvas 참조
    private GameObject escPanel;            // 일시정지 패널
    private GameObject optionPanel;         // 옵션 패널
    private GameObject helpPanel;           // 도움말 패널
    private GameObject creditPanel;         // 크레딧 패널

    [Header("--------------[ Buttons ]")]
    private Button optionButton;            // 옵션 버튼
    private Button bgmButton;               // BGM 버튼
    private Button sfxButton;               // SFX 버튼
    private Image bgmButtonImage;           // BGM 버튼 이미지
    private Image sfxButtonImage;           // SFX 버튼 이미지
    private TextMeshProUGUI bgmButtonText;  // BGM 버튼 텍스트
    private TextMeshProUGUI sfxButtonText;  // SFX 버튼 텍스트

    [Header("--------------[ Sound Settings ]")]
    private bool isBgmOn = true;            // BGM 상태
    private bool isSfxOn = true;            // SFX 상태
    private Color onColor = new Color(0f, 200f / 255f, 0f); // On 상태 색상
    private Color offColor = Color.red;                     // Off 상태 색상

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

    // 일시정지 패널 설정 초기화
    private void InitializePausePanel()
    {
        escPanel = mainCanvas.transform.Find("Pause Panel").gameObject;

        SetupButton(escPanel, "BackGround/Back Button", () => escPanel.SetActive(false));
        SetupButton(escPanel, "BackGround/Title Button", GoTitle);
        SetupButton(escPanel, "BackGround/Quit Button", QuitGame);

        escPanel.SetActive(false);
    }

    // 옵션 패널 설정 초기화
    private void InitializeOptionPanel()
    {
        optionPanel = mainCanvas.transform.Find("Option Panel").gameObject;

        // BGM On/Off 버튼
        Transform bgmButtonTransform = optionPanel.transform.Find("BackGround/Sound Objects/배경음 Button");
        if (bgmButtonTransform != null)
        {
            bgmButton = bgmButtonTransform.GetComponent<Button>();
            bgmButtonImage = bgmButtonTransform.GetComponent<Image>();
            bgmButtonText = bgmButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
            bgmButton.onClick.AddListener(ToggleBGM);
            UpdateButtonColor(bgmButtonImage, isBgmOn);
            UpdateButtonText(bgmButtonText, isBgmOn);
        }

        // SFX On/Off 버튼
        Transform sfxButtonTransform = optionPanel.transform.Find("BackGround/Sound Objects/효과음 Button");
        if (sfxButtonTransform != null)
        {
            sfxButton = sfxButtonTransform.GetComponent<Button>();
            sfxButtonImage = sfxButtonTransform.GetComponent<Image>();
            sfxButtonText = sfxButtonTransform.GetComponentInChildren<TextMeshProUGUI>();
            sfxButton.onClick.AddListener(ToggleSFX);
            UpdateButtonColor(sfxButtonImage, isSfxOn);
            UpdateButtonText(sfxButtonText, isSfxOn);
        }

        // 도움말 버튼
        SetupButton(optionPanel, "BackGround/도움말 Button", () => helpPanel.SetActive(true));

        // 크레딧 버튼
        SetupButton(optionPanel, "BackGround/크레딧 Button", () => creditPanel.SetActive(true));

        // 뒤로가기 버튼
        SetupButton(optionPanel, "BackGround/Back Button", () => optionPanel.SetActive(false));

        optionPanel.SetActive(false);
    }

    // 옵션 버튼 초기화
    private void InitializeOptionButton()
    {
        optionButton = mainCanvas.transform.Find("Start Group/Option Button").GetComponent<Button>();
        if (optionButton != null)
        {
            optionButton.onClick.AddListener(() => optionPanel.SetActive(true));
        }
    }

    // 도움말 패널 설정 초기화
    private void InitializeHelpPanel()
    {
        helpPanel = mainCanvas.transform.Find("Help Panel").gameObject;
        helpPanel.SetActive(false);

        SetupButton(helpPanel, "BackGround/Back Button", () => helpPanel.SetActive(false));
    }

    // 크레딧 패널 설정 초기화
    private void InitializeCreditPanel()
    {
        creditPanel = mainCanvas.transform.Find("Credit Panel").gameObject;
        creditPanel.SetActive(false);

        SetupButton(creditPanel, "BackGround/Back Button", () => creditPanel.SetActive(false));
    }

    // 버튼 설정 헬퍼 메서드
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

    // ESC(뒤로가기) 입력 처리
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

    // ESC(뒤로가기) 패널 활성화 여부 확인
    public bool IsActiveEscPanel()
    {
        return escPanel.activeSelf;
    }

    // 게임 시작
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 타이틀로 이동
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 게임 종료
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // BGM 토글
    private void ToggleBGM()
    {
        isBgmOn = !isBgmOn;
        UpdateButtonColor(bgmButtonImage, isBgmOn);
        UpdateButtonText(bgmButtonText, isBgmOn);
        SaveSoundSettings();

        // SoundManager 업데이트
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateBGMState();
        }
    }

    // SFX 토글
    private void ToggleSFX()
    {
        isSfxOn = !isSfxOn;
        UpdateButtonColor(sfxButtonImage, isSfxOn);
        UpdateButtonText(sfxButtonText, isSfxOn);
        SaveSoundSettings();

        // SoundManager 업데이트
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UpdateSFXState();
        }
    }

    // 버튼 색상 업데이트
    private void UpdateButtonColor(Image buttonImage, bool isOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isOn ? onColor : offColor;
        }
    }

    // 버튼 텍스트 업데이트
    private void UpdateButtonText(TextMeshProUGUI buttonText, bool isOn)
    {
        if (buttonText != null)
        {
            buttonText.text = isOn ? "On" : "Off";
        }
    }

    // BGM 상태 확인
    public bool IsBGMOn()
    {
        return isBgmOn;
    }

    // SFX 상태 확인
    public bool IsSFXOn()
    {
        return isSfxOn;
    }

    // 사운드 설정 저장
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 사운드 설정 불러오기
    private void LoadSoundSettings()
    {
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }
}
