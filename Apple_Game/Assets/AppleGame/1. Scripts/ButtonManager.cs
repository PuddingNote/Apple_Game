using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("--------------[ ETC ]")]
    private Canvas mainCanvas;              // Canvas 참조

    private Button optionButton;            // 옵션 버튼

    private GameObject escPanel;            // 일시정지 패널
    private GameObject optionPanel;         // 옵션 패널
    private GameObject helpPanel;           // 도움말 패널
    private GameObject creditPanel;         // 크레딧 패널

    private void Awake()
    {
        mainCanvas = FindObjectOfType<Canvas>();
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

    // 옵션 패널 설정 초기화
    private void InitializeOptionPanel()
    {
        optionPanel = mainCanvas.transform.Find("Option Panel").gameObject;

        // BGM



        // SFX


        
        // 도움말
        Transform helpButtonTransform = optionPanel.transform.Find("BackGround/도움말 Button");
        if (helpButtonTransform != null)
        {
            Button helpButton = helpButtonTransform.GetComponent<Button>();
            helpButton.onClick.AddListener(() => helpPanel.SetActive(true));
        }

        // 크레딧
        Transform creditButtonTransform = optionPanel.transform.Find("BackGround/크레딧 Button");
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

        Transform backButtonTransform = helpPanel.transform.Find("BackGround/Back Button");
        if (backButtonTransform != null)
        {
            Button backButton = backButtonTransform.GetComponent<Button>();
            backButton.onClick.AddListener(() => helpPanel.SetActive(false));
        }
    }

    // 크레딧 패널 설정 초기화
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

    // ESC(뒤로가기) 입력 처리
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

    // ESC(뒤로가기) 패널 활성화 여부 확인
    public bool IsActiveEscPanel()
    {
        if (escPanel.activeSelf)
        {
            return true;
        }
        return false;
    }

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
