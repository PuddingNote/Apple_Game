using UnityEngine;
using System.Collections.Generic;

public class DragManager : MonoBehaviour
{


    #region Variables

    [Header("--------------[ Drag Setting ]")]
    private Vector2 dragStartPos;                           // �巡�� ���� ��ġ
    private Vector2 rectMinPos;                             // ���� ���� �ּ� ��ǥ (���ϴ� ��ǥ)
    private Vector2 rectMaxPos;                             // ���� ���� �ִ� ��ǥ (���� ��ǥ)
    [HideInInspector] public bool isDrag;                   // �巡�� ���� ����

    private Vector2 currentMousePos;                                                    // ���� ���콺(��ġ) ��ũ�� ��ǥ
    private List<GameObject> currentSelectedApples;                                     // ���� �巡�� ���� ���� �ִ� ��� List
    private List<GameObject> deselectApples;                                            // �巡�� �������� ��� ���� ������ ��� List
    private List<RectTransform> rectTransformApples = new List<RectTransform>(120);     // ��� UI ��ġ/ũ�� ���� ĳ�� List
    private const float APPLE_BOUNDARY_SCALE = 0.3f;                                    // ��� ���� ���� ������ ũ�� ����

    [Header("--------------[ UI ]")]
    [SerializeField] private RectTransform selectionBox;    // �巡�� ���� ǥ�ÿ� UI
    private Canvas canvas;                                  // Canvas ����
    private RectTransform canvasRectTransform;              // Canvas ��ġ/ũ�� ����

    [Header("--------------[ Camera ]")]
    private Camera mainCamera;                              // mainCamera ����

    [Header("--------------[ ETC ]")]
    [SerializeField] private GameManager gameManager;       // GameManager ����
    [SerializeField] private OptionManager optionManager;   // OptionManager ����

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

    // DragManager �ʱ�ȭ �Լ�
    private void InitializeDragManager()
    {
        InitializeCamera();
        InitializeUI();
        InitializeCollections();
        CacheAppleTransforms();
    }

    // ī�޶� �ʱ�ȭ �Լ�
    private void InitializeCamera()
    {
        mainCamera = Camera.main;
    }

    // UI �ʱ�ȭ �� ������ ���� �Լ�
    private void InitializeUI()
    {
        canvas = selectionBox.GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        selectionBox.gameObject.SetActive(false);
    }

    // ��� ����Ʈ �ʱ�ȭ�Լ�
    private void InitializeCollections()
    {
        currentSelectedApples = new List<GameObject>();
        deselectApples = new List<GameObject>();
    }

    // ��� ������Ʈ�� RectTransform ĳ�� �Լ�
    private void CacheAppleTransforms()
    {
        foreach (GameObject apple in gameManager.appleObjects)
        {
            rectTransformApples.Add(apple.GetComponent<RectTransform>());
        }
    }

    #endregion


    #region Update State & UI

    // �巡�� ���¸� �ʱ�ȭ�ϰ� ���õ� ������� ��� �����ϴ� �Լ�
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

    // �巡�� ���� ������ ǥ���ϴ� �Լ�
    private void UpdateSelectionBox()
    {
        if (!isDrag)
        {
            selectionBox.gameObject.SetActive(false);
            return;
        }

        selectionBox.gameObject.SetActive(true);

        // �巡�� ������ �� �ڳ� ��ǥ (��ũ�� ��ǥ��)
        Vector2 startCorner = dragStartPos;
        Vector2 endCorner = currentMousePos;

        // ĵ���� ������ ��ǥ�� ��ȯ
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

        // ������, ���� �� �ּ�/�ִ� ���� ��� (ĵ���� ��ǥ��)
        Vector2 minCanvasPos = new Vector2(
            Mathf.Min(startCanvasPos.x, endCanvasPos.x),
            Mathf.Min(startCanvasPos.y, endCanvasPos.y)
        );
        Vector2 maxCanvasPos = new Vector2(
            Mathf.Max(startCanvasPos.x, endCanvasPos.x),
            Mathf.Max(startCanvasPos.y, endCanvasPos.y)
        );

        // �ڽ��� ũ��� ��ġ ���
        Vector2 size = maxCanvasPos - minCanvasPos;
        Vector2 position = minCanvasPos + size / 2; // �ڽ��� �߽� ��ġ

        // RectTransform ����
        selectionBox.anchorMin = new Vector2(0.5f, 0.5f);
        selectionBox.anchorMax = new Vector2(0.5f, 0.5f);
        selectionBox.pivot = new Vector2(0.5f, 0.5f);
        selectionBox.anchoredPosition = position;
        selectionBox.sizeDelta = size;
    }

    #endregion


    #region Input Handling

    // �Է��� ó���Ͽ� �巡�� ������ �����ϴ� �Լ�
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

    // �巡�� ���� ���� ������� �����ϴ� �Լ�
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

    // ���� ���� ���� ����ȭ �Լ�
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

    // �巡�� ���� �Լ�
    private void StartDrag(Vector2 startPosition)
    {
        isDrag = true;

        dragStartPos = startPosition;
        currentMousePos = dragStartPos;
    }

    // �巡�� ������Ʈ �Լ�
    private void UpdateDrag()
    {
        currentMousePos = Input.mousePosition;

        // �巡�� ������ �ּ�/�ִ� ��ǥ ���
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

    // �巡�� ���� �Լ�
    public void EndDrag()
    {
        isDrag = false;
        selectionBox.gameObject.SetActive(false);
        gameManager.CalculateApples();
    }

    #endregion


}
