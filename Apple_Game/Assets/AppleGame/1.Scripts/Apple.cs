using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Apple : MonoBehaviour
{


    #region Variables

    [Header("--------------[ Apple Setting ]")]
    [HideInInspector] public int appleNum;                  // ����� ������ �ִ� ���� ��
    [HideInInspector] public TextMeshProUGUI childText;     // ��� ���ڸ� ǥ���ϴ� Text
    [HideInInspector] public bool isDropping;               // ����� ��� ����

    private Image appleImage;                               // ��� �̹���
    private Color originalColor;                            // ��� �̹����� �ʱ� ����
    private Color originalNumberColor;                      // ��� ������ �ʱ� ����

    private RectTransform rectTransform;                    // UI ��ġ�� ũ�⸦ �����ϴ� RectTransform

    [Header("--------------[ ETC ]")]
    private int childCount;                                 // �ڽ� ������Ʈ ������ ĳ���ϴ� ����
    private Transform[] childTransforms;                    // �ڽ� ������Ʈ���� Transform ������Ʈ �迭

    #endregion


    #region Unity Methods

    private void Start()
    {
        InitializeUI();
        InitializeApple();
    }

    #endregion


    #region Initialize

    // UI �ʱ�ȭ �� ���� ���� ���� �Լ�
    private void InitializeUI()
    {
        rectTransform = GetComponent<RectTransform>();
        appleImage = GetComponent<Image>();
        childText = transform.Find("AppleNumber").GetComponent<TextMeshProUGUI>();

        // �ڽ� ������Ʈ ĳ��
        childCount = transform.childCount;
        childTransforms = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            childTransforms[i] = transform.GetChild(i);
        }

        if (appleImage != null)
        {
            originalColor = appleImage.color;
        }
        if (childText != null)
        {
            originalNumberColor = childText.color;
        }
    }

    // ��� �ʱ� ���� �Լ�
    private void InitializeApple()
    {
        SetRandomNumber();
        isDropping = false;
    }

    #endregion


    #region Apple Number Management

    // ���� ���� ����
    public void SetRandomNumber()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= 35f)             // 35% Ȯ���� 1~3
        {
            appleNum = Random.Range(1, 4);
        }
        else if (randomValue <= 75f)        // 40% Ȯ���� 4~7
        {
            appleNum = Random.Range(4, 8);
        }
        else                                // 25% Ȯ���� 8~9
        {
            appleNum = Random.Range(8, 10);
        }

        if (childText != null)
        {
            childText.text = appleNum.ToString();
        }
    }

    #endregion


    #region Apple Movement

    // ���� 10�� ����� ��� �Լ�
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

    // ��� ��� ������ �ڷ�ƾ
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
            yield return new WaitForEndOfFrame();
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

        for (int i = 0; i < childCount; i++)
        {
            childTransforms[i].gameObject.SetActive(false);
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
            
            for (int i = 0; i < childCount; i++)
            {
                childTransforms[i].gameObject.SetActive(true);
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

    #endregion


}
