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
#if UNITY_EDITOR    // �����Ϳ��� �������� ���
        UnityEditor.EditorApplication.isPlaying = false;
#else               // ����� ���ӿ��� �������� ���
        Application.Quit();
#endif
    }
}
