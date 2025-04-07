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
    private List<GameObject> currentSelectedApples;                                     // 현재 드래그 영역 내에 있는 사과 List
    private List<GameObject> deselectApples;                                            // 드래그 영역에서 벗어나 선택 해제할 사과 List
    private List<RectTransform> rectTransformApples = new List<RectTransform>(120);     // 사과 UI 위치/크기 정보 캐싱 List
    private const float APPLE_BOUNDARY_SCALE = 0.3f;                                    // 사과 선택 판정 영역의 크기 배율

    [Header("--------------[ UI ]")]
    [SerializeField] private RectTransform selectionBox;    // 드래그 영역 표시용 UI
    private Canvas canvas;                                  // Canvas 참조
    private RectTransform canvasRectTransform;              // Canvas 위치/크기 정보

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;                              // mainCamera 참조

    [Header("--------------[ ETC ]")]
    [SerializeField] private GameManager gameManager;       // GameManager 참조
    [SerializeField] private OptionManager optionManager;   // OptionManager 참조

    #endregion


    #region Unity Methods

    private void Start()
    {
        InitializeDragManager();
    }

    private void Update()
    {
        if (gameManager.isCountingDown)
        {
            ClearDragState();
            return;
        }

        if (gameManager.isGameOver || optionManager.IsActiveEscPanel())
        {
            return;
        }

        HandleInputDrag();
        if (isDrag)
        {
            UpdateDrag();
        }
    }

    #endregion


    #region Initialize

    // DragManager 초기화 함수
    private void InitializeDragManager()
    {
        InitializeCamera();
        InitializeUI();
        InitializeCollections();
        CacheAppleTransforms();
    }

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
    }

    // 사과 리스트 초기화함수
    private void InitializeCollections()
    {
        currentSelectedApples = new List<GameObject>();
        deselectApples = new List<GameObject>();
    }

    // 사과 오브젝트의 RectTransform 캐싱 함수
    private void CacheAppleTransforms()
    {
        foreach (GameObject apple in gameManager.appleObjects)
        {
            rectTransformApples.Add(apple.GetComponent<RectTransform>());
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
        currentSelectedApples.Clear();
        deselectApples.Clear();
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

        // 드래그 영역의 두 코너 좌표 (스크린 좌표계)
        Vector2 startCorner = dragStartPos;
        Vector2 endCorner = currentMousePos;

        // 캔버스 공간의 좌표로 변환
        Vector2 startCanvasPos, endCanvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            startCorner,
            canvas.worldCamera,
            out startCanvasPos
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            endCorner,
            canvas.worldCamera,
            out endCanvasPos
        );

        // 시작점, 끝점 중 최소/최대 지점 계산 (캔버스 좌표계)
        Vector2 minCanvasPos = new Vector2(
            Mathf.Min(startCanvasPos.x, endCanvasPos.x),
            Mathf.Min(startCanvasPos.y, endCanvasPos.y)
        );
        Vector2 maxCanvasPos = new Vector2(
            Mathf.Max(startCanvasPos.x, endCanvasPos.x),
            Mathf.Max(startCanvasPos.y, endCanvasPos.y)
        );

        // 박스의 크기와 위치 계산
        Vector2 size = maxCanvasPos - minCanvasPos;
        Vector2 position = minCanvasPos + size / 2; // 박스의 중심 위치

        // RectTransform 설정
        selectionBox.anchorMin = new Vector2(0.5f, 0.5f);
        selectionBox.anchorMax = new Vector2(0.5f, 0.5f);
        selectionBox.pivot = new Vector2(0.5f, 0.5f);
        selectionBox.anchoredPosition = position;
        selectionBox.sizeDelta = size;
    }

    #endregion


    #region Input Handling

    // 입력을 처리하여 드래그 동작을 수행하는 함수
    private void HandleInputDrag()
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
    private void SelectApplesInDrag(Vector2 rectMinPos, Vector2 rectMaxPos)
    {
        currentSelectedApples.Clear();
        deselectApples.Clear();

        Rect dragRect = new Rect(
            rectMinPos.x,
            rectMinPos.y,
            rectMaxPos.x - rectMinPos.x,
            rectMaxPos.y - rectMinPos.y
        );

        GameObject[] appleObjects = gameManager.appleObjects;
        for (int i = 0; i < appleObjects.Length; i++)
        {
            GameObject apple = appleObjects[i];
            Apple appleComponent = apple.GetComponent<Apple>();
            if (appleComponent == null || appleComponent.appleNum == 0 || appleComponent.isDropping)
            {
                continue;
            }

            RectTransform rectTransform = rectTransformApples[i];
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
                currentSelectedApples.Add(apple);
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
        List<GameObject> selectedApples = gameManager.selectedApples;
        foreach (GameObject apple in selectedApples)
        {
            if (!currentSelectedApples.Contains(apple))
            {
                deselectApples.Add(apple);
            }
        }

        foreach (GameObject apple in deselectApples)
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

    // 드래그 업데이트 함수
    private void UpdateDrag()
    {
        currentMousePos = Input.mousePosition;

        // 드래그 영역의 최소/최대 좌표 계산
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

    // 드래그 종료 함수
    public void EndDrag()
    {
        isDrag = false;
        selectionBox.gameObject.SetActive(false);
        gameManager.CalculateApples();
    }

    #endregion


}
