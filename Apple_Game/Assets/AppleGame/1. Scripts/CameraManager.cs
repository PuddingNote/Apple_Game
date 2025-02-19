using UnityEngine;

// 카메라 화면 비율을 16:9으로 고정하는 스크립트
public class CameraManager : MonoBehaviour
{
    private const float TARGET_ASPECT = 16f / 9f;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        InitializeCameraSystem();
    }

    private void InitializeCameraSystem()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / TARGET_ASPECT;

        Rect rect = mainCamera.rect;

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

        mainCamera.rect = rect;
    }


}