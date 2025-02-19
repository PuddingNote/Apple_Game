using UnityEngine;

public class ScreenDrag : MonoBehaviour
{
    public GameManager gameManager;
    public ButtonManager buttonManager;

    private Vector2 dragStartPos;
    private Vector2 rectMinPos;
    private Vector2 rectMaxPos;
    private bool isDrag;

    // �� �����Ӹ��� �巡�� ���¸� ������Ʈ
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

    // �Է��� ó���Ͽ� �巡�� ���� �� ���Ḧ ����
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            StartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            EndDrag();
        }
    }

    // �巡�� ���� �� �ʱ� ����
    private void StartDrag(Vector2 startPosition)
    {
        gameManager.ClearSelectedApples();

        isDrag = true;
        dragStartPos = ConvertToScreenPosition(startPosition);
    }

    // �巡�� ���� �� ó��
    private void EndDrag()
    {
        isDrag = false;
        gameManager.CalculateApples();
    }

    // �巡�� ���� ���¸� ������Ʈ
    private void UpdateDrag()
    {
        Vector2 currentMousePos = ConvertToScreenPosition(Input.mousePosition);
        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

        gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
    }

    // ȭ�� ��ǥ�� ��ȯ
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // �巡�� �ڽ��� �׸��� ���� GUI ó��
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
