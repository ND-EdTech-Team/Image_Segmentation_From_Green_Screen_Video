using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace UPRLuaProfiler
{
    public class LuaDeepProfilerAssetSetting : ScriptableObject
    {

        #region memeber
        public bool isDeepMonoProfiler = false;
        public bool isDeepLuaProfiler = false;

        private static LuaDeepProfilerAssetSetting instance;
        public static LuaDeepProfilerAssetSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<LuaDeepProfilerAssetSetting>("Assets/UPRLuaDeepProfilerAssetSetting.asset");
                    if (instance == null)
                    {
                        Debug.Log("UPR Lua Profiler: cannot find integration settings, creating default settings");
                        instance = CreateInstance<LuaDeepProfilerAssetSetting>();
                        instance.name = "UPR Lua Profiler Integration Settings";
#if UNITY_EDITOR
                        AssetDatabase.CreateAsset(instance, "Assets/UPRLuaDeepProfilerAssetSetting.asset");
#endif
                    }
                }
                return instance;
            }
        }
        #endregion

    }
}
#endif
