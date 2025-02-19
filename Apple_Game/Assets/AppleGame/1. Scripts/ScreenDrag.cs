using UnityEngine;

public class ScreenDrag : MonoBehaviour
{
    public GameManager gameManager;
    public ButtonManager buttonManager;

    private Vector2 dragStartPos;
    private Vector2 rectMinPos;
    private Vector2 rectMaxPos;
    private bool isDrag;

    // 매 프레임마다 드래그 상태를 업데이트
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

    // 입력을 처리하여 드래그 시작 및 종료를 결정
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

    // 드래그 시작 시 초기 설정
    private void StartDrag(Vector2 startPosition)
    {
        gameManager.ClearSelectedApples();

        isDrag = true;
        dragStartPos = ConvertToScreenPosition(startPosition);
    }

    // 드래그 종료 시 처리
    private void EndDrag()
    {
        isDrag = false;
        gameManager.CalculateApples();
    }

    // 드래그 중인 상태를 업데이트
    private void UpdateDrag()
    {
        Vector2 currentMousePos = ConvertToScreenPosition(Input.mousePosition);
        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

        gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
    }

    // 화면 좌표로 변환
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // 드래그 박스를 그리기 위한 GUI 처리
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
