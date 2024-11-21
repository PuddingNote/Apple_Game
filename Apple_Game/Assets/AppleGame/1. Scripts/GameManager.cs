using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                      // 점수
    private int appleScore = 10;                // 사과 하나의 점수
    [HideInInspector]public bool isGameOver;    // GameOver 판단


    [Header("--------------[ Game Setting ]")]
    public GameObject applePrefab;              // 사과 Prefab
    public Transform appleGroup;                // 오브젝트 풀링을 저장할 appleGroup
    public List<Apple> applePool;               // 사과 오브젝트 풀링 리스트
    [Range(0, 120)]
    public int poolSize;                        // 오브젝트 풀링 Size

    public GameObject[] appleObjects;           // 씬에 있는 모든 사과 객체 배열
    public List<GameObject> selectedApples;     // 선택된 사과 객체 리스트
    public List<GameObject> lastSelectedApples; // 이전에 선택된 사과들을 저장할 리스트


    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;           // ScoreGroup의 Score Text
    public TextMeshProUGUI endScoreText;        // EndGroup의 End ScoreText
    public GameObject endGroup;                 // Canvas의 EndGroup
    
    [Header("--------------[ Gaugebar ]")]
    public float timeLimit = 30f;               // 기본 시간 제한(초)
    private float currentTime;                  // 현재 시간
    public Slider timeSlider;                   // UI에 표시할 게이지 바


    private void Awake()
    {
        Application.targetFrameRate = 60;       // 프레임 설정

        endGroup.SetActive(false);

        applePool = new List<Apple>();
        selectedApples = new List<GameObject>();
        lastSelectedApples = new List<GameObject>();

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

        // 게이지 바 초기화
        timeSlider.maxValue = timeLimit;
        currentTime = timeLimit;
        UpdateTimeUI();
    }


    private void Update()
    {
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;          // 시간 감소
            UpdateTimeUI();                         // UI 업데이트

            // 시간 종료 체크
            if (currentTime <= 0f)
            {
                isGameOver = true;
                GameOver();
            }
        }
    }

    // 초기 사과 생성 메서드
    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);
        Apple appleComponent = newApple.GetComponent<Apple>();

        if (appleComponent != null)
        {
            applePool.Add(appleComponent);
        }
    }

    // 선택된 사과들 초기화 메서드
    public void ClearSelectedApples()
    {
        foreach (GameObject apple in selectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ShowApple();         // 사과 객체를 다시 보이게 함
            }
        }
        selectedApples.Clear();     // 선택된 객체 리스트 초기화
    }

    // 사과 드래그 메서드
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {   
        // 드래그 영역 내 사과 객체 선택 부분
        List<GameObject> currentlySelected = new List<GameObject>();    // 현재 드래그 영역 내에 있는 사과를 임시로 저장
        foreach (GameObject apple in appleObjects)
        {
            Apple appleComponent = apple.GetComponent<Apple>();

            if (appleComponent == null || appleComponent.appleNum == 0) continue;

            Vector3 appleScreenPos = Camera.main.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            if (appleScreenPos.x >= rectMinPos.x && appleScreenPos.x <= rectMaxPos.x &&
                appleScreenPos.y >= rectMinPos.y && appleScreenPos.y <= rectMaxPos.y)
            {
                currentlySelected.Add(apple);
                if (!selectedApples.Contains(apple))
                {
                    selectedApples.Add(apple);
                }
            }
        }

        // 드래그 영역 밖 사과 객체 선택 해제 부분
        List<GameObject> applesToDeselect = new List<GameObject>();     // 드래그 영역에서 벗어난 사과 객체들을 임시로 저장
        foreach (GameObject apple in selectedApples)
        {
            if (!currentlySelected.Contains(apple))
            {
                applesToDeselect.Add(apple);
            }
        }
        foreach (GameObject apple in applesToDeselect)
        {
            selectedApples.Remove(apple);
            Apple appleComponent = apple.GetComponent<Apple>();

            if (appleComponent != null)
            {
                appleComponent.ShowApple();                 // 사과 객체를 다시 보이게 함
            }
        }
    }

    // 선택한 사과 객체들의 숫자의 합 계산 및 처리 메서드
    public void CalculateApples()
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
            AddScore();
            UpdateScore();
        }
    }

    // 합이 10인 사과들을 숨기는 메서드
    private void HideSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.HideApple();                 // 사과 객체 투명하게 만들기
            }
        }
        //selectedApples.Clear(); // 리스트 초기화
    }

    // 이전에 선택된 사과들의 숫자 색상을 초기화하는 메서드
    public void ClearLastSelectedApples()
    {
        foreach (GameObject apple in lastSelectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ResetNumberColor();          // 사과 숫자 색상을 원래대로 복구
            }
        }
        lastSelectedApples.Clear();
    }

    // 드래그 중에 선택된 사과들의 숫자 색상을 변경하는 메서드
    public void ChangeSelectedApplesNumberColor(Color color)
    {
        ClearLastSelectedApples();

        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ChangeNumberColor(color);    // 사과 숫자 색상을 변경
                lastSelectedApples.Add(selectedApple);      // 이전에 선택된 사과들 리스트에 추가
            }
        }
    }

    // 선택된 사과 객체들을 반환하는 메서드
    public List<GameObject> GetSelectedApples()
    {
        return selectedApples;
    }

    // 점수 추가 메서드
    private void AddScore()
    {
        score += lastSelectedApples.Count * appleScore;
    }

    // 점수 업데이트 메서드
    private void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
    
    // 현재시간을 게이지바에 반영하는 메서드
    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
    }

    // 게임종료 메서드
    private void GameOver()
    {
        scoreText.enabled = false;
        endScoreText.text = "Score: " + scoreText.text;
        endGroup.SetActive(true);
    }

}
