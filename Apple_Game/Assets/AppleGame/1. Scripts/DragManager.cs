using UnityEngine;
using System.Collections.Generic;

public class DragManager : MonoBehaviour
{
    private Vector2 dragStartPos;               // 드래그 시작 위치
    private Vector2 rectMinPos;                 // 선택 영역 최소 좌표
    private Vector2 rectMaxPos;                 // 선택 영역 최대 좌표
    [HideInInspector] public bool isDrag;       // 드래그 상태 여부

    private Vector2 currentMousePos;            // 현재 마우스(터치) 위치
    private List<GameObject> currentlySelected; // 현재 선택된 사과 목록
    private List<GameObject> applesToDeselect;  // 선택 해제할 사과 목록

    [Header("--------------[ ETC ]")]
    public GameManager gameManager;             // gameManager 참조
    public ButtonManager buttonManager;         // ButtonManager 참조

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;                  // 메인 카메라 참조

    private void Awake()
    {
        mainCamera = Camera.main;

        currentlySelected = new List<GameObject>();
        applesToDeselect = new List<GameObject>();
    }

    private void Update()
    {
        // 카운트다운 중이면 모든 드래그 상태 초기화 및 입력 차단
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

    // 입력을 처리하여 드래그 동작을 수행하는 함수
    private void HandleInput()
    {
        // 카운트다운 중이면 모든 입력 무시
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

    // 드래그 영역 내의 사과들을 선택하는 함수
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

            // 사과의 RectTransform 정보 가져오기
            RectTransform rectTransform = apple.GetComponent<RectTransform>();
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            // 사과의 경계 영역 계산
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

        // 선택 해제된 사과들 처리
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

    // 드래그 시작 함수
    private void StartDrag(Vector2 startPosition)
    {
        isDrag = true;
        dragStartPos = ConvertToScreenPosition(startPosition);
    }

    // 드래그 종료 함수
    public void EndDrag()
    {
        isDrag = false;
        gameManager.CalculateApples();
    }

    // 드래그 업데이트 함수
    private void UpdateDrag()
    {
        currentMousePos = ConvertToScreenPosition(Input.mousePosition);
        rectMinPos = Vector2.Min(currentMousePos, dragStartPos);
        rectMaxPos = Vector2.Max(currentMousePos, dragStartPos);

        SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
    }

    // 화면 좌표를 변환하여 반환하는 함수
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // 드래그 선택 영역을 GUI로 표시하는 함수
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
