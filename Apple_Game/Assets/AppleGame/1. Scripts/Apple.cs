using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Apple : MonoBehaviour
{
    [HideInInspector]
    public int appleNum;            // 사과 보유 번호
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

    // 사과 숨기는 메서드
    public void HideApple()
    {
        // 사과 투명하게 만들기
        if (appleImage != null)
        {
            Color color = appleImage.color;
            color.a = 0f;   
            appleImage.color = color;
        }

        // 자식 오브젝트 비활성화
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        this.appleNum = 0;  // 사라진 사과들 번호 0으로 초기화
    }

}
