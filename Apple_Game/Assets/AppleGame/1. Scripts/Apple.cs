using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Apple : MonoBehaviour
{
    [HideInInspector] public int appleNum;              // ��� ������ȣ
    [HideInInspector] public TextMeshProUGUI childText; // ��� ������ȣ UI
    private Image appleImage;                           // ��� �̹���
    private Color originalColor;                        // ��� �̹����� ���� ����
    private Color originalNumberColor;                  // ��� ������ ���� ����


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
            originalNumberColor = childText.color;  // ���� ��� ���� ���� ����
            childText.text = appleNum.ToString();
        }

    }

    // ��� �������� �޼���
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

    // ��� ����� �޼���
    public void HideApple()
    {
        // ��� �����ϰ� �����
        if (appleImage != null)
        {
            Color color = appleImage.color;
            color.a = 0f;
            appleImage.color = color;
        }

        // �ڽ� ������Ʈ ��Ȱ��ȭ = ������ ����
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        this.appleNum = 0;  // ����� ����� ��ȣ�� 0���� �ʱ�ȭ
    }

    // ��� ���̱� �޼���
    public void ShowApple()
    {
        // appleNum�� 0�� ����� �ٽ� ������ ���� = �̹� ���õǰ� ����� �����
        if (this.appleNum == 0) return;

        // ��� �ٽ� ���̰� �����
        if (appleImage != null)
        {
            appleImage.color = originalColor;
        }

        // �ڽ� ������Ʈ Ȱ��ȭ = ������ ����
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }

    }

    // ��� ���� ������ �����ϴ� �޼���
    public void ChangeNumberColor(Color color)
    {
        if (childText != null)
        {
            childText.color = color;
        }
    }

    // ��� ���� ������ ������� �����ϴ� �޼���
    public void ResetNumberColor()
    {
        if (childText != null)
        {
            childText.color = originalNumberColor;
        }
    }

}
