using UnityEngine;
using System.Collections.Generic;

public class DragManager : MonoBehaviour
{
    private Vector2 dragStartPos;               // �巡�� ���� ��ġ
    private Vector2 rectMinPos;                 // ���� ���� �ּ� ��ǥ
    private Vector2 rectMaxPos;                 // ���� ���� �ִ� ��ǥ
    [HideInInspector] public bool isDrag;       // �巡�� ���� ����

    private Vector2 currentMousePos;            // ���� ���콺(��ġ) ��ġ
    private List<GameObject> currentlySelected; // ���� ���õ� ��� ���
    private List<GameObject> applesToDeselect;  // ���� ������ ��� ���

    [Header("--------------[ ETC ]")]
    public GameManager gameManager;             // gameManager ����
    public ButtonManager buttonManager;         // ButtonManager ����

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;                  // ���� ī�޶� ����

    private void Awake()
    {
        mainCamera = Camera.main;

        currentlySelected = new List<GameObject>();
        applesToDeselect = new List<GameObject>();
    }

    private void Update()
    {
        // ī��Ʈ�ٿ� ���̸� ��� �巡�� ���� �ʱ�ȭ �� �Է� ����
        if (gameManager.isCountingDown)
        {
            if (isDrag)
            {
                isDrag = false;
                currentlySelected.Clear();
                applesToDeselect.Clear();
                gameManager.ClearSelectedApples();
            }
            return;
        }

        if (gameManager.isGameOver || buttonManager.IsActiveEscPanel())
        {
            return;
        }

        HandleInput();

        if (isDrag)
        {
            UpdateDrag();
        }
    }

    // �Է��� ó���Ͽ� �巡�� ������ �����ϴ� �Լ�
    private void HandleInput()
    {
        // ī��Ʈ�ٿ� ���̸� ��� �Է� ����
        if (gameManager.isCountingDown) return;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            StartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (isDrag)
            {
                EndDrag();
            }
        }
    }

    // �巡�� ���� ���� ������� �����ϴ� �Լ�
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {
        currentlySelected.Clear();
        applesToDeselect.Clear();

        Rect dragRect = new Rect(
            rectMinPos.x,
            rectMinPos.y,
            rectMaxPos.x - rectMinPos.x,
            rectMaxPos.y - rectMinPos.y
        );

        foreach (GameObject apple in gameManager.appleObjects)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent == null || appleComponent.appleNum == 0 || appleComponent.isDropping)
            {
                continue;
            }

            // ����� RectTransform ���� ��������
            RectTransform rectTransform = apple.GetComponent<RectTransform>();
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            // ����� ��� ���� ���
            float halfWidth = rectTransform.rect.width * 0.4f;
            float halfHeight = rectTransform.rect.height * 0.4f;

            Rect appleRect = new Rect(
                appleScreenPos.x - halfWidth,
                appleScreenPos.y - halfHeight,
                halfWidth * 2,
                halfHeight * 2
            );

            if (dragRect.Overlaps(appleRect))
            {
                currentlySelected.Add(apple);
                if (!gameManager.selectedApples.Contains(apple))
                {
                    gameManager.selectedApples.Add(apple);
                }
            }
        }

        // ���� ������ ����� ó��
        foreach (GameObject apple in gameManager.selectedApples)
        {
            if (!currentlySelected.Contains(apple))
            {
                applesToDeselect.Add(apple);
            }
        }

        foreach (GameObject apple in applesToDeselect)
        {
            gameManager.selectedApples.Remove(apple);
            apple.GetComponent<Apple>().ShowApple();
        }
    }

    // �巡�� ���� �Լ�
    private void StartDrag(Vector2 startPosition)
    {
        isDrag = true;
        dragStartPos = ConvertToScreenPosition(startPosition);
    }

    // �巡�� ���� �Լ�
    public void EndDrag()
    {
        isDrag = false;
        gameManager.CalculateApples();
    }

    // �巡�� ������Ʈ �Լ�
    private void UpdateDrag()
    {
        currentMousePos = ConvertToScreenPosition(Input.mousePosition);
        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

        SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
    }

    // ȭ�� ��ǥ�� ��ȯ�Ͽ� ��ȯ�ϴ� �Լ�
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // �巡�� ���� ������ GUI�� ǥ���ϴ� �Լ�
    private void OnGUI()
    {
        if (!isDrag)
        {
            return;
        }

        Rect selectionRect = new Rect(rectMinPos, rectMaxPos - rectMinPos);
        GUI.Box(selectionRect, "");
    }

}
