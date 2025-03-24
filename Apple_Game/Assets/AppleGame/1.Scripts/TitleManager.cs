using UnityEngine;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("--------------[ Title UI ]")]
    public TextMeshProUGUI highScoreText;               // �ְ� ���� ǥ�� Text
    public TextMeshProUGUI highDestroyedAppleCountText; // �ְ� �Ͷ߸� ��� ���� Text

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
            highScoreText.text = "�ְ�����: " + highScore.ToString();
        }

        if (highDestroyedAppleCountText != null)
        {
            int highDestroyedAppleCount = PlayerPrefs.GetInt("HighDestroyedAppleCount", 0);
            highDestroyedAppleCountText.text = "�ְ� �������: " + highDestroyedAppleCount.ToString();
        }
    }

}
