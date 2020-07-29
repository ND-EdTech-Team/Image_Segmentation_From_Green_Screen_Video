using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Text;
using System.Threading;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
#if UNITY_2018_2_OR_NEWER
using Unity.Collections;
#endif

namespace UPRProfiler
{
    public class InnerPackageS : MonoBehaviour
    {
        private static int pid = 0;
        private static AndroidJavaClass UnityPlayer;
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaObject memoryManager;
        private static AndroidJavaObject[] memoryInfoArray;
        private static AndroidJavaObject memoryInfo;
        private static AndroidJavaObject intentFilter;
        private static AndroidJavaObject intent;
        private static string[] funcName = new string[] { "getTotalPss", "getTotalSwappablePss", "getTotalPrivateDirty", "getTotalSharedDirty", "getTotalPrivateClean", "getTotalSharedClean" };
        private static object[] tempeartureModel = new object[] { "temperature", 0 };
        private static int width;
        private static int height;
        private static byte[] rawBytes;
        public static WaitForSeconds waitOneSeconds = new WaitForSeconds(4);
        private static Process cprocess;
        private static int cpuCores = 0;
        public static string packageVersion = "0.6.3";
        private static string cpuString = "";
        public static int screenCnt = 0;
        public static int memoryCnt = 0;
        public static GUIStyle fontStyle;
#if UNITY_2018_2_OR_NEWER
        private static NativeArray<byte> nativeRawBytes;
#endif


