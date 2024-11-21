using UnityEngine;

public class ScreenDrag : MonoBehaviour
{
    private Vector2 currentMousePos;    // 실시간(현재 프레임) 마우스 좌표
    private Vector2 dragStartPos;       // 드래그 시작 지점 마우스 좌표
    private Vector2 rectMinPos;         // Rect의 최소 지점 좌표
    private Vector2 rectMaxPos;         // Rect의 최대 지점 좌표
    private bool isDrag = false;        // 드래그 여부

    public GameManager gameManager;


    private void Update()
    {
        // 게임 오버 상태라면 Update 메서드의 나머지 코드를 실행하지 않음
        if (gameManager.isGameOver) return;

        // 드래그 시작 - 이전에 선택된 사과 정보들 초기화
        if (Input.GetMouseButtonDown(0))
        {
            gameManager.ClearSelectedApples();
            isDrag = true;
            dragStartPos = Input.mousePosition;
            dragStartPos.y = Screen.height - dragStartPos.y;
        }
        // 터치 시작
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            gameManager.ClearSelectedApples();
            isDrag = true;
            dragStartPos = Input.GetTouch(0).position;
            dragStartPos.y = Screen.height - dragStartPos.y;
        }

        // 드래그 종료 - 선택된 사과 정보들 계산
        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            gameManager.CalculateApples();
        }
        // 터치 종료
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            isDrag = false;
            gameManager.CalculateApples();
        }

        // 드래그 진행중 - 좌표 업데이트
        if (isDrag)
        {
            // 드래그 진행중
            if (Input.GetMouseButton(0))
            {
                currentMousePos = Input.mousePosition;
                currentMousePos.y = Screen.height - currentMousePos.y;
            }
            // 터치 진행중
            else if (Input.touchCount > 0)
            {
                currentMousePos = Input.GetTouch(0).position;
                currentMousePos.y = Screen.height - currentMousePos.y;
            }

            rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
            rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

            gameManager.SelectApplesInDrag(rectMinPos, rectMaxPos);
            gameManager.ChangeSelectedApplesNumberColor(Color.green);    // 초록색으로 숫자 색상 변경
        }
    }

    // 드래그 영역 그리기
    private void OnGUI()
    {
        if (!isDrag) return;

        Rect selectionRect = new Rect
        {
            min = rectMinPos,
            max = rectMaxPos
        };

        GUI.Box(selectionRect, "");
    }

}
