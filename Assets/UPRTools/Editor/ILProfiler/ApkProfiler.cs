using System.IO;
using UPRLuaProfiler;
using UnityEditor;

#if UNITY_2018_2_OR_NEWER
using UnityEditor.Android;
using UnityEngine;

namespace UPRProfiler
{
    class ApkProfiler : IPostGenerateGradleAndroidProject
    {
        private string AssemblyPath = "./Temp/gradleOut/src/main/assets/bin/Data/Managed/";

        public int callbackOrder { get { return 0; } }
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var setting = UPRToolSetting.Instance;
            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);

            if (!Directory.Exists(AssemblyPath))
            {
                AssemblyPath = "./Temp/gradleOut/unityLibrary/src/main/assets/bin/Data/Managed/";
            }
        
            if (setting.enableMonoProfiler)
            {
                InjectMethods.InjectAllMethods(AssemblyPath + "Assembly-CSharp.dll");
                Debug.Log("Listening Mono Profiler Success");
            }

            if (backend == ScriptingImplementation.Mono2x)
            {
            
                if (setting.loadScene)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.CoreModule.dll", "SceneManager", "LoadScene");
                }
                if (setting.loadAsset)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.AssetBundleModule.dll", "AssetBundle", "LoadAsset");
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.AssetBundleModule.dll", "AssetBundle", "Load");
                }
                if (setting.loadAssetBundle)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.AssetBundleModule.dll", "AssetBundle", "LoadFromFile");
                }
                if (setting.instantiate)
                {
                    InjectUtils.addProfiler(AssemblyPath, "UnityEngine.CoreModule.dll", "Object", "Instantiate");
                }
                
            }
        }
    }
}
#endif
