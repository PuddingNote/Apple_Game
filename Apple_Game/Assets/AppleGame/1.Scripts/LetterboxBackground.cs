using UnityEngine;

// 16:9(1920x1080) 밖으로 남는 레터박스/필러박스 영역을 항상 검은색으로 채워주는 배경용 카메라.
// CameraManager가 각 씬의 메인 카메라 rect를 16:9에 맞게 줄여주지만, 그 rect 밖 영역은
// 메인 카메라가 그려주지 않는다 - 기기/GPU에 따라 이전 프레임 잔상이 보일 수 있어서,
// 아무것도 그리지 않고 화면 전체를 검은색으로 지우기만 하는 카메라를 메인 카메라보다
// 먼저(더 낮은 depth) 그려서 그 문제를 원천 차단한다.
// 씬 파일을 건드리지 않도록 GameBootstrap에서 한 번만 코드로 생성해서 계속 유지한다.
public static class LetterboxBackground
{

    public static void Create()
    {
        GameObject cameraObject = new GameObject("LetterboxBackgroundCamera");
        Camera camera = cameraObject.AddComponent<Camera>();

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.cullingMask = 0;              // 아무 오브젝트도 그리지 않고 화면만 지운다
        camera.depth = -100;                 // 항상 메인 카메라보다 먼저 그려지도록 최하위
        camera.rect = new Rect(0f, 0f, 1f, 1f);
        camera.useOcclusionCulling = false;
        camera.allowHDR = false;
        camera.allowMSAA = false;
        // AudioListener는 추가하지 않는다 - 각 씬의 메인 카메라가 이미 갖고 있어서
        // 하나 더 추가하면 "There are 2 audio listeners" 경고가 뜬다.

        Object.DontDestroyOnLoad(cameraObject);
    }

}
