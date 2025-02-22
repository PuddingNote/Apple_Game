using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Drag Mode Buttons")]
    public GameObject dragButton;           // 드래그 버튼
    public GameObject clickButton;          // 클릭 버튼
    private Image dragButtonImage;          // 드래그 버튼 이미지
    private Image clickButtonImage;         // 클릭 버튼 이미지
    private Color selectedButtonColor = new Color(0f / 255f, 200f / 255f, 0f / 255f, 255f / 255f);
    private Color unselectedButtonColor = new Color(0f / 255f, 0f / 255f, 0f / 255f, 150f / 255f);

    [Header("--------------[ ETC ]")]
    public ScreenDrag screenDrag;           // ScreenDrag 참조
    private Canvas mainCanvas;              // Canvas 참조
    private GameObject escPanel;            // 일시정지 패널




    private void Awake()
    {
        mainCanvas = FindObjectOfType<Canvas>();

        InitializePausePanel();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "TitleScene")
        {
            InitializeModeButtons();
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

        Button backButton = escPanel.transform.Find("BackGround/Back Button").GetComponent<Button>();
        backButton.onClick.AddListener(() => escPanel.SetActive(false));

        Button titleButton = escPanel.transform.Find("BackGround/Title Button").GetComponent<Button>();
        titleButton.onClick.AddListener(() => GoTitle());

        escPanel.SetActive(false);
    }

    // 게임 모드 버튼 초기화 (드래그 or 클릭)
    private void InitializeModeButtons()
    {
        dragButtonImage = dragButton.GetComponent<Image>();
        clickButtonImage = clickButton.GetComponent<Image>();

        SetDragMode();

        dragButton.GetComponent<Button>().onClick.AddListener(SetDragMode);
        clickButton.GetComponent<Button>().onClick.AddListener(SetClickMode);
    }

    // 드래그 모드 설정
    public void SetDragMode()
    {
        screenDrag.SetDragMode(ScreenDrag.SelectMode.Drag);
        UpdateModeButtonVisuals(true);
    }

    // 클릭 모드 설정
    public void SetClickMode()
    {
        screenDrag.SetDragMode(ScreenDrag.SelectMode.Click);
        UpdateModeButtonVisuals(false);
    }

    // 게임 모드 버튼 시각적 업데이트
    private void UpdateModeButtonVisuals(bool isNormalMode)
    {
        dragButtonImage.color = isNormalMode ? selectedButtonColor : unselectedButtonColor;
        clickButtonImage.color = isNormalMode ? unselectedButtonColor : selectedButtonColor;
    }

    // ESC(뒤로가기) 입력 처리
    private void HandleEscInput()
    {
        bool escapePressed = Input.GetKeyDown(KeyCode.Escape);
        bool androidBackPressed = Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape);

        if (escapePressed || androidBackPressed)
        {
            if (!escPanel.activeSelf)
            {
                escPanel.SetActive(true);
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
