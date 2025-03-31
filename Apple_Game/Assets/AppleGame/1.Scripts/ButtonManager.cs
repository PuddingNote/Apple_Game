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
    private Canvas mainCanvas;                      // Canvas 참조

    private GameObject escPanel;                    // 일시정지 패널
    private GameObject optionPanel;                 // 옵션 패널
    private GameObject helpPanel;                   // 도움말 패널
    private GameObject creditPanel;                 // 크레딧 패널

    [Header("--------------[ Buttons ]")]
    private Button optionButton;                    // 옵션 버튼
    private Button bgmButton;                       // BGM 버튼
    private Button sfxButton;                       // SFX 버튼

    private Image bgmButtonImage;                   // BGM 버튼 이미지
    private Image sfxButtonImage;                   // SFX 버튼 이미지

    private TextMeshProUGUI bgmButtonText;          // BGM 버튼 Text
    private TextMeshProUGUI sfxButtonText;          // SFX 버튼 Text

    [Header("--------------[ Sound Settings ]")]    
    private bool isBgmOn = true;                    // BGM 상태
    private bool isSfxOn = true;                    // SFX 상태
    private readonly Color onColor = new Color(0f, 200f / 255f, 0f);    // On 상태 색상
    private readonly Color offColor = Color.red;                        // Off 상태 색상

    [Header("--------------[ ETC ]")]
    private string currentSceneName;                                    // 현재 활성화된 씬의 이름
    private Vector3 buttonPressScale = new Vector3(0.95f, 0.95f, 1f);   // 버튼이 눌렸을 때의 크기 변화값

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

    // 메인 버튼 초기화 함수
    private void InitializeMainButton()
    {
        // 게임 시작 버튼 설정
        Button startButton = mainCanvas.transform.Find("Start Group/Start Button").GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
            AddButtonClickFeedback(startButton);
        }

        // 게임 종료 버튼 설정
        Button quitButton = mainCanvas.transform.Find("Start Group/Quit Button").GetComponent<Button>();
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
            AddButtonClickFeedback(quitButton);
        }
    }

    // 일시정지 패널 설정 초기화 함수
    private void InitializePausePanel()
    {
        escPanel = mainCanvas.transform.Find("Pause Panel").gameObject;

        SetupButton(escPanel, "BackGround/Back Button", () => escPanel.SetActive(false));
        SetupButton(escPanel, "BackGround/Title Button", GoTitle);
        SetupButton(escPanel, "BackGround/Quit Button", QuitGame);

        escPanel.SetActive(false);
    }

    // 옵션 패널 설정 초기화 함수
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

            AddButtonClickFeedback(bgmButton);
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

            AddButtonClickFeedback(sfxButton);
        }

        // 도움말 버튼
        SetupButton(optionPanel, "BackGround/도움말 Button", () => helpPanel.SetActive(true));

        // 크레딧 버튼
        SetupButton(optionPanel, "BackGround/크레딧 Button", () => creditPanel.SetActive(true));

        // 뒤로가기 버튼
        SetupButton(optionPanel, "BackGround/Back Button", () => optionPanel.SetActive(false));

        optionPanel.SetActive(false);
    }

    // 옵션 버튼 초기화 함수
    private void InitializeOptionButton()
    {
        optionButton = mainCanvas.transform.Find("Start Group/Option Button").GetComponent<Button>();
        if (optionButton != null)
        {
            optionButton.onClick.AddListener(() => optionPanel.SetActive(true));

            AddButtonClickFeedback(optionButton);
        }
    }

    // 도움말 패널 설정 초기화 함수
    private void InitializeHelpPanel()
    {
        helpPanel = mainCanvas.transform.Find("Help Panel").gameObject;
        helpPanel.SetActive(false);

        SetupButton(helpPanel, "BackGround/Back Button", () => helpPanel.SetActive(false));
    }

    // 크레딧 패널 설정 초기화 함수
    private void InitializeCreditPanel()
    {
        creditPanel = mainCanvas.transform.Find("Credit Panel").gameObject;
        creditPanel.SetActive(false);

        SetupButton(creditPanel, "BackGround/Back Button", () => creditPanel.SetActive(false));
    }

    #endregion


    #region Button Utilities

    // 버튼 설정 헬퍼 함수
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

    // 버튼 클릭 효과 추가 함수
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

        // 포인터 다운, 업 이벤트 추가
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

    // 버튼 눌림 효과 함수
    private void OnButtonDown(Transform buttonTransform)
    {
        buttonTransform.localScale = buttonPressScale;
    }

    // 버튼 뗌 효과 함수
    private void OnButtonUp(Transform buttonTransform)
    {
        buttonTransform.localScale = Vector3.one;
    }

    #endregion


    #region Input Handling

    // ESC(뒤로가기) 입력 처리 함수
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

    // ESC(뒤로가기) 패널 활성화 여부 확인 함수
    public bool IsActiveEscPanel()
    {
        return escPanel.activeSelf;
    }

    #endregion


    #region Scene Management

    // 게임 시작 함수
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // 타이틀로 이동 함수
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // 게임 종료 함수
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

    // BGM 토글 함수
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

    // SFX 토글 함수
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

    // 버튼 색상 업데이트 함수
    private void UpdateButtonColor(Image buttonImage, bool isOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isOn ? onColor : offColor;
        }
    }

    // 버튼 텍스트 업데이트 함수
    private void UpdateButtonText(TextMeshProUGUI buttonText, bool isOn)
    {
        if (buttonText != null)
        {
            buttonText.text = isOn ? "On" : "Off";
        }
    }

    // BGM 상태 확인 함수
    public bool IsBGMOn()
    {
        return isBgmOn;
    }

    // SFX 상태 확인 함수
    public bool IsSFXOn()
    {
        return isSfxOn;
    }

    // 사운드 설정 저장 함수
    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("BGM", isBgmOn ? 1 : 0);
        PlayerPrefs.SetInt("SFX", isSfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // 사운드 설정 불러오기 함수
    private void LoadSoundSettings()
    {
        isBgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
    }

    #endregion


}
