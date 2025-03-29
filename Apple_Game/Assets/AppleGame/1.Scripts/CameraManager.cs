using UnityEngine;

public class CameraManager : MonoBehaviour
{


    #region Variables

    private const float TARGET_ASPECT = 16f / 9f;       // 화면비 16:9로 고정

    #endregion


    #region Unity Methods

    private void Awake()
    {
        InitializeCameraManager();
    }

    #endregion


    #region Initialize

    // CameraManager 초기화 함수
    private void InitializeCameraManager()
    {
        Camera mainCamera = GetComponent<Camera>();
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

    #endregion


}
