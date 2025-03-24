using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                          // 현재 게임 점수
    private int highScore = 0;                      // 최고 점수
    private int appleScore = 10;                    // 기본 사과 점수
    private int destroyedApples = 0;                // 터뜨린 사과 개수
    [HideInInspector] public bool isGameOver;       // 게임 종료 상태 여부
    [HideInInspector] public bool isCountingDown;   // 카운트다운 진행 중 여부
    private int remainingResets = 2;                // 남은 숫자 재설정 횟수

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
    public TextMeshProUGUI destroyedAppleCountText; // 터뜨린 사과의 개수 Text

    public GameObject setNumberPanel;               // Canvas의 SetNumberPanel
    public TextMeshProUGUI countDownText;           // SetNumberPanel의 카운트다운 Text
    private int countDown = 3;                      // 카운트다운

    [Header("--------------[ Gaugebar ]")]
    public Slider timeSlider;                       // UI에 표시할 게이지바
    private const float TIME_LIMIT = 60f;           // 기본 시간 제한(초)
    private float currentTime;                      // 현재 시간

    [Header("--------------[ Effects ]")]
    public GameObject effectPrefab;                 // 이펙트 프리팹

    [Header("--------------[ ETC ]")]
    public DragManager dragManager;                 // DragManager 참조
    private Camera mainCamera;                      // mainCamera 참조
    private readonly WaitForSeconds effectDestroyDelay = new WaitForSeconds(1f);

    // =======================================================================================================

    private void Awake()
    {
        Application.targetFrameRate = 60;

        InitializeGame();
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

    // =======================================================================================================

    // 초기화
    private void InitializeGame()
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
        if (selectedApples.Count == 0) return;

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
            // 점수 계산을 위해 lastSelectedApples 동기화
            lastSelectedApples.Clear();
            for (int i = 0; i < selectedApples.Count; i++)
            {
                lastSelectedApples.Add(selectedApples[i]);
            }

            AddScore();
            SpawnEffectsOnSelectedApples();
            UpdateScore();
            StartCoroutine(ProcessMatchedApples(selectedApples));
            selectedApples.Clear();
        }
        else
        {
            // 선택 초기화
            ClearSelectedApples();
        }
    }

    // 매칭된 사과들 처리를 위한 코루틴
    private IEnumerator ProcessMatchedApples(List<GameObject> matchedApples)
    {
        // 사과들을 드롭
        int count = matchedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (matchedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                appleComponent.DropApple();
            }
        }

        // 드롭 애니메이션이 완료될 때까지 대기 (2초)
        yield return new WaitForSeconds(2f);

        // 드래그 중일 때의 처리 (예외 처리)
        if (dragManager.isDrag)
        {
            // 현재 드래그 중인 사과들의 정보를 임시 저장
            List<GameObject> tempSelectedApples = new List<GameObject>(selectedApples);

            // 드래그가 끝날 때까지 대기
            yield return new WaitUntil(() => !dragManager.isDrag);

            // 저장해둔 사과들의 합이 10인지 확인
            if (tempSelectedApples.Count > 0)
            {
                int sum = 0;
                for (int i = 0; i < tempSelectedApples.Count; i++)
                {
                    if (tempSelectedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
                    {
                        sum += appleComponent.appleNum;
                    }
                }

                // 합이 10이면 현재 코루틴 종료 (새로운 매치가 발생할 것이므로)
                if (sum == 10)
                {
                    yield break;
                }
            }
        }

        // 해당 드롭 애니메이션 제외 다른 모든 드롭 애니메이션이 완료되었는지 확인 (예외 처리)
        for (int i = 0; i < applePool.Count; i++)
        {
            if (applePool[i].isDropping)
            {
                yield break;
            }
        }

        // 게임 종료 확인 (예외 처리)
        if (isGameOver) yield break;

        // 가능한 조합 계산
        if (CalculatePossibleCombinations() <= 5 && !setNumberPanel.activeSelf && remainingResets > 0)
        {
            remainingResets--;
            StartCoroutine(StartCountdownAndRandomize());
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

    // =======================================================================================================

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
            SoundManager.Instance.PlaySFX();
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
        ClearLastSelectedApples();

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
    }

    // =======================================================================================================

    // 가능한 합 10 조합 계산
    private int CalculatePossibleCombinations()
    {
        const int GRID_WIDTH = 15;
        const int GRID_HEIGHT = 8;

        // 2차원 배열로 사과 그리드 생성
        int[,] appleGrid = new int[GRID_HEIGHT, GRID_WIDTH];

        // 그리드에 사과 숫자 채우기
        for (int i = 0; i < POOL_SIZE; i++)
        {
            if (appleObjects[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                int row = i / GRID_WIDTH;
                int col = i % GRID_WIDTH;
                appleGrid[row, col] = appleComponent.appleNum;
            }
        }

        int possibleCombinations = 0;

        // 모든 가능한 직사각형 영역 검사
        for (int startRow = 0; startRow < GRID_HEIGHT; startRow++)
        {
            for (int startCol = 0; startCol < GRID_WIDTH; startCol++)
            {
                // 직사각형의 높이와 너비를 변경하며 검사 (최대 4x4 크기까지)
                for (int height = 1; height <= Mathf.Min(4, GRID_HEIGHT - startRow); height++)
                {
                    for (int width = 1; width <= Mathf.Min(4, GRID_WIDTH - startCol); width++)
                    {
                        int sum = 0;
                        int validNumbers = 0;
                        List<Vector2Int> positions = new List<Vector2Int>();
                        List<int> numbers = new List<int>();

                        // 현재 직사각형 영역 내의 숫자들 확인
                        for (int r = startRow; r < startRow + height; r++)
                        {
                            for (int c = startCol; c < startCol + width; c++)
                            {
                                if (appleGrid[r, c] > 0)
                                {
                                    sum += appleGrid[r, c];
                                    validNumbers++;
                                    positions.Add(new Vector2Int(r, c));
                                    numbers.Add(appleGrid[r, c]);
                                }
                            }
                        }

                        // 합이 10이고 2개 이상의 숫자가 사용된 경우
                        if (sum == 10 && validNumbers >= 2)
                        {
                            possibleCombinations++;
                            string positionStr = string.Join(", ", positions.Select(p => $"({p.x},{p.y})"));
                            string numberStr = string.Join(" + ", numbers);
                            //Debug.Log($"조합 {possibleCombinations}: {numberStr} = 10 at positions {positionStr}");
                        }
                    }
                }
            }
        }

        //Debug.Log($"조합 가능 개수 : {possibleCombinations}");
        return possibleCombinations;
    }

    // 사과 숫자 랜덤 변경
    private void RandomizeAppleNumbers()
    {
        int count = appleObjects.Length;
        for (int i = 0; i < count; i++)
        {
            if (appleObjects[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                // 아직 남아있는 사과에만 숫자 재설정
                if (appleComponent.appleNum > 0)
                {
                    appleComponent.SetRandomNumber();
                }
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
        ClearLastSelectedApples();

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

    // =======================================================================================================



}
