using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Apple : MonoBehaviour
{
    [HideInInspector]
    public int appleNum;            // ��� ���� ��ȣ
    TextMeshProUGUI childText;

    private Image appleImage;
    private Color originalColor;

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

        // �ڽ� ������Ʈ ��Ȱ��ȭ
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        this.appleNum = 0;  // ����� ����� ��ȣ 0���� �ʱ�ȭ
    }

}
