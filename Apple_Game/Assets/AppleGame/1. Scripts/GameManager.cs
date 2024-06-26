using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]


    [Header("--------------[ Main ]")]
    public int score;               // 점수
    public bool isGameOver;         // GameOver 판단

    [Header("--------------[ Object Pooling ]")]
    public GameObject applePrefab;
    public Transform appleGroup;
    public List<Apple> applePool;

    [Range(0, 120)]
    public int poolSize;            // 오브젝트 풀링 Size

    [Header("--------------[ Game Settings ]")]
    //public int horizontalLength;
    //public int verticalLength;

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;


    [Header("--------------[ TEST ]")]
    public GameObject[] appleObjects;           // 씬에 있는 모든 사과 객체 배열
    public List<GameObject> selectedApples;     // 선택된 사과 객체 리스트


    private void Awake()
    {
        Application.targetFrameRate = 60;   // 프레임 설정
        applePool = new List<Apple>();
        selectedApples = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            MakeApple();
        }

        // appleObjects 배열 초기화 및 applePool의 정보를 복사
        appleObjects = new GameObject[applePool.Count];
        for (int i = 0; i < applePool.Count; i++)
        {
            appleObjects[i] = applePool[i].gameObject;
        }

    }

    
    private void Update()
    {
        //if (!isGameOver)
        //{
        //    UpdateScore();
        //}
    }

    // 초기 사과 생성 메서드
    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);
        Apple appleComponent = newApple.GetComponent<Apple>();

        if (appleComponent != null)
        {
            //appleComponent.Initialize();
            applePool.Add(appleComponent);
        }
    }

    // 선택된 사과들 초기화 메서드
    public void ClearSelectedApples()
    {
        selectedApples.Clear();  // 선택된 객체 리스트 초기화
    }

    // 사과 드래그 메서드
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {
        foreach (GameObject apple in appleObjects)
        {
            Vector3 appleScreenPos = Camera.main.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            if (appleScreenPos.x >= rectMinPos.x && appleScreenPos.x <= rectMaxPos.x &&
                appleScreenPos.y >= rectMinPos.y && appleScreenPos.y <= rectMaxPos.y)
            {
                if (!selectedApples.Contains(apple))
                {
                    selectedApples.Add(apple);
                }
            }
        }

        CalculateApples();
    }

    // 선택한 사과 객체들의 숫자의 합 계산 및 처리 메서드
    private void CalculateApples()
    {
        int totalAppleNum = 0;

        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                totalAppleNum += appleComponent.appleNum;
            }
        }

        if (totalAppleNum == 10)
        {
            HideSelectedApples();
        }
    }

    // 합이 10인 사과들을 숨기는 메서드
    private void HideSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            //selectedApple.SetActive(false);
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.HideApple(); // 사과 객체 투명하게 만들기
            }
        }
        ClearSelectedApples(); // 리스트 초기화
    }

    // 선택된 사과 객체들을 반환하는 메서드
    public List<GameObject> GetSelectedApples()
    {
        return selectedApples;
    }


    //public void UpdateScore()
    //{
    //    scoreText.text = score.ToString();
    //}

}
