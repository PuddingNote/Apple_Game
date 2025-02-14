using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Canvas mainCanvas;
    private GameObject escPanel;

    private void Start()
    {
        mainCanvas = FindObjectOfType<Canvas>();
        escPanel = mainCanvas.transform.Find("Quit Panel").gameObject;
        Button backButton = escPanel.transform.Find("BackGround/Back Button").GetComponent<Button>();
        Button titleButton = escPanel.transform.Find("BackGround/Title Button").GetComponent<Button>();
        backButton.onClick.AddListener(() => escPanel.SetActive(false));
        titleButton.onClick.AddListener(() => GoTitle());
        escPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)))
        {
            if (escPanel.activeSelf)
            {
                escPanel.SetActive(false);
            }
            else
            {
                escPanel.SetActive(true);
            }
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
