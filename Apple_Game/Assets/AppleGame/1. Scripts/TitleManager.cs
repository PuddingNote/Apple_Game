using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("--------------[ Title UI ]")]
    public TextMeshProUGUI highScoreText;   // �ְ� ���� ǥ�� Text

    private void Awake()
    {
        InitializeTitleUI();
    }

    private void InitializeTitleUI()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "�ְ�����: " + highScore.ToString();
        }
    }

}
