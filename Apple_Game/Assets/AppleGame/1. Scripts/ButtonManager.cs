using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("--------------[ ETC ]")]
    private Canvas mainCanvas;              // Canvas 참조
    private GameObject escPanel;            // 일시정지 패널

    private void Awake()
    {
        mainCanvas = FindObjectOfType<Canvas>();

        InitializePausePanel();
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
