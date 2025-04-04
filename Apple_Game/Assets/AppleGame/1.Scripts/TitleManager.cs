using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{


    #region Variables

    [Header("--------------[ UI References ]")]
    [SerializeField] private TextMeshProUGUI highScoreText;               // 최고 점수 표시 Text
    [SerializeField] private TextMeshProUGUI highDestroyedAppleCountText; // 최고 터뜨린 사과 개수 Text

    #endregion


    #region Unity Methods

    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitializeTitleManager();
    }

    #endregion


    #region Initialize

    // UI 초기화 함수
    private void InitializeTitleManager()
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

    #endregion


}
