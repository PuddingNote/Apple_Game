using UnityEngine;

public class CameraManager : MonoBehaviour
{


    #region Variables

    private const float TARGET_ASPECT = 16f / 9f;       // ȭ��� 16:9�� ����

    #endregion


    #region Unity Methods

    private void Awake()
    {
        InitializeCameraManager();
    }

    #endregion


    #region Initialize

    // CameraManager �ʱ�ȭ �Լ�
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