        void Start()
        {
            height = Screen.height;
            width = Screen.width;
            fontStyle = new GUIStyle();
            fontStyle.normal.background = null;    //设置背景填充
            fontStyle.normal.textColor= new Color(1,0,0);   //设置字体颜色
            fontStyle.fontSize = 20;

#if !UNITY_2018_2_OR_NEWER
        rawBytes = new byte[0];
#endif
            StartCoroutine(GetScreenShot());
           
            if (Application.platform == RuntimePlatform.Android)
            {
               
                UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
                intentFilter = new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED");
                intent = currentActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, intentFilter });
                pid = process.CallStatic<int>("myPid");
                if (pid > 0)
                {
                    memoryManager = currentActivity.Call<AndroidJavaObject>("getSystemService", new AndroidJavaObject("java.lang.String", "activity"));
                    memoryInfoArray = memoryManager.Call<AndroidJavaObject[]>("getProcessMemoryInfo", new int[] { pid });
                    memoryInfo = memoryInfoArray[0];
                    cpuCores = SystemInfo.processorCount;
                    cprocess = new Process();
                    StartCoroutine(GetSystemMemory());
                    Thread thread = new Thread(new ThreadStart(CpuThread));
                    thread.Start();
                }
                else
                {
                    Debug.LogError("Get Device Pid Error");
                }
            }

        }

        IEnumerator GetScreenShot()
        {
            WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
            Texture2D shot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Rect area = new Rect(0, 0, Screen.width, Screen.height);
            screenCnt = 0;
            while (true)
            {
                yield return waitForEndOfFrame;
                Profiler.BeginSample("Profiler.ScreenShotCoroutine");
                if (NetworkServer.isConnected && NetworkServer.enableScreenShot && !NetworkServer.screenFlag)
                {
                    if (width != Screen.width)
                    {
                        shot.Resize(Screen.width, Screen.height);
                        width = Screen.width;
                        height = Screen.height;
                        area = new Rect(0, 0, Screen.width, Screen.height);
                        yield return waitForEndOfFrame;
                    }

                    try
                    {
                        shot.ReadPixels(area, 0, 0, false);
                        shot.Apply();
#if UNITY_2018_2_OR_NEWER
                        nativeRawBytes = shot.GetRawTextureData<byte>();
                        NetworkServer.SendMessage(nativeRawBytes, 0, width, height);
#else
                    rawBytes = shot.GetRawTextureData();
                    NetworkServer.SendMessage(rawBytes, 0, width, height);
#endif
                        NetworkServer.screenFlag = true;
                        screenCnt++;
                    }
                    catch (Exception e)
                    {
                        NetworkServer.screenFlag = false;
                        Debug.LogError("[PACKAGE] Screenshot Error " + e);
                    }

                }
                Profiler.EndSample();
                if (NetworkServer.isConnected && !NetworkServer.sendDeviceInfo)
                {
                    try
                    {
                        GetSystemDevice();
                        NetworkServer.sendDeviceInfo = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[PACKAGE] Send SystemDevice Error " + e);
                    }
                }
                
                yield return waitOneSeconds;
            }
        }

        IEnumerator GetSystemMemory()
        {
            WaitForSeconds waitOneSeconds = new WaitForSeconds(3);
            memoryCnt = 0;
            while (true)
            {

                Profiler.BeginSample("Profiler.GetMemoryInfo");
                if (NetworkServer.isConnected)
                {
                    try
                    {
                        GetPSS();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("[PACKAGE] Send PSS Error " + ex);
                    }

                    memoryCnt++;
                }
                Profiler.EndSample();  
                yield return waitOneSeconds;
            }
        }
        

        public static void GetPSS()
        {
            StringBuilder meminfo = new StringBuilder();
            meminfo.Append("{");
            for (int i = 0; i < funcName.Length; i++)
            {
                if (i > 0)
                    meminfo.Append(",");
                meminfo.AppendFormat("\"{0}\":\"{1}\"", funcName[i], memoryInfo.Call<int>(funcName[i]).ToString());
            }
            meminfo.AppendFormat(", \"{0}\":\"{1}\"", "battery", SystemInfo.batteryLevel);
            meminfo.AppendFormat(", \"{0}\":\"{1}\"", "cpuTemp", intent.Call<int>("getIntExtra", tempeartureModel).ToString());
            meminfo.AppendFormat(", \"{0}\":\"{1}\"", "cpuUsage", cpuString);
            meminfo.Append("}");
            NetworkServer.SendMessage(Encoding.ASCII.GetBytes(meminfo.ToString()), 1, width, height);
        }

        public static void GetSystemDevice()
        {
            StringBuilder deviceInfo = new StringBuilder();
            deviceInfo.AppendFormat("{0}:{1}", "systemBrand", SystemInfo.deviceModel);
            deviceInfo.AppendFormat("& {0}:{1}", "systemTotalRam", SystemInfo.systemMemorySize.ToString() + "MB");
            deviceInfo.AppendFormat("& {0}:{1}", "systemMaxCpuFreq", SystemInfo.processorFrequency + "MHZ");
            deviceInfo.AppendFormat("& {0}:{1}", "packageVersion", packageVersion);
            
            deviceInfo.AppendFormat("& {0}:{1}", "operatingSystem", SystemInfo.operatingSystem);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsDeviceID", SystemInfo.graphicsDeviceID);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsDeviceName", SystemInfo.graphicsDeviceName);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsDeviceType", SystemInfo.graphicsDeviceType);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsMemorySize", SystemInfo.graphicsMemorySize);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsMultiThreaded", SystemInfo.graphicsMultiThreaded);
            deviceInfo.AppendFormat("& {0}:{1}", "graphicsShaderLevel", SystemInfo.graphicsShaderLevel);
            deviceInfo.AppendFormat("& {0}:{1}", "maxTextureSize", SystemInfo.maxTextureSize);
            deviceInfo.AppendFormat("& {0}:{1}", "npotSupport", SystemInfo.npotSupport);
            deviceInfo.AppendFormat("& {0}:{1}", "cpuCores", SystemInfo.processorCount);
            deviceInfo.AppendFormat("& {0}:{1}", "resolution", Screen.width + "*" + Screen.height);
            deviceInfo.AppendFormat("& {0}:{1}", "processorType", SystemInfo.processorType);
            NetworkServer.SendMessage(Encoding.ASCII.GetBytes(deviceInfo.ToString()), 2, width, height);
        }


        private void CpuThread()
        {
            while (true)
            {
                if (NetworkServer.isConnected)
                {
                    cpuString = GetCpuUsage(pid);
                }
                Thread.Sleep(3000);
            }
        }
        private static string GetCpuUsage(int pid)
        {
            string result = "";
            cprocess.StartInfo.FileName = "sh";
            cprocess.StartInfo.Arguments = "-c top | grep " + pid;
            cprocess.StartInfo.UseShellExecute = false;
            cprocess.StartInfo.RedirectStandardOutput = true;
            cprocess.StartInfo.RedirectStandardError = true;
            try
            {
                cprocess.Start();
                string strOutput = "S";
                for (int i = 0; i < 6; i++)
                    strOutput = cprocess.StandardOutput.ReadLine();
                strOutput = strOutput.Split('S')[1];
                int index = strOutput.IndexOf("   ", StringComparison.Ordinal);
                float cpuUsage = float.Parse(strOutput.Substring(1, index));
                result = (cpuUsage / cpuCores).ToString(CultureInfo.InvariantCulture);
                cprocess.WaitForExit();
            }
            catch (Exception e)
            {
                Debug.LogError("Package GetCPUUsage Error " + e);
            }
            return result;
        }
    }

}
