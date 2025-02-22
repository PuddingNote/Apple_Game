using UnityEngine;
using System.Collections.Generic;

public class SelectModeManager : MonoBehaviour
{
    private Vector2 dragStartPos;           // �巡�� ���� ��ġ
    private Vector2 rectMinPos;             // ���� ���� �ּ� ��ǥ
    private Vector2 rectMaxPos;             // ���� ���� �ִ� ��ǥ
    private bool isDrag;                    // �巡�� ���� ����

    private Vector2 firstClickPos;          // ù��° Ŭ�� ��ǥ
    private bool isSelected;                // ��� ���� ���� (Ŭ��)

    [Header("--------------[ ETC ]")]
    public GameManager gameManager;         // gameManager ����
    public ButtonManager buttonManager;     // ButtonManager ����

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;              // ���� ī�޶� ����

    public enum SelectMode
    {
        Drag,
        Click
    }
    private SelectMode currentDragMode = SelectMode.Drag;

    private const string DRAG_MODE_KEY = "DragMode";  // PlayerPrefs Ű��

    private void Awake()
    {
        mainCamera = Camera.main;
        LoadSavedMode();
    }

    private void Update()
    {
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

    // ����� ��� �ҷ�����
    private void LoadSavedMode()
    {
        int savedMode = PlayerPrefs.GetInt(DRAG_MODE_KEY, 0);
        currentDragMode = savedMode == 0 ? SelectMode.Drag : SelectMode.Click;
    }

    // ���� ��� ���� �� ���� �ʱ�ȭ
    public void SetSelectMode(SelectMode mode)
    {
        currentDragMode = mode;
        isDrag = false;
        isSelected = false;
        gameManager.ClearSelectedApples();
        gameManager.ClearLastSelectedApples();

        // ��� ���� �� PlayerPrefs�� ����
        PlayerPrefs.SetInt(DRAG_MODE_KEY, mode == SelectMode.Drag ? 0 : 1);
        PlayerPrefs.Save();
    }

    // �Է��� ó���Ͽ� ��忡 ���� ���� ����
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 touchPos = Input.mousePosition;

            switch (currentDragMode)
            {
                case SelectMode.Drag:
                    StartDrag(touchPos);
                    break;

                case SelectMode.Click:
                    HandleAppleClickDrag(touchPos);
                    break;
            }
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (isDrag)
            {
                EndDrag();
            }
        }
    }

    // Ŭ�� ��忡�� ����� �����ϰ� ������ ���
    private void HandleAppleClickDrag(Vector2 clickPos)
    {
        GameObject clickedApple = GetAppleAtPosition(ConvertToScreenPosition(clickPos));

        if (clickedApple != null)
        {
            if (!isSelected)
            {
                // ù ��° ��� ����
                isSelected = true;
                firstClickPos = ConvertToScreenPosition(clickPos);
                gameManager.ClearSelectedApples();
                gameManager.ClearLastSelectedApples();
                AddClickedApple(clickedApple);
            }
            else
            {
                // �� Ŭ���� �������� �簢�� �������� ��� ���� ���� 
                Vector2 secondClickPos = ConvertToScreenPosition(clickPos);

                rectMinPos = Vector2.Min(firstClickPos, secondClickPos);
                rectMaxPos = Vector2.Max(firstClickPos, secondClickPos);

                SelectApplesInDrag(rectMinPos, rectMaxPos);
                gameManager.CalculateApples();
                gameManager.ChangeSelectedApplesNumberColor(Color.green);

                isSelected = false;
            }
        }
    }

    // �巡�� ���� ���� ������� ����
    public void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {
        List<GameObject> currentlySelected = new List<GameObject>();

        foreach (GameObject apple in gameManager.appleObjects)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent == null || appleComponent.appleNum == 0)
            {
                continue;
            }

            // ����� RectTransform ���� ��������
            RectTransform rectTransform = apple.GetComponent<RectTransform>();
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            // ����� ��� ���� ��� (50% ũ��)
            float halfWidth = rectTransform.rect.width * 0.25f;
            float halfHeight = rectTransform.rect.height * 0.25f;

            Rect appleRect = new Rect(
                appleScreenPos.x - halfWidth,
                appleScreenPos.y - halfHeight,
                halfWidth * 2,
                halfHeight * 2
            );

            // �巡�� ������ ��� ������ ��ġ���� Ȯ��
            Rect dragRect = new Rect(
                rectMinPos.x,
                rectMinPos.y,
                rectMaxPos.x - rectMinPos.x,
                rectMaxPos.y - rectMinPos.y
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
        List<GameObject> applesToDeselect = new List<GameObject>();
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
            Apple appleComponent = apple.GetComponent<Apple>();
            appleComponent.ShowApple();
        }
    }

    // �־��� ȭ�� ��ǥ�� ��ġ�� ��� ��ü�� ��ȯ
    public GameObject GetAppleAtPosition(Vector2 screenPos)
    {
        foreach (GameObject apple in gameManager.appleObjects)
        {
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent == null || appleComponent.appleNum == 0)
            {
                continue;
            }

            RectTransform rectTransform = apple.GetComponent<RectTransform>();
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            float halfWidth = rectTransform.rect.width * 0.25f;
            float halfHeight = rectTransform.rect.height * 0.25f;

            if (screenPos.x >= appleScreenPos.x - halfWidth &&
                screenPos.x <= appleScreenPos.x + halfWidth &&
                screenPos.y >= appleScreenPos.y - halfHeight &&
                screenPos.y <= appleScreenPos.y + halfHeight)
            {
                return apple;
            }
        }
        return null;
    }

    // Ŭ���� ��� �߰�
    public void AddClickedApple(GameObject apple)
    {
        gameManager.selectedApples.Add(apple);
        gameManager.lastSelectedApples.Add(apple);
        apple.GetComponent<Apple>().ChangeNumberColor(Color.green);
    }

    private void StartDrag(Vector2 startPosition)
    {
        isDrag = true;
        dragStartPos = ConvertToScreenPosition(startPosition);
    }

    public void EndDrag()
    {
        isDrag = false;
        gameManager.CalculateApples();
    }

    private void UpdateDrag()
    {
        Vector2 currentMousePos = ConvertToScreenPosition(Input.mousePosition);
        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

        SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
    }

    // ȭ�� ��ǥ�� ��ȯ�Ͽ� ��ȯ
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // ���� ���� ��带 ��ȯ�ϴ� �Լ� �߰�
    public SelectMode GetCurrentMode()
    {
        return currentDragMode;
    }

    // �巡�� ��忡�� ���� ������ GUI�� ǥ��
    private void OnGUI()
    {
        if (!isDrag || currentDragMode != SelectMode.Drag)
        {
            return;
        }

        Rect selectionRect = new Rect(rectMinPos, rectMaxPos - rectMinPos);
        GUI.Box(selectionRect, "");
    }

}
