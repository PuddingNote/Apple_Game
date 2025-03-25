using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine;

public class GoogleManager : MonoBehaviour
{
    AppUpdateManager appUpdateManager;

    private void Awake()
    {
#if UNITY_EDITOR
        // ����Ƽ �����Ϳ����� ������Ʈ ����� �������� ����
#else
        StartCoroutine(CheckForUpdate());
#endif
    }

    IEnumerator CheckForUpdate()
    {
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        // �� ���� Ȯ���� �Ϸ� �� ������ ��ٸ��� ����
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            // ������Ʈ�� �ִ� ����
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
                while (!startUpdateRequest.IsDone)
                {
                    if (startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        // ������Ʈ �ٿ�ε尡 �������� ����

                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        // ������Ʈ �ٿ�ε尡 �Ϸ�� ����

                    }

                    yield return null;
                }

                // �Ϸ�Ǿ����� ������ Ȯ��
                var result = appUpdateManager.CompleteUpdate();
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                // ������Ʈ�� ���� ����

                // GPGS �α����� �ִٸ� ���⿡�� ȣ��
                // GPGS_LOGIN();
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            else
            {
                // ������Ʈ ���� ���θ� �� �� ���� ����

                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else
        {
            // �� ������ Ȯ���� �ȵǴ� ����
        }
    }

}
