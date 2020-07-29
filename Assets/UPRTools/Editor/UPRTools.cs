using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UPRLuaProfiler;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace UPRProfiler
{
    public class UPRTools : EditorWindow
{
    private static bool useLua = false;
    private static bool uprLuaProfiler = false;

    UPRTools()
    {
        this.titleContent = new GUIContent("UPRTools");
    }
    
    [MenuItem("Tool/UPRTools")]
    static void showWindow()
    {
       EditorWindow.GetWindow(typeof(UPRTools));
    }

    [System.Obsolete]
    private void OnGUI()
    {
        string version = Application.unityVersion;
        GUILayout.BeginVertical();

        // draw the title
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("UPRTools");
        
        // draw the version
        GUI.skin.label.fontSize = 12;
        GUI.skin.label.alignment = TextAnchor.LowerCenter;
        GUILayout.Label("Current Version: " + InnerPackageS.packageVersion);
        
        //draw the text
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 12;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;

        doPackage();
        GUILayout.EndVertical();
    }

    void doPackage()
    {
        var setting = UPRToolSetting.Instance;
        var luasetting = LuaDeepProfilerSetting.Instance;
        #region deep function profiler
        GUILayout.Space(10);
        GUILayout.Label("Deep Function Profiler");
        GUILayout.BeginVertical("Box");
        GUILayout.BeginHorizontal();
        setting.loadScene = GUILayout.Toggle(setting.loadScene, "LoadScene         ");
       /*
        setting.loadAsset = GUILayout.Toggle(setting.loadAsset, "LoadAsset ");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        setting.loadAssetBundle = GUILayout.Toggle(setting.loadAssetBundle, "LoadAssetBundle ");
        setting.instantiate = GUILayout.Toggle(setting.instantiate, "Instantiate ");
       */
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        #endregion



        #region  deep lua profiler
        GUILayout.Space(10);
        GUILayout.Label("Deep Lua Profiler");
        GUILayout.BeginVertical("Box");
        
        setting.enableMonoProfiler = GUILayout.Toggle(setting.enableMonoProfiler, "Enable Deep Mono");
        if (setting.enableMonoProfiler)
        {
            InjectMethods.Recompile("MIKU_RECOMPILE",true);
            luasetting.isDeepMonoProfiler = true;
            EditorGUILayout.HelpBox("Package will inject the necessary code to collect Mono information. This will cause some performance loss.", MessageType.Warning);
            EditorGUILayout.HelpBox("This function doesn't support ios devices currently.", MessageType.Warning);
            if (!uprLuaProfiler)
            {
                uprLuaProfiler = true;
                InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
            }
        }
        else
        {
            luasetting.isDeepMonoProfiler = false;
            InjectMethods.Recompile("MIKU_RECOMPILE",false);
            if (!setting.enableLuaProfiler && uprLuaProfiler)
            {
                uprLuaProfiler = false;
                InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
            }
        }
        
        setting.enableLuaProfiler = GUILayout.Toggle(setting.enableLuaProfiler, "Enable Lua");
        if (setting.enableLuaProfiler)
        {
            luasetting.isDeepLuaProfiler = true;
            EditorGUILayout.HelpBox("Package will load some simple lua function to collect lua information. You can easy control whether to send data on UPR website.", MessageType.Warning);
            if (!useLua)
            {
                string[] dir =
                    Directory.GetDirectories(Application.dataPath, "*LuaProfiler*", SearchOption.TopDirectoryOnly);
                
                if (dir.Length > 0)
                {
                    useLua = false;
                    EditorGUILayout.HelpBox("LuaProfiler is Find in the directory", MessageType.Warning);
                }
                else
                {
                    useLua = true;
                }
                InjectMethods.Recompile("USE_LUA", useLua);
            }
            if (!uprLuaProfiler)
            {
                uprLuaProfiler = true;
                InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
            }
        }
        else
        {
            luasetting.isDeepLuaProfiler = false;
            if (useLua)
            {
                useLua = false;
                InjectMethods.Recompile("USE_LUA", false);
            }

            if (!setting.enableMonoProfiler && uprLuaProfiler)
            {
                uprLuaProfiler = false;
                InjectMethods.Recompile("UPR_LUA_PROFILER", uprLuaProfiler);
            }
        }
        
        GUILayout.EndVertical();
        #endregion
   
    }
}
#endif
}
