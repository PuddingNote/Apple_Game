using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Apple : MonoBehaviour
{
    [HideInInspector] public int appleNum;                  // 사과 보유번호
    [HideInInspector] public TextMeshProUGUI childText;     // 사과 보유번호 UI
    private Image appleImage;                               // 사과 이미지
    private Color originalColor;                            // 사과 이미지의 기존 색상
    private Color originalNumberColor;                      // 사과 보유번호의 기존 색상

    private void Awake()
    {
        InitializeApple();
    }

    private void InitializeApple()
    {
        appleNum = Random.Range(1, 10);
        appleImage = GetComponent<Image>();
        childText = transform.Find("AppleNumber").GetComponent<TextMeshProUGUI>();

        if (appleImage != null)
        {
            originalColor = appleImage.color;
        }

        if (childText != null)
        {
            originalNumberColor = childText.color;
            childText.text = appleNum.ToString();
        }
    }

    public void DropApple()
    {
        Vector2 startPosition = GetComponent<RectTransform>().position;
        startPosition.y += 0.55f;

        float randomX = Random.Range(-1f, 1f);
        StartCoroutine(DropAppleCoroutine(startPosition, randomX, 1f, 0.5f, 3.0f));
    }

    private IEnumerator DropAppleCoroutine(Vector2 startPosition, float randomX, float jumpHeight, float speed, float duration)
    {
        float elapsedTime = 0f;
        float xMovement = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            xMovement += randomX * Time.deltaTime * 2;

            float t = elapsedTime / speed;
            float x = startPosition.x + xMovement;
            float y = Mathf.Lerp(startPosition.y, startPosition.y + jumpHeight, t) - Mathf.Pow(t - 0.5f, 2) * jumpHeight * 4;

            transform.position = new Vector2(x, y);
            yield return null;
        }

        HideApple();
    }

    public void HideApple()
    {
        if (appleImage != null)
        {
            Color color = appleImage.color;
            color.a = 0f;
            appleImage.color = color;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        appleNum = 0;
    }

    public void ShowApple()
    {
        if (appleNum == 0)
        {
            return;
        }

        if (appleImage != null)
        {
            appleImage.color = originalColor;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void ChangeNumberColor(Color color)
    {
        if (childText != null)
        {
            childText.color = color;
        }
    }

    public void ResetNumberColor()
    {
        if (childText != null)
        {
            childText.color = originalNumberColor;
        }
    }

}
