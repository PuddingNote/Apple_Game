using UnityEngine;

// 게임이 켜질 때 씬과 무관하게 가장 먼저 한 번 실행되는 부트스트랩.
// 강제 업데이트 확인(VersionGate)과 광고(AdManager)를 씬에 수동으로 배치하지 않고
// 코드에서 직접 생성한다 - 예전 GoogleManager 오브젝트가 하던 자리를 대신한다.
public static class GameBootstrap
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        GameObject bootstrapObject = new GameObject("GameBootstrap");
        Object.DontDestroyOnLoad(bootstrapObject);

        VersionGate versionGate = bootstrapObject.AddComponent<VersionGate>();
        AdManager adManager = bootstrapObject.AddComponent<AdManager>();

        versionGate.Initialize();
        adManager.Initialize();
    }

}
