using UnityEngine;

public class ScreenDrag : MonoBehaviour
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

    public enum SelectMode
    {
        Drag,
        Click
    }
    private SelectMode currentDragMode = SelectMode.Drag;




    private void Update()
    {
        if (gameManager.isGameOver || buttonManager.IsActiveEscPanel())
        {
            return;
        }

        HandleInput();

        if (isDrag)
        {
            UpdateNormalDrag();
        }
    }




    // ���� ��� ���� �� ���� �ʱ�ȭ
    public void SetDragMode(SelectMode mode)
    {
        currentDragMode = mode;
        isDrag = false;
        isSelected = false;
        gameManager.ClearSelectedApples();
        gameManager.ClearLastSelectedApples();
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
                    StartNormalDrag(touchPos);
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
                EndNormalDrag();
            }
        }
    }

    // Ŭ�� ��忡�� ����� �����ϰ� ������ ���
    private void HandleAppleClickDrag(Vector2 clickPos)
    {
        GameObject clickedApple = gameManager.GetAppleAtPosition(ConvertToScreenPosition(clickPos));

        if (clickedApple != null)
        {
            if (!isSelected)
            {
                // ù ��° ��� ����
                isSelected = true;
                firstClickPos = ConvertToScreenPosition(clickPos);
                gameManager.ClearSelectedApples();
                gameManager.ClearLastSelectedApples();
                gameManager.AddClickedApple(clickedApple);
            }
            else
            {
                // �� Ŭ���� �������� �簢�� �������� ��� ���� ���� 
                Vector2 secondClickPos = ConvertToScreenPosition(clickPos);

                rectMinPos = Vector2.Min(firstClickPos, secondClickPos);
                rectMaxPos = Vector2.Max(firstClickPos, secondClickPos);

                gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos);
                gameManager.CalculateApples();
                gameManager.ChangeSelectedApplesNumberColor(Color.green);

                isSelected = false;
            }
        }
    }

    private void StartNormalDrag(Vector2 startPosition)
    {
        isDrag = true;
        dragStartPos = ConvertToScreenPosition(startPosition);
    }

    public void EndNormalDrag()
    {
        isDrag = false;
        gameManager.CalculateApples();
    }

    private void UpdateNormalDrag()
    {
        Vector2 currentMousePos = ConvertToScreenPosition(Input.mousePosition);
        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

        gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
    }

    // ȭ�� ��ǥ�� ��ȯ�Ͽ� ��ȯ
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
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
