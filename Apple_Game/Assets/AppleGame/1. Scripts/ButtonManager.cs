using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("Drag Mode Buttons")]
    public GameObject dragButton;           // �巡�� ��ư
    public GameObject clickButton;          // Ŭ�� ��ư
    private Image dragButtonImage;          // �巡�� ��ư �̹���
    private Image clickButtonImage;         // Ŭ�� ��ư �̹���
    private Color selectedButtonColor = new Color(0f / 255f, 200f / 255f, 0f / 255f, 255f / 255f);
    private Color unselectedButtonColor = new Color(0f / 255f, 0f / 255f, 0f / 255f, 150f / 255f);

    [Header("--------------[ ETC ]")]
    public SelectModeManager selectMode;    // SelectModeManager ����
    private Canvas mainCanvas;              // Canvas ����
    private GameObject escPanel;            // �Ͻ����� �г�

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

    // �Ͻ����� �г� ���� �ʱ�ȭ
    private void InitializePausePanel()
    {
        escPanel = mainCanvas.transform.Find("Pause Panel").gameObject;

        Button backButton = escPanel.transform.Find("BackGround/Back Button").GetComponent<Button>();
        backButton.onClick.AddListener(() => escPanel.SetActive(false));

        Button titleButton = escPanel.transform.Find("BackGround/Title Button").GetComponent<Button>();
        titleButton.onClick.AddListener(() => GoTitle());

        escPanel.SetActive(false);
    }

    // ���� ��� ��ư �ʱ�ȭ (�巡�� or Ŭ��)
    private void InitializeModeButtons()
    {
        dragButtonImage = dragButton.GetComponent<Image>();
        clickButtonImage = clickButton.GetComponent<Image>();

        dragButton.GetComponent<Button>().onClick.AddListener(SetDragMode);
        clickButton.GetComponent<Button>().onClick.AddListener(SetClickMode);

        // SelectModeManager�� ���� ��忡 ���� ��ư UI ���� ������Ʈ
        UpdateSelectModeButtonUI(selectMode.GetCurrentMode() == SelectModeManager.SelectMode.Drag);
    }

    // �巡�� ��� ����
    public void SetDragMode()
    {
        selectMode.SetSelectMode(SelectModeManager.SelectMode.Drag);
        UpdateSelectModeButtonUI(true);
    }

    // Ŭ�� ��� ����
    public void SetClickMode()
    {
        selectMode.SetSelectMode(SelectModeManager.SelectMode.Click);
        UpdateSelectModeButtonUI(false);
    }

    // ���� ��� ��ư �ð��� ������Ʈ
    private void UpdateSelectModeButtonUI(bool isNormalMode)
    {
        dragButtonImage.color = isNormalMode ? selectedButtonColor : unselectedButtonColor;
        clickButtonImage.color = isNormalMode ? unselectedButtonColor : selectedButtonColor;
    }

    // ESC(�ڷΰ���) �Է� ó��
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

    // ESC(�ڷΰ���) �г� Ȱ��ȭ ���� Ȯ��
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
