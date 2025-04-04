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
    private int score = 0;                              // ���� ���� ����
    private int highScore = 0;                          // �ְ� ����
    private int appleScore = 10;                        // �⺻ ��� ����
    private int destroyedApples = 0;                    // �Ͷ߸� ��� ����
    [HideInInspector] public bool isGameOver;           // ���� ���� ���� ����
    [HideInInspector] public bool isCountingDown;       // ī��Ʈ�ٿ� ���� �� ����
    private int remainingResets = 2;                    // ���� ���� �缳�� Ƚ��

    [Header("--------------[ Game Setting ]")]
    [SerializeField] private GameObject applePrefab;    // ��� ������
    [SerializeField] private Transform appleGroup;      // ������Ʈ Ǯ���� ������ ��� �θ� ������Ʈ
    private List<Apple> applePool;                      // ��� ������Ʈ Ǯ List
    private const int POOL_SIZE = 120;                  // ������Ʈ Ǯ �ִ� ũ��
    private const int GRID_WIDTH = 15;                  // ���� �׸��� ���� ������
    private const int GRID_HEIGHT = 8;                  // ���� �׸��� ���� ������
    private const int TARGET_COUNT = 3;                 // ���ʽ� ������ ���� �ּ� ��� ����

    [HideInInspector] public GameObject[] appleObjects;                     // Ȱ��ȭ�� ��� ������Ʈ �迭
    [HideInInspector] public List<GameObject> selectedApples;               // ���� ���õ� ��� ������Ʈ List
    [HideInInspector] public List<GameObject> lastSelectedApples;           // ������ ���õ� ��� ������Ʈ List

    private bool isProcessingMatch = false;                                 // ���� ��ġ ó�� ������ ���θ� ��Ÿ���� �÷��� (�ߺ� ���� ������)
    private Coroutine currentMatchProcess = null;                           // ���� ���� ���� ��ġ ó�� �ڷ�ƾ�� ���� (��� �����ϵ��� ����)

    private HashSet<string> positionCombinations = new HashSet<string>();   // ��ȿ�� ��� ������ ��ġ ������ �����ϴ� HashSet
    private List<Vector2Int> tempPositions = new List<Vector2Int>(50);      // ���� ��� �� �ӽ÷� ����ϴ� ��ġ List
    private List<int> tempNumbers = new List<int>(50);                      // ���� ��� �� �ӽ÷� ����ϴ� ���� List

    [Header("--------------[ UI References ]")]
    [SerializeField] private TextMeshProUGUI scoreText;                     // ���� ������ ǥ���ϴ� Text (ScoreGroup�� Score Text)
    [SerializeField] private TextMeshProUGUI endScoreText;                  // ���� ���� �� ������ ǥ���ϴ� Text (EndGroup�� End ScoreText)
    [SerializeField] private GameObject endGroup;                           // ���� ���� UI �׷� (Canvas�� EndGroup)
    [SerializeField] private RectTransform appleImageRect;                  // ���� ���� �� �������� ��� �̹����� RectTransform (EndGroup�� Apple Image�� ��ǥ)
    [SerializeField] private TextMeshProUGUI timeText;                      // ���� ���� �ð��� ǥ���ϴ� Text
    [SerializeField] private TextMeshProUGUI destroyedAppleCountText;       // �Ͷ߸� ��� ������ ǥ���ϴ� Text

    [SerializeField] private GameObject setNumberPanel;                     // ���� �缳�� �� ǥ�õǴ� �г� (Canvas�� SetNumberPanel)
    [SerializeField] private TextMeshProUGUI countDownText;                 // ���� �缳�� ī��Ʈ�ٿ� Text
    private int countDown = 3;                                              // ���� �缳�� ī��Ʈ�ٿ� �ʱⰪ

    [Header("--------------[ Gaugebar ]")]
    [SerializeField] private Slider timeSlider;         // ���� �ð��� ǥ���ϴ� ��������
    private const float TIME_LIMIT = 60f;               // ���� ���� �ð�(��)
    private float currentTime;                          // ���� ���� �ð�

    [Header("--------------[ Effects ]")]
    [SerializeField] private GameObject effectPrefab;   // ��� ��Ī �� �����Ǵ� ����Ʈ ������

    [Header("--------------[ ETC ]")]
    [SerializeField] private DragManager dragManager;   // DragManager ����
    private Camera mainCamera;                          // mainCamera ����

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

    // ���� �ʱ� ���� �Լ�
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

        // appleObjects �迭 �ʱ�ȭ �� applePool�� ������ ����
        appleObjects = new GameObject[applePool.Count];
        for (int i = 0; i < applePool.Count; i++)
        {
            appleObjects[i] = applePool[i].gameObject;
        }

        // �������� �ʱ�ȭ
        timeSlider.maxValue = TIME_LIMIT;
        currentTime = TIME_LIMIT;
        UpdateTimeUI();
    }

    #endregion


    #region Apple Selection & Matching

    // ���ο� ��� ������Ʈ ���� �Լ�
    private void MakeApples()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);

        if (newApple.TryGetComponent<Apple>(out Apple appleComponent))
        {
            applePool.Add(appleComponent);
        }
    }

    // ���õ� ������� �� ��� �Լ�
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
            // �̹� ��ġ ó���� ���� ���� ���, ���� ó���� �ߴ��ϰ� �� ��ġ�� ��ü
            if (isProcessingMatch && currentMatchProcess != null)
            {
                //Debug.Log("���� ��ġ ó�� �ߴ�, �� ��ġ�� ��ȯ");
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

    // ��Ī�� ����� ó���� ���� �ڷ�ƾ
    private IEnumerator ProcessMatchedApples(List<GameObject> matchedApples)
    {
        try
        {
            // ������� ���
            int count = matchedApples.Count;
            Apple[] appleComponents = new Apple[count];

            for (int i = 0; i < count; i++)
            {
                appleComponents[i] = matchedApples[i].GetComponent<Apple>();
                appleComponents[i].DropApple();
            }

            // ��� �ִϸ��̼��� �Ϸ�� ������ ��� (2��)
            yield return new WaitForSeconds(2f);

            // ���� ���� Ȯ��
            if (isGameOver)
            {
                yield break;
            }

            // �巡�� ���� �� �Ϸ�� ������ ��� (������ ���� ��� �ּ�ȭ�� ����)
            if (dragManager.isDrag)
            {
                yield return new WaitUntil(() => !dragManager.isDrag);
            }

            // ������ ���� ���
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

    // ���õ� ����� ��ġ�� ����Ʈ ���� �Լ�
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

    // ������ ����Ʈ�� ���� �ð� �� �����ϴ� �ڷ�ƾ
    private IEnumerator DestroyEffectAfterDelay(GameObject effect)
    {
        yield return new WaitForSeconds(1f);
        Destroy(effect);
    }

    // ���õ� ����� �ʱ�ȭ �Լ�
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

    // ������ ���õ� ������� ���� ������ �ʱ�ȭ�ϴ� �Լ�
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

    // �巡���� ���õ� ������� ���� ������ �����ϴ� �Լ�
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

    // ���� �߰� �Լ�
    private void AddScore()
    {
        int count = lastSelectedApples.Count;
        if (count == 0) return;

        destroyedApples += count;
        UpdateAppleCount();

        int baseScore = count * appleScore;

        // ���ʽ� ���� ��� (3�� �̻� ���� ��)
        int bonusScore = 0;
        if (count >= TARGET_COUNT)
        {
            bonusScore = (count - (TARGET_COUNT - 1)) * 5;
        }

        score += baseScore + bonusScore;

        // ��Ī ȿ���� ���
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx();
        }

        // �ְ� ���� ���� Ȯ�� �� ����
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    // ���� ������Ʈ �Լ�
    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = $"����: {score}";
        }
    }

    // �ð� ������Ʈ �Լ�
    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
        timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // ��� ���� ������Ʈ �Լ�
    private void UpdateAppleCount()
    {
        if (destroyedAppleCountText != null)
        {
            destroyedAppleCountText.text = $"�������: {destroyedApples}";
        }
    }

    // ���ӿ��� �Լ�
    private void GameOver()
    {
        dragManager.EndDrag();
        ClearLastSelectedApplesColor();

        // �ְ� �Ͷ߸� ��� ���� ���� �� ����
        int highDestroyedAppleCount = PlayerPrefs.GetInt("HighDestroyedAppleCount", 0);
        if (destroyedApples > highDestroyedAppleCount)
        {
            PlayerPrefs.SetInt("HighDestroyedAppleCount", destroyedApples);
            PlayerPrefs.Save();
        }

        endScoreText.text = $"�ְ� ����: {highScore}\n���� ����: {score}\n��� ����: {destroyedApples}";
        Vector2 startPosition = new Vector2(appleImageRect.anchoredPosition.x, 1050);
        appleImageRect.anchoredPosition = startPosition;

        endGroup.SetActive(true);

        float targetY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0)).y;
        appleImageRect.DOMoveY(targetY, 1f).SetEase(Ease.OutBounce);

        //StartCoroutine(MoveWithBounceEffect(appleImageRect, targetY, 1f));
    }

    /*
    // ���� ������� DoTween�� DOMoveY, SetEase�� ������ ȿ��
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

        // �ִϸ��̼� ���� �� ��Ȯ�� ��ġ�� ����
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

    // ��� ���� ���� �� ��� (�ߺ� ����)
    private int CalculatePossibleCombinations()
    {
        positionCombinations.Clear();

        // ��� ������ ���簢�� ���� �˻�
        for (int startRow = 0; startRow < GRID_HEIGHT; startRow++)
        {
            for (int startCol = 0; startCol < GRID_WIDTH; startCol++)
            {
                // ���� ��ġ�� ����� ��ȿ���� ������ ��ŵ
                Apple startApple = appleObjects[startRow * GRID_WIDTH + startCol].GetComponent<Apple>();
                if (startApple == null || startApple.appleNum <= 0)
                {
                    continue;
                }

                // ������ ��� ���簢�� ũ�⿡ ���� ���� ���� �� ���
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
        //Debug.Log($"���� ���� ����: {count}");
        return count;
    }

    // �־��� ũ�⿡ �°� ���� ���� �� ��� �Լ�
    private void CheckRectangleArea(int startRow, int startCol, int height, int width, int gridWidth)
    {
        tempPositions.Clear();
        tempNumbers.Clear();
        int sum = 0;
        bool hasValidApples = false;

        // ���簢�� ���� ���� ��� ��� ����
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

        // ��ȿ�� ����� 2�� �̻��̰� ���� 10�� ��츸 ó��
        if (hasValidApples && tempNumbers.Count >= 2 && sum == 10)
        {
            // ��ġ���� �����Ͽ� �ϰ��� Ű ����
            tempPositions.Sort((a, b) => (a.x * gridWidth + a.y).CompareTo(b.x * gridWidth + b.y));

            // �ߺ� üũ
            string positionKey = string.Join("|", tempPositions.Select(p => $"{p.x},{p.y}"));
            if (!positionCombinations.Contains(positionKey))
            {
                positionCombinations.Add(positionKey);
            }
        }
    }

    // ��� ���� ���� ���� �Լ�
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

    // ī��Ʈ�ٿ� �ڷ�ƾ
    private IEnumerator StartCountdownAndRandomize()
    {
        isCountingDown = true;
        setNumberPanel.SetActive(true);

        // ���� ���õ� ����� �ʱ�ȭ
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
