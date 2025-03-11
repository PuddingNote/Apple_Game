using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                          // ���� ���� ����
    private int highScore = 0;                      // �ְ� ����
    private int appleScore = 10;                    // �⺻ ��� ����
    [HideInInspector] public bool isGameOver;       // ���� ���� ���� ����

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

    [Header("--------------[ Gaugebar ]")]
    public Slider timeSlider;                       // UI�� ǥ���� ��������
    private const float TIME_LIMIT = 60f;           // �⺻ �ð� ����(��)
    private float currentTime;                      // ���� �ð�

    [Header("--------------[ Effects ]")]
    public GameObject effectPrefab;                 // ����Ʈ ������

    [Header("--------------[ ETC ]")]
    public SelectModeManager selectMode;            // SelectModeManager ����
    private Camera mainCamera;                      // mainCamera ����
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

    // �ʱ�ȭ
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

    // ���õ� ����� ����߸��� �Լ�
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

    // ���� �߰� �Լ�
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

    // ���� ������Ʈ �Լ�
    private void UpdateScore()
    {
        scoreText.text = $"����: {score}";
    }

    // �ð� ������Ʈ �Լ�
    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
        timeText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // ���ӿ��� �Լ�
    private void GameOver()
    {
        // �ְ����� ���
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        selectMode.EndDrag();
        ClearLastSelectedApples();

        endScoreText.text = $"�ְ�����: {highScore}\n����: {score}";
        Vector2 startPosition = new Vector2(appleImageRect.anchoredPosition.x, 1050);
        appleImageRect.anchoredPosition = startPosition;

        endGroup.SetActive(true);

        float targetY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0)).y;
        appleImageRect.DOMoveY(targetY, 1f).SetEase(Ease.OutBounce);
    }

}
