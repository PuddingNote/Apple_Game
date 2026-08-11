using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 강제 업데이트 차단 창. 씬/프리팹에 미리 만들어두지 않고, VersionGate가 필요할 때만
// 코드로 직접 생성한다(씬 파일을 건드릴 필요가 없어서 안전하다).
// 한 번 뜨면 닫히지 않는다 - 업데이트 버튼을 눌러 스토어에 갔다 돌아와도 여전히 막혀 있어야 하기 때문.
public class UpdateRequiredView : MonoBehaviour
{

    private const int SORT_ORDER = 999;                                     // 다른 UI보다 항상 위에 뜨도록
    private static readonly Color BackgroundColor = new Color(0f, 0f, 0f, 0.92f);
    private static readonly Color32 ButtonColor = new Color32(0, 200, 0, 255);   // 게임 내 다른 버튼과 동일한 색상
    private const float BUTTON_PIXELS_PER_UNIT_MULTIPLIER = 10f;                 // 게임 내 다른 버튼과 동일한 설정

    // 게임이 실제로 쓰는 폰트/버튼 스프라이트 (Resources 하위 경로)
    private const string FONT_RESOURCE_PATH = "2.Fonts/ONE Mobile POP SDF";
    private const string BUTTON_SPRITE_RESOURCE_PATH = "1.Sprites/Rounded Background";

    private string storeUrl;
    private TMP_FontAsset font;
    private Sprite buttonSprite;

    // 차단 창을 생성하고 표시한다. storeUrl이 비어 있으면 업데이트 버튼은 숨긴다.
    public static UpdateRequiredView Show(Transform parent, string message, string storeUrl)
    {
        GameObject viewObject = new GameObject("UpdateRequiredView");
        viewObject.transform.SetParent(parent, false);

        UpdateRequiredView view = viewObject.AddComponent<UpdateRequiredView>();
        view.storeUrl = storeUrl;
        view.Build(message);
        return view;
    }

    private void Build(string message)
    {
        // 못 찾아도(리소스 이름이 바뀌는 등) 크래시 없이 TMP 기본 폰트/단색으로 대체된다.
        font = Resources.Load<TMP_FontAsset>(FONT_RESOURCE_PATH);
        buttonSprite = Resources.Load<Sprite>(BUTTON_SPRITE_RESOURCE_PATH);

        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = SORT_ORDER;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f;

        gameObject.AddComponent<GraphicRaycaster>();

        // 화면 전체를 덮는 반투명 배경 (레이캐스트를 막아 뒤쪽 UI 클릭을 차단)
        GameObject background = CreateFullScreenChild("Background", transform);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = BackgroundColor;

        // 메시지 텍스트
        GameObject messageObject = new GameObject("MessageText");
        messageObject.transform.SetParent(background.transform, false);
        RectTransform messageRect = messageObject.AddComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0.5f, 0.5f);
        messageRect.anchorMax = new Vector2(0.5f, 0.5f);
        messageRect.anchoredPosition = new Vector2(0, 120);
        messageRect.sizeDelta = new Vector2(1200, 400);

        TextMeshProUGUI messageText = messageObject.AddComponent<TextMeshProUGUI>();
        messageText.text = message;
        messageText.fontSize = 48;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.white;
        if (font != null)
        {
            messageText.font = font;
        }

        // 버튼 두 개 (종료 / 업데이트)
        bool hasStoreUrl = !string.IsNullOrWhiteSpace(storeUrl);

        if (hasStoreUrl)
        {
            CreateButton(background.transform, "업데이트", new Vector2(-220, -180), OnUpdateClicked);
            CreateButton(background.transform, "종료", new Vector2(220, -180), OnQuitClicked);
        }
        else
        {
            CreateButton(background.transform, "종료", new Vector2(0, -180), OnQuitClicked);
        }
    }

    private GameObject CreateFullScreenChild(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return obj;
    }

    private void CreateButton(Transform parent, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(label + "Button");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(320, 100);

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = ButtonColor;
        if (buttonSprite != null)
        {
            buttonImage.sprite = buttonSprite;
            buttonImage.type = Image.Type.Sliced;
            buttonImage.fillCenter = true;
            buttonImage.pixelsPerUnitMultiplier = BUTTON_PIXELS_PER_UNIT_MULTIPLIER;
        }

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(onClick);

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 40;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        if (font != null)
        {
            text.font = font;
        }
    }

    private void OnUpdateClicked()
    {
        Application.OpenURL(storeUrl);
        // 창을 닫지 않는다 - 스토어에 갔다가 업데이트 없이 돌아와도 계속 막혀 있어야 한다.
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 뒤로가기(Escape/Android Back)는 종료로만 동작하게 한다.
    // 안 막으면 뒤로가기로 다른 패널(Pause 등)이 이 창 위로 뜰 수 있다.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitClicked();
        }
    }

}
