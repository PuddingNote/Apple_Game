using UnityEngine;
using System.Collections.Generic;

public class SelectModeManager : MonoBehaviour
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

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;              // 메인 카메라 참조

    public enum SelectMode
    {
        Drag,
        Click
    }
    private SelectMode currentDragMode = SelectMode.Drag;

    private const string DRAG_MODE_KEY = "DragMode";  // PlayerPrefs 키값

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

    // 저장된 모드 불러오기
    private void LoadSavedMode()
    {
        int savedMode = PlayerPrefs.GetInt(DRAG_MODE_KEY, 0);
        currentDragMode = savedMode == 0 ? SelectMode.Drag : SelectMode.Click;
    }

    // 선택 모드 설정 및 상태 초기화
    public void SetSelectMode(SelectMode mode)
    {
        currentDragMode = mode;
        isDrag = false;
        isSelected = false;
        gameManager.ClearSelectedApples();
        gameManager.ClearLastSelectedApples();

        // 모드 변경 시 PlayerPrefs에 저장
        PlayerPrefs.SetInt(DRAG_MODE_KEY, mode == SelectMode.Drag ? 0 : 1);
        PlayerPrefs.Save();
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

    // 클릭 모드에서 사과를 선택하고 범위를 계산
    private void HandleAppleClickDrag(Vector2 clickPos)
    {
        GameObject clickedApple = GetAppleAtPosition(ConvertToScreenPosition(clickPos));

        if (clickedApple != null)
        {
            if (!isSelected)
            {
                // 첫 번째 사과 선택
                isSelected = true;
                firstClickPos = ConvertToScreenPosition(clickPos);
                gameManager.ClearSelectedApples();
                gameManager.ClearLastSelectedApples();
                AddClickedApple(clickedApple);
            }
            else
            {
                // 두 클릭을 기준으로 사각형 영역내에 사과 상태 변경 
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

    // 드래그 영역 내의 사과들을 선택
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

            // 사과의 RectTransform 정보 가져오기
            RectTransform rectTransform = apple.GetComponent<RectTransform>();
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            // 사과의 경계 영역 계산 (50% 크기)
            float halfWidth = rectTransform.rect.width * 0.25f;
            float halfHeight = rectTransform.rect.height * 0.25f;

            Rect appleRect = new Rect(
                appleScreenPos.x - halfWidth,
                appleScreenPos.y - halfHeight,
                halfWidth * 2,
                halfHeight * 2
            );

            // 드래그 영역과 사과 영역이 겹치는지 확인
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

        // 선택 해제된 사과들 처리
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

    // 주어진 화면 좌표에 위치한 사과 객체를 반환
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

    // 클릭한 사과 추가
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

    // 화면 좌표를 변환하여 반환
    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    // 현재 선택 모드를 반환하는 함수 추가
    public SelectMode GetCurrentMode()
    {
        return currentDragMode;
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
