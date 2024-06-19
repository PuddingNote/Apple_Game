using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("--------------[ Main ]")]
    public int score;               // ����
    public bool isGameOver;         // GameOver �Ǵ�

    [Header("--------------[ Object Pooling ]")]
    public GameObject applePrefab;
    public Transform appleGroup;
    public List<Apple> applePool;

    public int poolSize;            // ������Ʈ Ǯ�� Size

    [Header("--------------[ UI ]")]
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        Application.targetFrameRate = 60;   // ������ ����
        applePool = new List<Apple>();
        poolSize = 120;

        for (int index = 0; index < poolSize; index++)
        {
            MakeApple();
        }
    }

    
    private void Update()
    {
        if (!isGameOver)
        {
            UpdateScore();
        }
    }


    public void MakeApple()
    {
        GameObject newApple = Instantiate(applePrefab, appleGroup);
        Apple appleComponent = newApple.GetComponent<Apple>();

        if (appleComponent != null)
        {
            appleComponent.Initialize();
            applePool.Add(appleComponent);
        }
    }


    public Apple GetAppleFromPool()
    {
        foreach (Apple apple in applePool)
        {
            if (!apple.gameObject.activeInHierarchy)
            {
                return apple;
            }
        }
        return null;  // Ǯ�� ����� �� �ִ� ����� ���ٸ� null ��ȯ
    }

    public void SpawnApple(Vector3 position)
    {
        Apple apple = GetAppleFromPool();
        if (apple != null)
        {
            apple.transform.position = position;
            apple.Activate();
        }
        else
        {
            Debug.LogWarning("No available apples in the pool.");
        }
    }

    public void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

}
