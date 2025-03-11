using UnityEngine;
using System.Collections.Generic;

public class SelectModeManager : MonoBehaviour
{
    private Vector2 dragStartPos;           // 드래그 시작 위치
    private Vector2 rectMinPos;             // 선택 영역 최소 좌표
    private Vector2 rectMaxPos;             // 선택 영역 최대 좌표
    private bool isDrag;                    // 드래그 상태 여부

    [Header("--------------[ ETC ]")]
    public GameManager gameManager;         // gameManager 참조
    public ButtonManager buttonManager;     // ButtonManager 참조

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;              // 메인 카메라 참조

    private void Awake()
    {
        mainCamera = Camera.main;
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

    // 입력을 처리하여 드래그 동작 수행
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 touchPos = Input.mousePosition;

            StartDrag(touchPos);
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (isDrag)
            {
                EndDrag();
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
            if (appleComponent == null || appleComponent.appleNum == 0 || appleComponent.isDropping)
            {
                continue;
            }

            RectTransform rectTransform = apple.GetComponent<RectTransform>();
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            float halfWidth = rectTransform.rect.width * 0.5f;
            float halfHeight = rectTransform.rect.height * 0.5f;

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

    // 드래그 선택 영역을 GUI로 표시
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
