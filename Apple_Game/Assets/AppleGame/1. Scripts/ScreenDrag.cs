using UnityEngine;

public class ScreenDrag : MonoBehaviour
{
    private Vector2 dragStartPos;           // 드래그 시작 위치
    private Vector2 rectMinPos;             // 선택 영역 최소 좌표
    private Vector2 rectMaxPos;             // 선택 영역 최대 좌표
    private bool isDrag;                    // 드래그 상태 여부

    private Vector2 firstClickPos;          // 첫번째 클릭 좌표
    private bool isSelected;                // 사과 선택 여부 (클릭)

    [Header("--------------[ ETC ]")]
    public GameManager gameManager;         // gameManager 참조
    public ButtonManager buttonManager;     // ButtonManager 참조

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




    // 선택 모드 설정 및 상태 초기화
    public void SetDragMode(SelectMode mode)
    {
        currentDragMode = mode;
        isDrag = false;
        isSelected = false;
        gameManager.ClearSelectedApples();
        gameManager.ClearLastSelectedApples();
    }

    // 입력을 처리하여 모드에 따라 동작 수행
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

    // 클릭 모드에서 사과를 선택하고 범위를 계산
    private void HandleAppleClickDrag(Vector2 clickPos)
    {
        GameObject clickedApple = gameManager.GetAppleAtPosition(ConvertToScreenPosition(clickPos));

        if (clickedApple != null)
        {
            if (!isSelected)
            {
                // 첫 번째 사과 선택
                isSelected = true;
                firstClickPos = ConvertToScreenPosition(clickPos);
                gameManager.ClearSelectedApples();
                gameManager.ClearLastSelectedApples();
                gameManager.AddClickedApple(clickedApple);
            }
            else
            {
                // 두 클릭을 기준으로 사각형 영역내에 사과 상태 변경 
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

    // 화면 좌표를 변환하여 반환
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // 드래그 모드에서 선택 영역을 GUI로 표시
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
