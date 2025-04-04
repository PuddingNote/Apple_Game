using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{

    #region Variables

    [Header("--------------[ Game Control ]")]
    private int score = 0;                              // 현재 게임 점수
    private int highScore = 0;                          // 최고 점수
    private int appleScore = 10;                        // 기본 사과 점수
    private int destroyedApples = 0;                    // 터뜨린 사과 개수
    [HideInInspector] public bool isGameOver;           // 게임 종료 상태 여부
    [HideInInspector] public bool isCountingDown;       // 카운트다운 진행 중 여부
    private int remainingResets = 2;                    // 남은 숫자 재설정 횟수

    [Header("--------------[ Game Setting ]")]
    [SerializeField] private GameObject applePrefab;    // 사과 프리펩
    [SerializeField] private Transform appleGroup;      // 오브젝트 풀링을 설정할 사과 부모 오브젝트
    private List<Apple> applePool;                      // 사과 오브젝트 풀 List
    private const int POOL_SIZE = 120;                  // 오브젝트 풀 최대 크기
    private const int GRID_WIDTH = 15;                  // 게임 그리드 가로 사이즈
    private const int GRID_HEIGHT = 8;                  // 게임 그리드 세로 사이즈
    private const int TARGET_COUNT = 3;                 // 보너스 점수를 위한 최소 사과 개수

    [HideInInspector] public GameObject[] appleObjects;                     // 활성화된 사과 오브젝트 배열
    [HideInInspector] public List<GameObject> selectedApples;               // 현재 선택된 사과 오브젝트 List
    [HideInInspector] public List<GameObject> lastSelectedApples;           // 이전에 선택된 사과 오브젝트 List

    private bool isProcessingMatch = false;                                 // 현재 매치 처리 중인지 여부를 나타내는 플래그 (중복 실행 방지용)
    private Coroutine currentMatchProcess = null;                           // 현재 실행 중인 매치 처리 코루틴의 참조 (취소 가능하도록 저장)

    private HashSet<string> positionCombinations = new HashSet<string>();   // 유효한 사과 조합의 위치 정보를 저장하는 HashSet
    private List<Vector2Int> tempPositions = new List<Vector2Int>(50);      // 조합 계산 시 임시로 사용하는 위치 List
    private List<int> tempNumbers = new List<int>(50);                      // 조합 계산 시 임시로 사용하는 숫자 List

    [Header("--------------[ UI References ]")]
    [SerializeField] private TextMeshProUGUI scoreText;                     // 현재 점수를 표시하는 Text (ScoreGroup의 Score Text)
    [SerializeField] private TextMeshProUGUI endScoreText;                  // 게임 종료 시 점수를 표시하는 Text (EndGroup의 End ScoreText)
    [SerializeField] private GameObject endGroup;                           // 게임 종료 UI 그룹 (Canvas의 EndGroup)
    [SerializeField] private RectTransform appleImageRect;                  // 게임 종료 시 떨어지는 사과 이미지의 RectTransform (EndGroup의 Apple Image의 좌표)
    [SerializeField] private TextMeshProUGUI timeText;                      // 남은 게임 시간을 표시하는 Text
    [SerializeField] private TextMeshProUGUI destroyedAppleCountText;       // 터뜨린 사과 개수를 표시하는 Text

    [SerializeField] private GameObject setNumberPanel;                     // 숫자 재설정 시 표시되는 패널 (Canvas의 SetNumberPanel)
    [SerializeField] private TextMeshProUGUI countDownText;                 // 숫자 재설정 카운트다운 Text
    private int countDown = 3;                                              // 숫자 재설정 카운트다운 초기값

    [Header("--------------[ Gaugebar ]")]
    [SerializeField] private Slider timeSlider;         // 남은 시간을 표시하는 게이지바
    private const float TIME_LIMIT = 60f;               // 게임 제한 시간(초)
    private float currentTime;                          // 현재 남은 시간

    [Header("--------------[ Effects ]")]
    [SerializeField] private GameObject effectPrefab;   // 사과 매칭 시 생성되는 이펙트 프리팹

    [Header("--------------[ ETC ]")]
    [SerializeField] private DragManager dragManager;   // DragManager 참조
    private Camera mainCamera;                          // mainCamera 참조

    #endregion


    #region Unity Methods

    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitializeGameManager();
    }

    private void Start()
    {
        CalculatePossibleCombinations();
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

    #endregion


    #region Initialize

    // 게임 초기 설정 함수
    private void InitializeGameManager()
    {
        mainCamera = Camera.main;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        remainingResets = 2;
        destroyedApples = 0;
        UpdateAppleCount();

        endGroup.SetActive(false);
        setNumberPanel.SetActive(false);

        applePool = new List<Apple>(POOL_SIZE);
        selectedApples = new List<GameObject>(POOL_SIZE);
        lastSelectedApples = new List<GameObject>(POOL_SIZE);

        for (int i = 0; i < POOL_SIZE; i++)
        {
            MakeApples();
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

    #endregion


    #region Apple Selection & Matching

    // 새로운 사과 오브젝트 생성 함수
    private void MakeApples()
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
        if (selectedApples.Count <= 1) return;

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
            // 이미 매치 처리가 진행 중인 경우, 기존 처리를 중단하고 새 매치로 교체
            if (isProcessingMatch && currentMatchProcess != null)
            {
                //Debug.Log("기존 매치 처리 중단, 새 매치로 전환");
                StopCoroutine(currentMatchProcess);
                isProcessingMatch = false;
            }

            SpawnEffectsOnSelectedApples();
            AddScore();
            UpdateScore();

            isProcessingMatch = true;
            currentMatchProcess = StartCoroutine(ProcessMatchedApples(lastSelectedApples));
            selectedApples.Clear();
        }
        else
        {
            ClearSelectedApples();
        }
    }

    // 매칭된 사과들 처리를 위한 코루틴
    private IEnumerator ProcessMatchedApples(List<GameObject> matchedApples)
    {
        try
        {
            // 사과들을 드롭
            int count = matchedApples.Count;
            Apple[] appleComponents = new Apple[count];

            for (int i = 0; i < count; i++)
            {
                appleComponents[i] = matchedApples[i].GetComponent<Apple>();
                appleComponents[i].DropApple();
            }

            // 드롭 애니메이션이 완료될 때까지 대기 (2초)
            yield return new WaitForSeconds(2f);

            // 게임 종료 확인
            if (isGameOver)
            {
                yield break;
            }

            // 드래그 중일 때 완료될 때까지 대기 (가능한 조합 계산 최소화를 위해)
            if (dragManager.isDrag)
            {
                yield return new WaitUntil(() => !dragManager.isDrag);
            }

            // 가능한 조합 계산
            int possibleCombinations = CalculatePossibleCombinations();
            if (possibleCombinations <= 3 && !setNumberPanel.activeSelf && remainingResets > 0)
            {
                remainingResets--;
                StartCoroutine(StartCountdownAndRandomize());
            }
        }
        finally
        {
            isProcessingMatch = false;
            currentMatchProcess = null;
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
        yield return new WaitForSeconds(1f);
        Destroy(effect);
    }

    // 선택된 사과들 초기화 함수
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
    private void ClearLastSelectedApplesColor()
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
        ClearLastSelectedApplesColor();

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

    #endregion


    #region Score & UI

    // 점수 추가 함수
    private void AddScore()
    {
        int count = lastSelectedApples.Count;
        if (count == 0) return;

        destroyedApples += count;
        UpdateAppleCount();

        int baseScore = count * appleScore;

        // 보너스 점수 계산 (3개 이상 선택 시)
        int bonusScore = 0;
        if (count >= TARGET_COUNT)
        {
            bonusScore = (count - (TARGET_COUNT - 1)) * 5;
        }

        score += baseScore + bonusScore;

        // 매칭 효과음 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx();
        }

        // 최고 점수 갱신 확인 및 저장
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    // 점수 업데이트 함수
    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"점수: {score}";
        }
    }

    // 시간 업데이트 함수
    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
        timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // 사과 개수 업데이트 함수
    private void UpdateAppleCount()
    {
        if (destroyedAppleCountText != null)
        {
            destroyedAppleCountText.text = $"사과개수: {destroyedApples}";
        }
    }

    // 게임오버 함수
    private void GameOver()
    {
        dragManager.EndDrag();
        ClearLastSelectedApplesColor();

        // 최고 터뜨린 사과 개수 갱신 및 저장
        int highDestroyedAppleCount = PlayerPrefs.GetInt("HighDestroyedAppleCount", 0);
        if (destroyedApples > highDestroyedAppleCount)
        {
            PlayerPrefs.SetInt("HighDestroyedAppleCount", destroyedApples);
            PlayerPrefs.Save();
        }

        endScoreText.text = $"최고 점수: {highScore}\n현재 점수: {score}\n사과 개수: {destroyedApples}";
        Vector2 startPosition = new Vector2(appleImageRect.anchoredPosition.x, 1050);
        appleImageRect.anchoredPosition = startPosition;

        endGroup.SetActive(true);

        float targetY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0)).y;
        appleImageRect.DOMoveY(targetY, 1f).SetEase(Ease.OutBounce);

        //StartCoroutine(MoveWithBounceEffect(appleImageRect, targetY, 1f));
    }

    /*
    // 현재 사용중인 DoTween의 DOMoveY, SetEase와 동일한 효과
    private IEnumerator MoveWithBounceEffect(RectTransform rectTransform, float targetY, float duration)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float startY = startPosition.y;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;

            float yOffset = OutBounceEasing(normalizedTime);

            float currentY = Mathf.Lerp(startY, targetY, yOffset);
            rectTransform.anchoredPosition = new Vector2(startPosition.x, currentY);

            yield return null;
        }

        // 애니메이션 종료 시 정확한 위치로 설정
        rectTransform.anchoredPosition = new Vector2(startPosition.x, targetY);
    }

    private float OutBounceEasing(float t)
    {
        if (t < 1 / 2.75f)
        {
            return 7.5625f * t * t;
        }
        else if (t < 2 / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < 2.5f / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }
    */

    #endregion


    #region Apple Combination Calculation

    // 사과 조합 가능 수 계산 (중복 제외)
    private int CalculatePossibleCombinations()
    {
        positionCombinations.Clear();

        // 모든 가능한 직사각형 영역 검사
        for (int startRow = 0; startRow < GRID_HEIGHT; startRow++)
        {
            for (int startCol = 0; startCol < GRID_WIDTH; startCol++)
            {
                // 시작 위치의 사과가 유효하지 않으면 스킵
                Apple startApple = appleObjects[startRow * GRID_WIDTH + startCol].GetComponent<Apple>();
                if (startApple == null || startApple.appleNum <= 0)
                {
                    continue;
                }

                // 가능한 모든 직사각형 크기에 대해 조합 가능 수 계산
                for (int height = 1; height <= Math.Min(GRID_HEIGHT, GRID_HEIGHT - startRow); height++)
                {
                    for (int width = 1; width <= Math.Min(GRID_WIDTH, GRID_WIDTH - startCol); width++)
                    {
                        CheckRectangleArea(startRow, startCol, height, width, GRID_WIDTH);
                    }
                }
            }
        }

        int count = positionCombinations.Count;
        //Debug.Log($"조합 가능 개수: {count}");
        return count;
    }

    // 주어진 크기에 맞게 조합 가능 수 계산 함수
    private void CheckRectangleArea(int startRow, int startCol, int height, int width, int gridWidth)
    {
        tempPositions.Clear();
        tempNumbers.Clear();
        int sum = 0;
        bool hasValidApples = false;

        // 직사각형 영역 내의 모든 사과 수집
        for (int r = startRow; r < startRow + height; r++)
        {
            for (int c = startCol; c < startCol + width; c++)
            {
                Apple apple = appleObjects[r * gridWidth + c].GetComponent<Apple>();
                if (apple != null && apple.appleNum > 0)
                {
                    tempPositions.Add(new Vector2Int(r, c));
                    tempNumbers.Add(apple.appleNum);
                    sum += apple.appleNum;
                    hasValidApples = true;
                }
            }
        }

        // 유효한 사과가 2개 이상이고 합이 10인 경우만 처리
        if (hasValidApples && tempNumbers.Count >= 2 && sum == 10)
        {
            // 위치들을 정렬하여 일관된 키 생성
            tempPositions.Sort((a, b) => (a.x * gridWidth + a.y).CompareTo(b.x * gridWidth + b.y));

            // 중복 체크
            string positionKey = string.Join("|", tempPositions.Select(p => $"{p.x},{p.y}"));
            if (!positionCombinations.Contains(positionKey))
            {
                positionCombinations.Add(positionKey);
            }
        }
    }

    // 사과 숫자 랜덤 변경 함수
    private void RandomizeAppleNumbers()
    {
        Apple[] appleComponents = new Apple[appleObjects.Length];
        for (int i = 0; i < appleObjects.Length; i++)
        {
            appleComponents[i] = appleObjects[i].GetComponent<Apple>();
            if (appleComponents[i].appleNum > 0)
            {
                appleComponents[i].SetRandomNumber();
            }
        }
    }

    // 카운트다운 코루틴
    private IEnumerator StartCountdownAndRandomize()
    {
        isCountingDown = true;
        setNumberPanel.SetActive(true);

        // 현재 선택된 사과들 초기화
        ClearSelectedApples();
        ClearLastSelectedApplesColor();

        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        while (countDown > 0)
        {
            countDownText.text = countDown.ToString();
            yield return new WaitForSecondsRealtime(1f);
            countDown--;
        }

        setNumberPanel.SetActive(false);
        countDown = 3;
        RandomizeAppleNumbers();

        CalculatePossibleCombinations();

        Time.timeScale = previousTimeScale;
        isCountingDown = false;
    }

    #endregion


}
