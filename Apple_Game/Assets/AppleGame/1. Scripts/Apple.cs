using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Apple : MonoBehaviour
{
    [HideInInspector] public int appleNum;              // 사과 보유번호
    [HideInInspector] public TextMeshProUGUI childText; // 사과 보유번호 UI
    private Image appleImage;                           // 사과 이미지
    private Color originalColor;                        // 사과 이미지의 원래 색상
    private Color originalNumberColor;                  // 사과 숫자의 원래 색상


    private void Awake()
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
            originalNumberColor = childText.color;  // 원래 사과 숫자 색상 저장
            childText.text = appleNum.ToString();
        }

    }

    // 사과 떨어지기 메서드
    public void Drop()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.position;
        startPosition.y += 0.55f;

        float randomX = Random.Range(-1f, 1f);
        float jumpHeight = 1f;
        float animationSpeed = 0.5f;
        float animationDuration = 3.0f;
        float startTime = Time.time;

        StartCoroutine(DropCoroutine(startPosition, randomX, jumpHeight, animationSpeed, animationDuration, startTime));
    }

    private IEnumerator DropCoroutine(Vector2 startPosition, float randomX, float jumpHeight, float speed, float duration, float startTime)
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

    // 사과 숨기기 메서드
    public void HideApple()
    {
        // 사과 투명하게 만들기
        if (appleImage != null)
        {
            Color color = appleImage.color;
            color.a = 0f;
            appleImage.color = color;
        }

        // 자식 오브젝트 비활성화 = 보유한 숫자
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        this.appleNum = 0;  // 사라진 사과의 번호를 0으로 초기화
    }

    // 사과 보이기 메서드
    public void ShowApple()
    {
        // appleNum이 0인 사과는 다시 보이지 않음 = 이미 선택되고 사라진 사과들
        if (this.appleNum == 0) return;

        // 사과 다시 보이게 만들기
        if (appleImage != null)
        {
            appleImage.color = originalColor;
        }

        // 자식 오브젝트 활성화 = 보유한 숫자
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }

    }

    // 사과 숫자 색상을 변경하는 메서드
    public void ChangeNumberColor(Color color)
    {
        if (childText != null)
        {
            childText.color = color;
        }
    }

    // 사과 숫자 색상을 원래대로 복구하는 메서드
    public void ResetNumberColor()
    {
        if (childText != null)
        {
            childText.color = originalNumberColor;
        }
    }

}
