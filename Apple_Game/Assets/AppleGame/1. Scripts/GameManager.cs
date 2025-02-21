using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                          // 점수
    private int appleScore = 10;                    // 사과 하나의 점수
    [HideInInspector] public bool isGameOver;       // GameOver 판단

    [Header("--------------[ Game Setting ]")]
    public GameObject applePrefab;                  // 사과 Prefab
    public Transform appleGroup;                    // 오브젝트 풀링을 저장할 appleGroup 위치
    private List<Apple> applePool;                  // 사과 오브젝트 풀링 리스트
    private int poolSize = 120;                     // 오브젝트 풀링 Size

    private GameObject[] appleObjects;              // 씬에 있는 모든 사과 객체 배열
    private List<GameObject> selectedApples;        // 선택된 사과 객체 리스트
    private List<GameObject> lastSelectedApples;    // 이전에 선택된 사과들을 저장할 리스트

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;               // ScoreGroup의 Score Text
    public TextMeshProUGUI endScoreText;            // EndGroup의 End ScoreText
    public GameObject endGroup;                     // Canvas의 EndGroup
    public RectTransform appleImageRect;            // EndGroup의 Apple Image의 좌표
    public TextMeshProUGUI timeText;                // 남은 시간을 표시할 Text

    [Header("--------------[ Gaugebar ]")]
    public Slider timeSlider;                       // UI에 표시할 게이지바
    private float timeLimit = 60f;                  // 기본 시간 제한(초)
    private float currentTime;                      // 현재 시간

    [Header("--------------[ Effects ]")]
    public GameObject effectPrefab;                 // 이펙트 프리팹

    [Header("--------------[ ETC ]")]
    public ScreenDrag screenDrag;
    private Camera mainCamera;

    private void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        mainCamera = Camera.main;

        endGroup.SetActive(false);

        applePool = new List<Apple>(poolSize);
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

        // 게이지바 초기화
        timeSlider.maxValue = timeLimit;
        currentTime = timeLimit;
        UpdateTimeUI();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;
            UpdateTimeUI();

            if (currentTime <= 0f)
            {
                isGameOver = true;
                GameOver();
            }
        }
    }

    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);
        Apple appleComponent = newApple.GetComponent<Apple>();

        if (appleComponent != null)
        {
            applePool.Add(appleComponent);
        }
    }

    // 사과 드래그
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {
        List<GameObject> currentlySelected = new List<GameObject>(poolSize);
        foreach (GameObject apple in appleObjects)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent == null || appleComponent.appleNum == 0)
            {
                continue;
            }

            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
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

        List<GameObject> applesToDeselect = new List<GameObject>();
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
            appleComponent.ShowApple();
        }
    }

    // 선택된 사과들의 합 계산
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
            SpawnEffectsOnSelectedApples();
            HideSelectedApples();
            AddScore();
            UpdateScore();
        }
    }

    // 선택된 사과의 위치에 이펙트 생성
    private void SpawnEffectsOnSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            Vector3 effectPosition = selectedApple.transform.position;

            GameObject effect = Instantiate(effectPrefab, effectPosition, Quaternion.identity, null);

            ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
            particleSystem.Play();

            Destroy(effect, 1.0f);
        }
    }

    // 선택된 사과를 숨기기
    private void HideSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            appleComponent.DropApple();
        }
    }

    // 선택된 사과들 초기화
    public void ClearSelectedApples()
    {
        foreach (GameObject apple in selectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            appleComponent.ShowApple();
        }
        selectedApples.Clear();
    }

    // 이전에 선택된 사과들의 숫자 색상을 초기화
    public void ClearLastSelectedApples()
    {
        foreach (GameObject apple in lastSelectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            appleComponent.ResetNumberColor();
        }
        lastSelectedApples.Clear();
    }

    // 드래그중 선택된 사과들의 숫자 색상을 변경
    public void ChangeSelectedApplesNumberColor(Color color)
    {
        ClearLastSelectedApples();

        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ChangeNumberColor(color);
                lastSelectedApples.Add(selectedApple);
            }
        }
    }

    private void AddScore()
    {
        int additionalScore = 0;
        int targetCount = 2;

        if (lastSelectedApples.Count > targetCount)
        {
            additionalScore = (lastSelectedApples.Count - targetCount) * 5;
        }
        score += (lastSelectedApples.Count * appleScore) + additionalScore;
    }

    private void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
        timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    private void GameOver()
    {
        screenDrag.EndDrag();
        ClearLastSelectedApples();

        endScoreText.text = "Score: " + scoreText.text;

        Vector2 startPosition = new Vector2(appleImageRect.anchoredPosition.x, 1050);
        appleImageRect.anchoredPosition = startPosition;

        endGroup.SetActive(true);

        float targetY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0)).y;
        appleImageRect.DOMoveY(targetY, 1f).SetEase(Ease.OutBounce);
    }

}
