using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                          // ���� ���� ����
    private int appleScore = 10;                    // �⺻ ��� ����
    [HideInInspector] public bool isGameOver;       // ���� ���� ���� ����

    [Header("--------------[ Game Setting ]")]
    public GameObject applePrefab;                  // ��� Prefab
    public Transform appleGroup;                    // ������Ʈ Ǯ���� ������ ��� �θ� ������Ʈ
    private List<Apple> applePool;                  // ��� ������Ʈ Ǯ
    private int poolSize = 120;                     // ������Ʈ Ǯ Size
    private int targetCount = 3;                    // ���ʽ� ������ ���� ��� ��ü ī��Ʈ

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
    private float timeLimit = 60f;                  // �⺻ �ð� ����(��)
    private float currentTime;                      // ���� �ð�

    [Header("--------------[ Effects ]")]
    public GameObject effectPrefab;                 // ����Ʈ ������

    [Header("--------------[ ETC ]")]
    public SelectModeManager selectMode;            // SelectModeManager ����
    private Camera mainCamera;                      // mainCamera ����

    private void Awake()
    {
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

        // appleObjects �迭 �ʱ�ȭ �� applePool�� ������ ����
        appleObjects = new GameObject[applePool.Count];
        for (int i = 0; i < applePool.Count; i++)
        {
            appleObjects[i] = applePool[i].gameObject;
        }

        // �������� �ʱ�ȭ
        timeSlider.maxValue = timeLimit;
        currentTime = timeLimit;
        UpdateTimeUI();
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

    // ���õ� ������� �� ���
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
            DropSelectedApples();
            AddScore();
            UpdateScore();
        }
    }

    // ���õ� ����� ��ġ�� ����Ʈ ����
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

    // ���õ� ����� ����߸���
    private void DropSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            appleComponent.DropApple();
        }
    }

    // ���õ� ����� �ʱ�ȭ
    public void ClearSelectedApples()
    {
        foreach (GameObject apple in selectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            appleComponent.ShowApple();
        }
        selectedApples.Clear();
    }

    // ������ ���õ� ������� ���� ������ �ʱ�ȭ
    public void ClearLastSelectedApples()
    {
        foreach (GameObject apple in lastSelectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            appleComponent.ResetNumberColor();
        }
        lastSelectedApples.Clear();
    }

    // �巡���� ���õ� ������� ���� ������ ����
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

        if (lastSelectedApples.Count >= targetCount)
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
        selectMode.EndDrag();
        ClearLastSelectedApples();

        endScoreText.text = "Score: " + scoreText.text;

        Vector2 startPosition = new Vector2(appleImageRect.anchoredPosition.x, 1050);
        appleImageRect.anchoredPosition = startPosition;

        endGroup.SetActive(true);

        float targetY = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 0)).y;
        appleImageRect.DOMoveY(targetY, 1f).SetEase(Ease.OutBounce);
    }

}
