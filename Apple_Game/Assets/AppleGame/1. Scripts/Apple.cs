using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Apple : MonoBehaviour
{
    [HideInInspector] public int appleNum;                      // ��� ������ȣ
    [HideInInspector] public TextMeshProUGUI childText;         // ��� ������ȣ Text
    [HideInInspector] public bool isDropping;                   // ����� �������� �ִ��� ����

    private Image appleImage;                                   // ��� �̹���
    private Color originalColor;                                // ��� �̹����� ���� ����
    private Color originalNumberColor;                          // ��� ������ȣ�� ���� ����

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

    // ��� �ʱ�ȭ
    private void InitializeApple()
    {
        SetRandomNumber();
        isDropping = false;
    }

    // ���� ���� ����
    public void SetRandomNumber()
    {
        float randomValue = Random.Range(0f, 100f);
        appleNum = randomValue <= 75f ? Random.Range(1, 8) : Random.Range(8, 10);

        if (childText != null)
        {
            childText.text = appleNum.ToString();
        }
    }

    // ���� 10�� ����� ���
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

    // ��� ��� �ڷ�ƾ
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

    // ��ӵ� ��� ����� �Լ�
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

    // ��� �����ִ� �Լ�
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

    // ��� ���� �� ���� �Լ�
    public void ChangeNumberColor(Color color)
    {
        if (childText != null)
        {
            childText.color = color;
        }
    }

    // ��� ���� �� ���� �Լ�
    public void ResetNumberColor()
    {
        if (childText != null)
        {
            childText.color = originalNumberColor;
        }
    }

}
