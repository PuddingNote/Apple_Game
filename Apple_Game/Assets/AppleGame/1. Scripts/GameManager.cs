using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]
    private int score = 0;                      // ����
    private int appleScore = 10;                // ��� �ϳ��� ����
    [HideInInspector]public bool isGameOver;    // GameOver �Ǵ�


    [Header("--------------[ Game Setting ]")]
    public GameObject applePrefab;              // ��� Prefab
    public Transform appleGroup;                // ������Ʈ Ǯ���� ������ appleGroup
    public List<Apple> applePool;               // ��� ������Ʈ Ǯ�� ����Ʈ
    [Range(0, 120)]
    public int poolSize;                        // ������Ʈ Ǯ�� Size

    public GameObject[] appleObjects;           // ���� �ִ� ��� ��� ��ü �迭
    public List<GameObject> selectedApples;     // ���õ� ��� ��ü ����Ʈ
    public List<GameObject> lastSelectedApples; // ������ ���õ� ������� ������ ����Ʈ


    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;           // ScoreGroup�� Score Text
    public TextMeshProUGUI endScoreText;        // EndGroup�� End ScoreText
    public GameObject endGroup;                 // Canvas�� EndGroup
    
    [Header("--------------[ Gaugebar ]")]
    public float timeLimit = 30f;               // �⺻ �ð� ����(��)
    private float currentTime;                  // ���� �ð�
    public Slider timeSlider;                   // UI�� ǥ���� ������ ��


    private void Awake()
    {
        Application.targetFrameRate = 60;       // ������ ����

        endGroup.SetActive(false);

        applePool = new List<Apple>();
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

        // ������ �� �ʱ�ȭ
        timeSlider.maxValue = timeLimit;
        currentTime = timeLimit;
        UpdateTimeUI();
    }


    private void Update()
    {
        if (!isGameOver)
        {
            currentTime -= Time.deltaTime;          // �ð� ����
            UpdateTimeUI();                         // UI ������Ʈ

            // �ð� ���� üũ
            if (currentTime <= 0f)
            {
                isGameOver = true;
                GameOver();
            }
        }
    }

    // �ʱ� ��� ���� �޼���
    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);
        Apple appleComponent = newApple.GetComponent<Apple>();

        if (appleComponent != null)
        {
            applePool.Add(appleComponent);
        }
    }

    // ���õ� ����� �ʱ�ȭ �޼���
    public void ClearSelectedApples()
    {
        foreach (GameObject apple in selectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ShowApple();         // ��� ��ü�� �ٽ� ���̰� ��
            }
        }
        selectedApples.Clear();     // ���õ� ��ü ����Ʈ �ʱ�ȭ
    }

    // ��� �巡�� �޼���
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {   
        // �巡�� ���� �� ��� ��ü ���� �κ�
        List<GameObject> currentlySelected = new List<GameObject>();    // ���� �巡�� ���� ���� �ִ� ����� �ӽ÷� ����
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

        // �巡�� ���� �� ��� ��ü ���� ���� �κ�
        List<GameObject> applesToDeselect = new List<GameObject>();     // �巡�� �������� ��� ��� ��ü���� �ӽ÷� ����
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
                appleComponent.ShowApple();                 // ��� ��ü�� �ٽ� ���̰� ��
            }
        }
    }

    // ������ ��� ��ü���� ������ �� ��� �� ó�� �޼���
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

    // ���� 10�� ������� ����� �޼���
    private void HideSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.HideApple();                 // ��� ��ü �����ϰ� �����
            }
        }
        //selectedApples.Clear(); // ����Ʈ �ʱ�ȭ
    }

    // ������ ���õ� ������� ���� ������ �ʱ�ȭ�ϴ� �޼���
    public void ClearLastSelectedApples()
    {
        foreach (GameObject apple in lastSelectedApples)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ResetNumberColor();          // ��� ���� ������ ������� ����
            }
        }
        lastSelectedApples.Clear();
    }

    // �巡�� �߿� ���õ� ������� ���� ������ �����ϴ� �޼���
    public void ChangeSelectedApplesNumberColor(Color color)
    {
        ClearLastSelectedApples();

        foreach (GameObject selectedApple in selectedApples)
        {
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.ChangeNumberColor(color);    // ��� ���� ������ ����
                lastSelectedApples.Add(selectedApple);      // ������ ���õ� ����� ����Ʈ�� �߰�
            }
        }
    }

    // ���õ� ��� ��ü���� ��ȯ�ϴ� �޼���
    public List<GameObject> GetSelectedApples()
    {
        return selectedApples;
    }

    // ���� �߰� �޼���
    private void AddScore()
    {
        score += lastSelectedApples.Count * appleScore;
    }

    // ���� ������Ʈ �޼���
    private void UpdateScore()
    {
        scoreText.text = score.ToString();
    }
    
    // ����ð��� �������ٿ� �ݿ��ϴ� �޼���
    private void UpdateTimeUI()
    {
        timeSlider.value = currentTime;
    }

    // �������� �޼���
    private void GameOver()
    {
        scoreText.enabled = false;
        endScoreText.text = "Score: " + scoreText.text;
        endGroup.SetActive(true);
    }

}
