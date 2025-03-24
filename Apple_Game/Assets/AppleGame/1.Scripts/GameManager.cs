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
    private int score = 0;                          // ���� ���� ����
    private int highScore = 0;                      // �ְ� ����
    private int appleScore = 10;                    // �⺻ ��� ����
    private int destroyedApples = 0;                // �Ͷ߸� ��� ����
    [HideInInspector] public bool isGameOver;       // ���� ���� ���� ����
    [HideInInspector] public bool isCountingDown;   // ī��Ʈ�ٿ� ���� �� ����
    private int remainingResets = 2;                // ���� ���� �缳�� Ƚ��

    [Header("--------------[ Game Setting ]")]
    public GameObject applePrefab;                  // ��� Prefab
    public Transform appleGroup;                    // ������Ʈ Ǯ���� ������ ��� �θ� ������Ʈ
    private List<Apple> applePool;                  // ��� ������Ʈ Ǯ
    private const int POOL_SIZE = 120;              // ������Ʈ Ǯ Size
    private const int TARGET_COUNT = 3;             // ���ʽ� ������ ���� ��� ��ü ī��Ʈ

    [HideInInspector] public GameObject[] appleObjects;             // Ȱ��ȭ�� ��� �迭
    [HideInInspector] public List<GameObject> selectedApples;       // ���� ���õ� �����
    [HideInInspector] public List<GameObject> lastSelectedApples;   // ������ ���õ� �����

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;               // ScoreGroup�� Score Text
    public TextMeshProUGUI endScoreText;            // EndGroup�� End ScoreText
    public GameObject endGroup;                     // Canvas�� EndGroup
    public RectTransform appleImageRect;            // EndGroup�� Apple Image�� ��ǥ
    public TextMeshProUGUI timeText;                // ���� �ð��� ǥ���� Text
    public TextMeshProUGUI destroyedAppleCountText; // �Ͷ߸� ����� ���� Text

    public GameObject setNumberPanel;               // Canvas�� SetNumberPanel
    public TextMeshProUGUI countDownText;           // SetNumberPanel�� ī��Ʈ�ٿ� Text
    private int countDown = 3;                      // ī��Ʈ�ٿ�

    [Header("--------------[ Gaugebar ]")]
    public Slider timeSlider;                       // UI�� ǥ���� ��������
    private const float TIME_LIMIT = 60f;           // �⺻ �ð� ����(��)
    private float currentTime;                      // ���� �ð�

    [Header("--------------[ Effects ]")]
    public GameObject effectPrefab;                 // ����Ʈ ������

    [Header("--------------[ ETC ]")]
    public DragManager dragManager;                 // DragManager ����
    private Camera mainCamera;                      // mainCamera ����
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

    // �ʱ�ȭ
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

    // ���ο� ��� ������Ʈ ���� �Լ�
    public void MakeApple()
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
            // ���� ����� ���� lastSelectedApples ����ȭ
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
            // ���� �ʱ�ȭ
            ClearSelectedApples();
        }
    }

    // ��Ī�� ����� ó���� ���� �ڷ�ƾ
    private IEnumerator ProcessMatchedApples(List<GameObject> matchedApples)
    {
        // ������� ���
        int count = matchedApples.Count;
        for (int i = 0; i < count; i++)
        {
            if (matchedApples[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                appleComponent.DropApple();
            }
        }

        // ��� �ִϸ��̼��� �Ϸ�� ������ ��� (2��)
        yield return new WaitForSeconds(2f);

        // �巡�� ���� ���� ó�� (���� ó��)
        if (dragManager.isDrag)
        {
            // ���� �巡�� ���� ������� ������ �ӽ� ����
            List<GameObject> tempSelectedApples = new List<GameObject>(selectedApples);

            // �巡�װ� ���� ������ ���
            yield return new WaitUntil(() => !dragManager.isDrag);

            // �����ص� ������� ���� 10���� Ȯ��
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

                // ���� 10�̸� ���� �ڷ�ƾ ���� (���ο� ��ġ�� �߻��� ���̹Ƿ�)
                if (sum == 10)
                {
                    yield break;
                }
            }
        }

        // �ش� ��� �ִϸ��̼� ���� �ٸ� ��� ��� �ִϸ��̼��� �Ϸ�Ǿ����� Ȯ�� (���� ó��)
        for (int i = 0; i < applePool.Count; i++)
        {
            if (applePool[i].isDropping)
            {
                yield break;
            }
        }

        // ���� ���� Ȯ�� (���� ó��)
        if (isGameOver) yield break;

        // ������ ���� ���
        if (CalculatePossibleCombinations() <= 5 && !setNumberPanel.activeSelf && remainingResets > 0)
        {
            remainingResets--;
            StartCoroutine(StartCountdownAndRandomize());
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
        yield return effectDestroyDelay;
        Destroy(effect);
    }

    // ���õ� ����� �ʱ�ȭ
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

    // �巡���� ���õ� ������� ���� ������ �����ϴ� �Լ�
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
            SoundManager.Instance.PlaySFX();
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
        ClearLastSelectedApples();

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
    }

    // =======================================================================================================

    // ������ �� 10 ���� ���
    private int CalculatePossibleCombinations()
    {
        const int GRID_WIDTH = 15;
        const int GRID_HEIGHT = 8;

        // 2���� �迭�� ��� �׸��� ����
        int[,] appleGrid = new int[GRID_HEIGHT, GRID_WIDTH];

        // �׸��忡 ��� ���� ä���
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

        // ��� ������ ���簢�� ���� �˻�
        for (int startRow = 0; startRow < GRID_HEIGHT; startRow++)
        {
            for (int startCol = 0; startCol < GRID_WIDTH; startCol++)
            {
                // ���簢���� ���̿� �ʺ� �����ϸ� �˻� (�ִ� 4x4 ũ�����)
                for (int height = 1; height <= Mathf.Min(4, GRID_HEIGHT - startRow); height++)
                {
                    for (int width = 1; width <= Mathf.Min(4, GRID_WIDTH - startCol); width++)
                    {
                        int sum = 0;
                        int validNumbers = 0;
                        List<Vector2Int> positions = new List<Vector2Int>();
                        List<int> numbers = new List<int>();

                        // ���� ���簢�� ���� ���� ���ڵ� Ȯ��
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

                        // ���� 10�̰� 2�� �̻��� ���ڰ� ���� ���
                        if (sum == 10 && validNumbers >= 2)
                        {
                            possibleCombinations++;
                            string positionStr = string.Join(", ", positions.Select(p => $"({p.x},{p.y})"));
                            string numberStr = string.Join(" + ", numbers);
                            //Debug.Log($"���� {possibleCombinations}: {numberStr} = 10 at positions {positionStr}");
                        }
                    }
                }
            }
        }

        //Debug.Log($"���� ���� ���� : {possibleCombinations}");
        return possibleCombinations;
    }

    // ��� ���� ���� ����
    private void RandomizeAppleNumbers()
    {
        int count = appleObjects.Length;
        for (int i = 0; i < count; i++)
        {
            if (appleObjects[i].TryGetComponent<Apple>(out Apple appleComponent))
            {
                // ���� �����ִ� ������� ���� �缳��
                if (appleComponent.appleNum > 0)
                {
                    appleComponent.SetRandomNumber();
                }
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
