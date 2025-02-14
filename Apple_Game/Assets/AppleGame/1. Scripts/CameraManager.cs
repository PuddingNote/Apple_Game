using UnityEngine;

// 카메라 화면 비율을 16:9으로 고정하는 스크립트
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private float targetAspect = 16f / 9f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeCameraSystem();
    }

    private void InitializeCameraSystem()
    {
        Camera cam = FindObjectOfType<Camera>();
        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / targetAspect;

        Rect rect = cam.rect;

        if (scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) * 0.5f;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) * 0.5f;
        }

        cam.rect = rect;
    }
}