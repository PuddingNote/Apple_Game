using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{


    #region Variables

    [Header("--------------[ UI References ]")]
    [SerializeField] private TextMeshProUGUI highScoreText;               // �ְ� ���� ǥ�� Text
    [SerializeField] private TextMeshProUGUI highDestroyedAppleCountText; // �ְ� �Ͷ߸� ��� ���� Text

    #endregion


    #region Unity Methods

    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitializeTitleManager();
    }

    #endregion


    #region Initialize

    // UI �ʱ�ȭ �Լ�
    private void InitializeTitleManager()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "�ְ�����: " + highScore.ToString();
        }

        if (highDestroyedAppleCountText != null)
        {
            int highDestroyedAppleCount = PlayerPrefs.GetInt("HighDestroyedAppleCount", 0);
            highDestroyedAppleCountText.text = "�ְ� �������: " + highDestroyedAppleCount.ToString();
        }
    }

    #endregion


}
