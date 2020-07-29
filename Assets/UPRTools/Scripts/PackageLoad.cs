using UnityEngine;

namespace UPRProfiler
{
    public class PackageLoad : MonoBehaviour
    {
        public static bool useLua = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnStartGame()
        {

            GameObject uprGameObject = new GameObject("UPRGameObject");
            uprGameObject.name = "UPRProfiler";
            uprGameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(uprGameObject);
            uprGameObject.AddComponent<InnerPackageS>();
            NetworkServer.ConnectTcpPort(56000);
        }
    }
}

