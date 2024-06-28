using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GameEnd()
    {
#if UNITY_EDITOR    // 에디터에서 실행중인 경우
        UnityEditor.EditorApplication.isPlaying = false;
#else               // 빌드된 게임에서 실행중인 경우
        Application.Quit();
#endif
    }
}
