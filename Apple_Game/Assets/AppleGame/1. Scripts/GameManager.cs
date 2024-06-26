using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Game Control ]")]


    [Header("--------------[ Main ]")]
    public int score;               // ����
    public bool isGameOver;         // GameOver �Ǵ�

    [Header("--------------[ Object Pooling ]")]
    public GameObject applePrefab;
    public Transform appleGroup;
    public List<Apple> applePool;

    [Range(0, 120)]
    public int poolSize;            // ������Ʈ Ǯ�� Size

    [Header("--------------[ Game Settings ]")]
    //public int horizontalLength;
    //public int verticalLength;

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;


    [Header("--------------[ TEST ]")]
    public GameObject[] appleObjects;           // ���� �ִ� ��� ��� ��ü �迭
    public List<GameObject> selectedApples;     // ���õ� ��� ��ü ����Ʈ


    private void Awake()
    {
        Application.targetFrameRate = 60;   // ������ ����
        applePool = new List<Apple>();
        selectedApples = new List<GameObject>();

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

    }

    
    private void Update()
    {
        //if (!isGameOver)
        //{
        //    UpdateScore();
        //}
    }

    // �ʱ� ��� ���� �޼���
    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);
        Apple appleComponent = newApple.GetComponent<Apple>();

        if (appleComponent != null)
        {
            //appleComponent.Initialize();
            applePool.Add(appleComponent);
        }
    }

    // ���õ� ����� �ʱ�ȭ �޼���
    public void ClearSelectedApples()
    {
        selectedApples.Clear();  // ���õ� ��ü ����Ʈ �ʱ�ȭ
    }

    // ��� �巡�� �޼���
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {
        foreach (GameObject apple in appleObjects)
        {
            Vector3 appleScreenPos = Camera.main.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            if (appleScreenPos.x >= rectMinPos.x && appleScreenPos.x <= rectMaxPos.x &&
                appleScreenPos.y >= rectMinPos.y && appleScreenPos.y <= rectMaxPos.y)
            {
                if (!selectedApples.Contains(apple))
                {
                    selectedApples.Add(apple);
                }
            }
        }

        CalculateApples();
    }

    // ������ ��� ��ü���� ������ �� ��� �� ó�� �޼���
    private void CalculateApples()
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
        }
    }

    // ���� 10�� ������� ����� �޼���
    private void HideSelectedApples()
    {
        foreach (GameObject selectedApple in selectedApples)
        {
            //selectedApple.SetActive(false);
            Apple appleComponent = selectedApple.GetComponent<Apple>();
            if (appleComponent != null)
            {
                appleComponent.HideApple(); // ��� ��ü �����ϰ� �����
            }
        }
        ClearSelectedApples(); // ����Ʈ �ʱ�ȭ
    }

    // ���õ� ��� ��ü���� ��ȯ�ϴ� �޼���
    public List<GameObject> GetSelectedApples()
    {
        return selectedApples;
    }


    //public void UpdateScore()
    //{
    //    scoreText.text = score.ToString();
    //}

}
