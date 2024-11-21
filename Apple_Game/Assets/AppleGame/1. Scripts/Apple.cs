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
