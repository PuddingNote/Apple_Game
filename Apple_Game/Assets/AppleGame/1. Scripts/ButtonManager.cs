using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Canvas mainCanvas;
    private GameObject escPanel;

    private void Awake()
    {
        mainCanvas = FindObjectOfType<Canvas>();

        InitializePausePanel();
    }

    private void InitializePausePanel()
    {
        escPanel = mainCanvas.transform.Find("Pause Panel").gameObject;

        Button backButton = escPanel.transform.Find("BackGround/Back Button").GetComponent<Button>();
        backButton.onClick.AddListener(() => escPanel.SetActive(false));

        Button titleButton = escPanel.transform.Find("BackGround/Title Button").GetComponent<Button>();
        titleButton.onClick.AddListener(() => GoTitle());

        escPanel.SetActive(false);
    }

    private void Update()
    {
        HandleEscapeInput();
    }

    private void HandleEscapeInput()
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
