using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Apple : MonoBehaviour
{
    private int appleNum;

    private void Awake()
    {
        appleNum = Random.Range(1, 10);
        TextMeshProUGUI childText = transform.Find("AppleNumber").GetComponent<TextMeshProUGUI>();

        if (childText != null)
        {
            childText.text = appleNum.ToString();
        }

    }

    public void Initialize()
    {
        // 초기화 작업
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
