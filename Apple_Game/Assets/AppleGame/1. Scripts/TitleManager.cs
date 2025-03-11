using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("--------------[ Title UI ]")]
    public TextMeshProUGUI highScoreText;   // 최고 점수 표시 Text

    private void Awake()
    {
        InitializeTitleUI();
    }

    private void InitializeTitleUI()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "최고점수: " + highScore.ToString();
        }
    }

}
