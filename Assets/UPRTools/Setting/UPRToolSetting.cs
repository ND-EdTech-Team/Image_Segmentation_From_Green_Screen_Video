﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace  UPRProfiler
{
     
    public class UPRToolSetting
    {

        private bool m_loadScene = false;
        private bool m_loadAsset = false;
        private bool m_loadAssetBundle = false;
        private bool m_instantiate = false;

        private bool m_enableLuaProfiler = false;
        private bool m_enableMonoProfiler = false;

        private static UPRToolSetting instance;
        public static UPRToolSetting MakeInstance()
        {
            instance = UPRToolSetting.Load();
            return instance;
        }
        public static UPRToolSetting Instance
        {
            get
            {
    #if UNITY_EDITOR
                if (instance == null)
                {
                    instance = MakeInstance();
                }
    #endif
                return instance;
            }
        }

        public bool enableLuaProfiler
        {
            get
            {
                return m_enableLuaProfiler;
            }
            set
            {
                if (this.m_enableLuaProfiler != value)
                {
                    this.m_enableLuaProfiler = value;
                    this.Save();
                }
            }
        }

        public bool enableMonoProfiler
        {
            get
            {
                return m_enableMonoProfiler;
            }
            set
            {
                if (this.m_enableMonoProfiler != value)
                {
                    m_enableMonoProfiler = value;
                    this.Save();
                }
            }
        }

        public bool loadScene
        {
            get
            {
                return m_loadScene;
            }
            set
            {
                if (this.m_loadScene != value)
                {
                    this.m_loadScene = value;
                    this.Save();
                }
            }
        }
        public bool loadAsset
        {
            get
            {
                return m_loadAsset;
            }
            set
            {
                if (this.m_loadAsset != value)
                {
                    this.m_loadAsset = value;
                    this.Save();
                }
            }
        }
        public bool loadAssetBundle
        {
            get
            {
                return m_loadAssetBundle;
            }
            set
            {
                if (this.m_loadAssetBundle != value)
                {
                    this.m_loadAssetBundle = value;
                    this.Save();
                }
            }
        }
        public bool instantiate
        {
            get
            {
                return m_instantiate;
            }
            set
            {
                if (this.m_instantiate != value)
                {
                    this.m_instantiate = value;
                    this.Save();
                }
            }
        }

        public static UPRToolSetting Load()
        {
            UPRToolSetting uprToolSetting = new UPRToolSetting();
            byte[] datas = null;
    #if UNITY_EDITOR
            string text = "Assets/UPRTools/Resources/UPRToolSettings.bytes";
            if (!File.Exists(text))
            {
                uprToolSetting.Save();
            }
            datas = File.ReadAllBytes(text);
    #else
                TextAsset textAsset = null;
                string path = Application.persistentDataPath + "/UPRToolSettings.bytes";
                if (File.Exists(path))
                {
                    datas = File.ReadAllBytes(path);
                }
                else
                {
                    textAsset = Resources.Load<TextAsset>("UPRToolSettings");
                    datas = textAsset != null ? textAsset.bytes : null;
                }
    #endif

            if (datas != null)
            {
                MemoryStream memoryStream = new MemoryStream(datas);
                try
                {
                    BinaryReader binaryReader = new BinaryReader(memoryStream);
                    uprToolSetting.m_enableLuaProfiler = binaryReader.ReadBoolean();
                    uprToolSetting.m_enableMonoProfiler = binaryReader.ReadBoolean();
                    uprToolSetting.m_loadScene = binaryReader.ReadBoolean();
                    uprToolSetting.m_loadAsset = binaryReader.ReadBoolean();
                    uprToolSetting.m_loadAssetBundle = binaryReader.ReadBoolean();
                    uprToolSetting.m_instantiate = binaryReader.ReadBoolean();
                    binaryReader.Close();
                }
                catch
                {
    #if UNITY_EDITOR
                    memoryStream.Dispose();
                    File.Delete(text);
                    return UPRToolSetting.Load();
    #endif
                }
            }
            else
            {
                uprToolSetting.Save();
            }

    #if !UNITY_EDITOR
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, datas);
                }
                if (textAsset != null)
                {
                    Resources.UnloadAsset(textAsset);
                }
            
    #endif

            return uprToolSetting;
        }

        public void Save()
        {
    #if UNITY_EDITOR
            string text = "Assets/UPRTools/Resources/UPRToolSettings.bytes";

            if (!Directory.Exists("Assets/UPRTools/Resources"))
            {
                Directory.CreateDirectory("Assets/UPRTools/Resources");
            }

            FileStream output = new FileStream(text, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(output);
            binaryWriter.Write(this.m_enableLuaProfiler);
            binaryWriter.Write(this.m_enableMonoProfiler);
            binaryWriter.Write(this.m_loadScene);
            binaryWriter.Write(this.m_loadAsset);
            binaryWriter.Write(this.m_loadAssetBundle);
            binaryWriter.Write(this.m_instantiate);

            output.Flush();
            binaryWriter.Close();
    #endif
        }
    }
   
}