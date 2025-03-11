using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                          // 현재 게임 점수
    private int highScore = 0;                      // 최고 점수
    private int appleScore = 10;                    // 기본 사과 점수
    [HideInInspector] public bool isGameOver;       // 게임 종료 상태 여부

    [Header("--------------[ Game Setting ]")]
    public GameObject applePrefab;                  // 사과 Prefab
    public Transform appleGroup;                    // 오브젝트 풀링을 설정할 사과 부모 오브젝트
    private List<Apple> applePool;                  // 사과 오브젝트 풀
    private const int POOL_SIZE = 120;              // 오브젝트 풀 Size
    private const int TARGET_COUNT = 3;             // 보너스 점수를 위한 사과 객체 카운트

    [HideInInspector] public GameObject[] appleObjects;             // 활성화된 사과 배열
    [HideInInspector] public List<GameObject> selectedApples;       // 현재 선택된 사과들
    [HideInInspector] public List<GameObject> lastSelectedApples;   // 이전에 선택된 사과들

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;               // ScoreGroup의 Score Text
    public TextMeshProUGUI endScoreText;            // EndGroup의 End ScoreText
    public GameObject endGroup;                     // Canvas의 EndGroup
    public RectTransform appleImageRect;            // EndGroup의 Apple Image의 좌표
    public TextMeshProUGUI timeText;                // 남은 시간을 표시할 Text

    [Header("--------------[ Gaugebar ]")]
    public Slider timeSlider;                       // UI에 표시할 게이지바
    private const float TIME_LIMIT = 60f;           // 기본 시간 제한(초)
    private float currentTime;                      // 현재 시간

    [Header("--------------[ Effects ]")]
    public GameObject effectPrefab;                 // 이펙트 프리팹

    [Header("--------------[ ETC ]")]
    public SelectModeManager selectMode;            // SelectModeManager 참조
    private Camera mainCamera;                      // mainCamera 참조
    private readonly WaitForSeconds effectDestroyDelay = new WaitForSeconds(1f);

    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitializeGame();
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

    // 초기화
    private void InitializeGame()
    {
        mainCamera = Camera.main;
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        endGroup.SetActive(false);

        applePool = new List<Apple>(POOL_SIZE);
        selectedApples = new List<GameObject>(POOL_SIZE);
        lastSelectedApples = new List<GameObject>(POOL_SIZE);

        for (int i = 0; i < POOL_SIZE; i++)
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
        timeSlider.maxValue = TIME_LIMIT;
        currentTime = TIME_LIMIT;
        UpdateTimeUI();
    }

    // 새로운 사과 오브젝트 생성 함수
    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);

        if (newApple.TryGetComponent<Apple>(out Apple appleComponent))
        {
            applePool.Add(appleComponent);
        }
    }

    // 선택된 사과들의 합 계산 함수
    public void CalculateApples()
    {
        int totalAppleNum = 0;

        int count = selectedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (selectedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                totalAppleNum += appleComponent.appleNum;
            }
        }

        if (totalAppleNum == 10)
        {
            SpawnEffectsOnSelectedApples();
            DropSelectedApples();
            AddScore();
            UpdateScore();
        }
    }

    // 선택된 사과의 위치에 이펙트 생성 함수
    private void SpawnEffectsOnSelectedApples()
    {
        int count = selectedApples.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 effectPosition = selectedApples[i].transform.position;
            GameObject effect = Instantiate(effectPrefab, effectPosition, Quaternion.identity);

            if (effect.TryGetComponent<ParticleSystem>(out ParticleSystem particleSystem))
            {
                particleSystem.Play();
                StartCoroutine(DestroyEffectAfterDelay(effect));
            }
        }
    }

    // 생성된 이펙트를 일정 시간 후 제거하는 코루틴
    private IEnumerator DestroyEffectAfterDelay(GameObject effect)
    {
        yield return effectDestroyDelay;
        Destroy(effect);
    }

    // 선택된 사과를 떨어뜨리는 함수
    private void DropSelectedApples()
    {
        int count = selectedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (selectedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                appleComponent.DropApple();
            }
        }
    }

    // 선택된 사과들 초기화
    public void ClearSelectedApples()
    {
        int count = selectedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (selectedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                appleComponent.ShowApple();
            }
        }
        selectedApples.Clear();
    }

    // 이전에 선택된 사과들의 숫자 색상을 초기화하는 함수
    public void ClearLastSelectedApples()
    {
        int count = lastSelectedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (lastSelectedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                appleComponent.ResetNumberColor();
            }
        }
        lastSelectedApples.Clear();
    }

    // 드래그중 선택된 사과들의 숫자 색상을 변경하는 함수
    public void ChangeSelectedApplesNumberColor(Color color)
    {
        ClearLastSelectedApples();

        int count = selectedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (selectedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                appleComponent.ChangeNumberColor(color);
                lastSelectedApples.Add(selectedApples[i]);
            }
        }
    }

    // 점수 추가 함수
    private void AddScore()
    {
        int additionalScore = 0;
        int count = lastSelectedApples.Count;

        if (count >= TARGET_COUNT)
        {
            additionalScore = (count - (TARGET_COUNT - 1)) * 5;
        }
        score += (count * appleScore) + additionalScore;
    }

    // 점수 업데이트 함수
    private void UpdateScore()
    {
        scoreText.text = $"점수: {score}";
    }

    // 시간 업데이트 함수
    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
        timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // 게임오버 함수
    private void GameOver()
    {
        // 최고점수 계산
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        selectMode.EndDrag();
        ClearLastSelectedApples();

        endScoreText.text = $"최고점수: {highScore}\n점수: {score}";
        Vector2 startPosition = new Vector2(appleImageRect.anchoredPosition.x, 1050);
        appleImageRect.anchoredPosition = startPosition;

        endGroup.SetActive(true);

        float targetY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0)).y;
        appleImageRect.DOMoveY(targetY, 1f).SetEase(Ease.OutBounce);
    }

}
