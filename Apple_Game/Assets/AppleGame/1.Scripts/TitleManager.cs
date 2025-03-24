using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("--------------[ Title UI ]")]
    public TextMeshProUGUI highScoreText;               // 최고 점수 표시 Text
    public TextMeshProUGUI highDestroyedAppleCountText; // 최고 터뜨린 사과 개수 Text

    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitializeTitleUI();
    }

    private void InitializeTitleUI()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "최고점수: " + highScore.ToString();
        }

        if (highDestroyedAppleCountText != null)
        {
            int highDestroyedAppleCount = PlayerPrefs.GetInt("HighDestroyedAppleCount", 0);
            highDestroyedAppleCountText.text = "최고 사과개수: " + highDestroyedAppleCount.ToString();
        }
    }

}
