using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Apple : MonoBehaviour
{
    [HideInInspector] public int appleNum;                      // 사과 보유번호
    [HideInInspector] public TextMeshProUGUI childText;         // 사과 보유번호 Text
    [HideInInspector] public bool isDropping;                   // 사과가 떨어지고 있는지 여부

    private Image appleImage;                                   // 사과 이미지
    private Color originalColor;                                // 사과 이미지의 기존 색상
    private Color originalNumberColor;                          // 사과 보유번호의 기존 색상

    private RectTransform rectTransform;
    private static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        appleImage = GetComponent<Image>();
        childText = transform.Find("AppleNumber").GetComponent<TextMeshProUGUI>();

        if (appleImage != null)
        {
            originalColor = appleImage.color;
        }
        if (childText != null)
        {
            originalNumberColor = childText.color;
        }

        InitializeApple();
    }

    // 사과 초기화
    private void InitializeApple()
    {
        SetRandomNumber();
        isDropping = false;
    }

    // 랜덤 숫자 생성
    public void SetRandomNumber()
    {
        float randomValue = Random.Range(0f, 100f);
        appleNum = randomValue <= 75f ? Random.Range(1, 8) : Random.Range(8, 10);

        if (childText != null)
        {
            childText.text = appleNum.ToString();
        }
    }

    // 합이 10인 사과들 드롭
    public void DropApple()
    {
        if (!isDropping)
        {
            isDropping = true;
            Vector2 startPosition = rectTransform.position;
            startPosition.y += 0.55f;
            float randomX = Random.Range(-1f, 1f);
            StartCoroutine(DropAppleCoroutine(startPosition, randomX));
        }
    }

    // 사과 드롭 코루틴
    private IEnumerator DropAppleCoroutine(Vector2 startPosition, float randomX)
    {
        const float duration = 2f;
        const float speed = 0.5f;
        const float jumpHeight = 1f;

        float elapsedTime = 0f;
        float xMovement = 0f;
        Vector2 newPosition = startPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            xMovement += randomX * Time.deltaTime * 2;

            float t = elapsedTime / speed;
            newPosition.x = startPosition.x + xMovement;
            newPosition.y = Mathf.Lerp(startPosition.y, startPosition.y + jumpHeight, t) - (t - 0.5f) * (t - 0.5f) * jumpHeight * 4;

            rectTransform.position = newPosition;
            yield return waitForEndOfFrame;
        }

        HideApple();
    }

    // 드롭된 사과 숨기는 함수
    public void HideApple()
    {
        if (appleImage != null)
        {
            var color = appleImage.color;
            color.a = 0f;
            appleImage.color = color;
        }

        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        isDropping = false;
        appleNum = 0;
    }

    // 사과 보여주는 함수
    public void ShowApple()
    {
        if (appleNum != 0 && appleImage != null)
        {
            appleImage.color = originalColor;
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    // 사과 숫자 색 변경 함수
    public void ChangeNumberColor(Color color)
    {
        if (childText != null)
        {
            childText.color = color;
        }
    }

    // 사과 숫자 색 복원 함수
    public void ResetNumberColor()
    {
        if (childText != null)
        {
            childText.color = originalNumberColor;
        }
    }

}
