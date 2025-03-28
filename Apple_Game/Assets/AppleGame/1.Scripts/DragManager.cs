using UnityEngine;
using System.Collections.Generic;

public class DragManager : MonoBehaviour
{


    #region Variables

    [Header("--------------[ Drag Setting ]")]
    private Vector2 dragStartPos;                           // 드래그 시작 위치
    private Vector2 rectMinPos;                             // 선택 영역 최소 좌표 (좌하단 좌표)
    private Vector2 rectMaxPos;                             // 선택 영역 최대 좌표 (우상단 좌표)
    [HideInInspector] public bool isDrag;                   // 드래그 상태 여부

    private Vector2 currentMousePos;                                                    // 현재 마우스(터치) 스크린 좌표
    private List<GameObject> currentlySelected;                                         // 현재 드래그 영역 내에 있는 사과 List
    private List<GameObject> applesToDeselect;                                          // 드래그 영역에서 벗어나 선택 해제할 사과 List
    private List<RectTransform> appleRectTransforms = new List<RectTransform>(120);     // 사과 UI 위치/크기 정보 캐싱 List
    private const float APPLE_BOUNDARY_SCALE = 0.4f;                                    // 사과 선택 판정 영역의 크기 배율

    [Header("--------------[ UI ]")]
    [SerializeField] private RectTransform selectionBox;    // 드래그 영역 표시용 UI
    private Canvas canvas;                                  // Canvas 참조
    private RectTransform canvasRectTransform;              // Canvas 위치/크기 정보
    private Vector2 canvasScale;                            // Canvas 스케일 값

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;                              // mainCamera 참조

    [Header("--------------[ ETC ]")]
    [SerializeField] private GameManager gameManager;       // GameManager 참조
    [SerializeField] private ButtonManager buttonManager;   // ButtonManager 참조

    #endregion


    #region Unity Methods

    private void Start()
    {
        InitializeCamera();
        InitializeUI();
        InitializeCollections();
        CacheAppleTransforms();
    }

    private void Update()
    {
        if (gameManager.isCountingDown)
        {
            ClearDragState();
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

    #endregion


    #region Initialize

    // 카메라 초기화 함수
    private void InitializeCamera()
    {
        mainCamera = Camera.main;
    }

    // UI 초기화 및 스케일 설정 함수
    private void InitializeUI()
    {
        canvas = selectionBox.GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        selectionBox.gameObject.SetActive(false);

        // Canvas 스케일 초기화
        canvasScale = new Vector2(
            canvasRectTransform.rect.width / Screen.width,
            canvasRectTransform.rect.height / Screen.height
        );
    }

    // 사과 리스트 초기화함수
    private void InitializeCollections()
    {
        currentlySelected = new List<GameObject>();
        applesToDeselect = new List<GameObject>();
    }

    // 사과 오브젝트의 RectTransform 캐싱 함수
    private void CacheAppleTransforms()
    {
        foreach (var apple in gameManager.appleObjects)
        {
            appleRectTransforms.Add(apple.GetComponent<RectTransform>());
        }
    }

    #endregion


    #region Update State & UI

    // 드래그 상태를 초기화하고 선택된 사과들을 모두 해제하는 함수
    private void ClearDragState()
    {
        if (!isDrag)
        {
            return;
        }

        isDrag = false;
        currentlySelected.Clear();
        applesToDeselect.Clear();
        gameManager.ClearSelectedApples();
    }

    // 드래그 선택 영역을 표시하는 함수
    private void UpdateSelectionBox()
    {
        if (!isDrag)
        {
            selectionBox.gameObject.SetActive(false);
            return;
        }

        selectionBox.gameObject.SetActive(true);

        // 드래그 영역의 크기 계산
        Vector2 size = new Vector2(
            Mathf.Abs(rectMaxPos.x - rectMinPos.x),
            Mathf.Abs(rectMaxPos.y - rectMinPos.y)
        );

        // 스크린 좌표를 Canvas 좌표로 변환
        Vector2 screenCenter = new Vector2(
            (rectMaxPos.x + rectMinPos.x) * 0.5f,
            (rectMaxPos.y + rectMinPos.y) * 0.5f
        );

        // Canvas 상의 위치 계산
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenCenter,
            canvas.worldCamera,
            out canvasPosition
        );

        // UI 업데이트
        canvasPosition = new Vector2(canvasPosition.x, -canvasPosition.y);
        selectionBox.anchoredPosition = canvasPosition;
        selectionBox.sizeDelta = new Vector2(
            size.x * canvasScale.x,
            size.y * canvasScale.y
        );
    }

    #endregion


    #region Input Handling

    // 입력을 처리하여 드래그 동작을 수행하는 함수
    private void HandleInput()
    {
        if (gameManager.isCountingDown)
        {
            return;
        }

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

    #endregion


    #region Drag Apple

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

        var appleObjects = gameManager.appleObjects;
        for (int i = 0; i < appleObjects.Length; i++)
        {
            var apple = appleObjects[i];
            var appleComponent = apple.GetComponent<Apple>();
            if (appleComponent == null || appleComponent.appleNum == 0 || appleComponent.isDropping)
            {
                continue;
            }

            var rectTransform = appleRectTransforms[i];
            Vector3 appleScreenPos = mainCamera.WorldToScreenPoint(apple.transform.position);
            appleScreenPos.y = Screen.height - appleScreenPos.y;

            float halfWidth = rectTransform.rect.width * APPLE_BOUNDARY_SCALE;
            float halfHeight = rectTransform.rect.height * APPLE_BOUNDARY_SCALE;

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

        ProcessDeselectedApples();
    }

    // 선택 해제 로직 최적화 함수
    private void ProcessDeselectedApples()
    {
        var selectedApples = gameManager.selectedApples;
        foreach (var apple in selectedApples)
        {
            if (!currentlySelected.Contains(apple))
            {
                applesToDeselect.Add(apple);
            }
        }

        foreach (var apple in applesToDeselect)
        {
            selectedApples.Remove(apple);
            apple.GetComponent<Apple>().ShowApple();
        }
    }

    #endregion


    #region Drag Operations

    // 드래그 시작 함수
    private void StartDrag(Vector2 startPosition)
    {
        isDrag = true;

        dragStartPos = startPosition;
        currentMousePos = dragStartPos;
    }

    // 드래그 종료 함수
    public void EndDrag()
    {
        isDrag = false;
        selectionBox.gameObject.SetActive(false);
        gameManager.CalculateApples();
    }

    // 드래그 업데이트 함수
    private void UpdateDrag()
    {
        currentMousePos = Input.mousePosition;

        // 최소/최대 좌표 계산
        rectMinPos = new Vector2(
            Mathf.Min(dragStartPos.x, currentMousePos.x),
            Mathf.Min(Screen.height - dragStartPos.y, Screen.height - currentMousePos.y)
        );
        rectMaxPos = new Vector2(
            Mathf.Max(dragStartPos.x, currentMousePos.x),
            Mathf.Max(Screen.height - dragStartPos.y, Screen.height - currentMousePos.y)
        );

        SelectApplesInDrag(rectMinPos, rectMaxPos);
        gameManager.ChangeSelectedApplesNumberColor(Color.green);
        UpdateSelectionBox();
    }

    #endregion


}
