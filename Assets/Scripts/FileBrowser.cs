using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// 文件管理器
/// </summary>
public static class FileBrowser
{
    public const string ImageFile = "图像文件(*.png,*.jpg,*.tga)\0*.png;*.jpg;*.jpeg;*.tga";

    /// <summary>
    /// 打开本地文件选择器
    /// </summary>
    /// <param name="filter">文件类型</param>
    /// <param name="title">选择器标题名称</param>
    /// <returns>保存路径</returns>
    public static string OpenFileDialog(string filter, string title = "打开")
    {
#if UNITY_EDITOR
        //EDITOR的过滤规则是数组，需要按'\0'(NULL)分割为数组
        string[] filters = filter.Split('\0');
        filters[1] = filters[1].Replace("*.", string.Empty).Replace(";", ",");
        return UnityEditor.EditorUtility.OpenFilePanelWithFilters(title, string.Empty, filters);
#else
        string currentDirectory = Environment.CurrentDirectory;
        OpenFileName openFileName = new OpenFileName(filter, title, string.Empty);
        if (GetOpenFileName(openFileName))
        {
            Environment.CurrentDirectory = currentDirectory;
            return openFileName.file;
        }
        else
        {
            Environment.CurrentDirectory = currentDirectory;
            return string.Empty;
        }
#endif
    }

    /// <summary>
    /// 获取有效文件名
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetValidFilename(string fileName)
    {
        StringBuilder rBuilder = new StringBuilder(fileName);
        foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
        {
            rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
        }

        return rBuilder.ToString();
    }

    /// <summary>
    /// 保存文件夹
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="folder">目录</param>
    /// <param name="defaultName">默认文件名</param>
    /// <returns>保存路径</returns>
    public static string SaveFolderPanel(string title = "选择保存目录", string folder = "", string defaultName = "")
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.SaveFolderPanel(title, folder, defaultName);
#else
        OpenFolder openFolder = new OpenFolder();
        openFolder.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区    
        openFolder.lpszTitle = title;// 标题    
        //openFolder.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // 新的样式,带编辑框
        openFolder.ulFlags = 0x0010 | 0x40; //新的样式,带编辑框
        IntPtr pidlPtr = SHBrowseForFolder(openFolder);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);
        return fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));
#endif
    }

    //链接指定系统函数       打开文件对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName openFileName);

    private static bool OpenFileDialog([In, Out] OpenFileName openFileName)
    {
        return GetOpenFileName(openFileName);
    }

    //链接指定系统函数        另存为对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenFileName openFileName);

    private static bool SaveFileDialog([In, Out] OpenFileName openFileName)
    {
        return GetSaveFileName(openFileName);
    }

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In, Out] OpenFolder ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public string filter = null;
    public string customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public string file = null;
    public int maxFile = 0;
    public string fileTitle = null;
    public int maxFileTitle = 0;
    public string initialDir = null;
    public string title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public string defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public string templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;

    public OpenFileName(string filter, string title, string file)
    {
        structSize = Marshal.SizeOf(this);
        this.filter = filter;
        if (!string.IsNullOrEmpty(file))
        {
            this.file = file;
        }
        else
        {
            this.file = new string(new char[256]);
        }

        maxFile = 256;
        fileTitle = new string(new char[128]);
        maxFileTitle = fileTitle.Length;
        this.title = title;
        flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFolder
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}