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
        // 유니티 에디터에서는 업데이트 기능을 실행하지 않음
#else
        StartCoroutine(CheckForUpdate());
#endif
    }

    IEnumerator CheckForUpdate()
    {
        appUpdateManager = new AppUpdateManager();

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        // 앱 정보 확인이 완료 될 때까지 기다리는 상태
        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            // 업데이트가 있는 상태
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfoResult, appUpdateOptions);
                while (!startUpdateRequest.IsDone)
                {
                    if (startUpdateRequest.Status == AppUpdateStatus.Downloading)
                    {
                        // 업데이트 다운로드가 진행중인 상태

                    }
                    else if (startUpdateRequest.Status == AppUpdateStatus.Downloaded)
                    {
                        // 업데이트 다운로드가 완료된 상태

                    }

                    yield return null;
                }

                // 완료되었는지 마지막 확인
                var result = appUpdateManager.CompleteUpdate();
                while (!result.IsDone)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return (int)startUpdateRequest.Status;
            }
            else if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                // 업데이트가 없는 상태

                // GPGS 로그인이 있다면 여기에서 호출
                // GPGS_LOGIN();
                yield return (int)UpdateAvailability.UpdateNotAvailable;
            }
            else
            {
                // 업데이트 가능 여부를 알 수 없는 상태

                yield return (int)UpdateAvailability.Unknown;
            }
        }
        else
        {
            // 앱 정보가 확인이 안되는 상태
        }
    }

}
