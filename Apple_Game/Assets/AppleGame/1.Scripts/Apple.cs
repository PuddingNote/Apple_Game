using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Apple : MonoBehaviour
{


    #region Variables

    [Header("--------------[ Apple Setting ]")]
    [HideInInspector] public int appleNum;                  // 사과가 가지고 있는 숫자 값
    [HideInInspector] public TextMeshProUGUI childText;     // 사과 숫자를 표시하는 Text
    [HideInInspector] public bool isDropping;               // 사과가 드랍 여부

    private Image appleImage;                               // 사과 이미지
    private Color originalColor;                            // 사과 이미지의 초기 색상
    private Color originalNumberColor;                      // 사과 숫자의 초기 색상

    private RectTransform rectTransform;                    // UI 위치와 크기를 제어하는 RectTransform

    [Header("--------------[ ETC ]")]
    private int childCount;                                 // 자식 오브젝트 개수를 캐싱하는 변수
    private Transform[] childTransforms;                    // 자식 오브젝트들의 Transform 컴포넌트 배열

    #endregion


    #region Unity Methods

    private void Start()
    {
        InitializeUI();
        InitializeApple();
    }

    #endregion


    #region Initialize

    // UI 초기화 및 원본 색상 저장 함수
    private void InitializeUI()
    {
        rectTransform = GetComponent<RectTransform>();
        appleImage = GetComponent<Image>();
        childText = transform.Find("AppleNumber").GetComponent<TextMeshProUGUI>();

        // 자식 오브젝트 캐싱
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

    // 사과 초기 설정 함수
    private void InitializeApple()
    {
        SetRandomNumber();
        isDropping = false;
    }

    #endregion


    #region Apple Number Management

    // 랜덤 숫자 생성
    public void SetRandomNumber()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= 35f)             // 35% 확률로 1~3
        {
            appleNum = Random.Range(1, 4);
        }
        else if (randomValue <= 75f)        // 40% 확률로 4~7
        {
            appleNum = Random.Range(4, 8);
        }
        else                                // 25% 확률로 8~9
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

    // 합이 10인 사과들 드롭 함수
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

    // 사과 드롭 포물선 코루틴
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

    // 드롭된 사과 숨기는 함수
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

    // 사과 보여주는 함수
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

    #endregion


}
