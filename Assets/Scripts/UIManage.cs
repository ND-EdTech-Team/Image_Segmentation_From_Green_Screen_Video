using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManage : MonoBehaviour
{
    public VideoPlayer[] VideoPlayers;

    public Text fileSizeText;

    public Text fileExtensionText;

    public Text tipsText;

    private string _url;

    public GameObject greenCurtainToggle;

    private Dictionary<VideoPlayer, Vector2[]> oldUvs;

    private Dictionary<VideoPlayer, Vector2[]> newUvs;

    private Stopwatch sw = new Stopwatch();

    public Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        oldUvs = new Dictionary<VideoPlayer, Vector2[]>();
        newUvs = new Dictionary<VideoPlayer, Vector2[]>();
        foreach (var mediaPlayerCtrl in VideoPlayers)
        {
            Vector2[] uv = mediaPlayerCtrl.GetComponent<MeshFilter>().mesh.uv;
            oldUvs.Add(mediaPlayerCtrl, uv);

            Vector2[] vec2UVs = mediaPlayerCtrl.GetComponent<MeshFilter>().mesh.uv;

            for (int i = 0; i < vec2UVs.Length; i++)
            {
                vec2UVs[i] = new Vector2(vec2UVs[i].x, 1.0f - vec2UVs[i].y);
            }

            newUvs.Add(mediaPlayerCtrl, vec2UVs);
        }
    }

    private IEnumerator TimeTest()
    {
        while (!VideoPlayers[0].isPrepared)
        {
            yield return null;
        }

        yield return null;
        sw.Stop();
        timeText.text = string.Format("加载视频时间: {0} ms", sw.ElapsedMilliseconds);
    }

    public void Load()
    {
        var path = FileBrowser.OpenFileDialog("视频文件\0*.mp4;*.avi;*.wav;*.rmvb;*.3gp;*.asf;*.wma;*.wmv;*.mov;*.flv");
        if (string.IsNullOrEmpty(path))
            return;
        if (!File.Exists(path))
            return;
        _url = path;


        sw.Reset();
        sw.Start();
        StartCoroutine(TimeTest());
     

        foreach (var video in VideoPlayers)
        {
            if (video != null && video.gameObject.activeInHierarchy && video.enabled)
            {
                video.url = path;
                video.Play();
            }
        }

        PrintFileVersionInfo(path);
        Resources.UnloadUnusedAssets();
    }

    public void GreenCurtain(bool isOn)
    {
        var active = isOn && greenCurtainToggle.GetComponent<Toggle>()
            .isOn;
    }

    public void VideoPlayer(bool isOn)
    {
        if (!isOn)
            return;
        SetPlayMode(true);
    }

    public void EasyMovie(bool isOn)
    {
        if (!isOn)
            return;
        SetPlayMode(false);
    }

    private void SetPlayMode(bool isVideoPlayer)
    {
        foreach (var mediaPlayerCtrl in VideoPlayers)
        {
            if (!isVideoPlayer)
            {
                mediaPlayerCtrl.GetComponent<MeshFilter>().mesh.uv = newUvs[mediaPlayerCtrl];
            }
            else
            {
                mediaPlayerCtrl.GetComponent<MeshFilter>().mesh.uv = oldUvs[mediaPlayerCtrl];
            }

            mediaPlayerCtrl.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = null;

            mediaPlayerCtrl.enabled = !isVideoPlayer;
        }

        foreach (var video in VideoPlayers)
        {
            video.enabled = isVideoPlayer;
            if (string.IsNullOrEmpty(_url) || !isVideoPlayer)
            {
                continue;
            }

            video.url = _url;
            video.Play();
        }

        tipsText.gameObject.SetActive(isVideoPlayer);
        greenCurtainToggle.SetActive(isVideoPlayer);
        GreenCurtain(isVideoPlayer);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    

    public Text urlText;

    public void OpenUrl()
    {
        Application.OpenURL(urlText.text.Split('：')[1]);
    }

    private void PrintFileVersionInfo(string path)
    {
        var fileInfo = new FileInfo(path);
        System.Diagnostics.FileVersionInfo info = System.Diagnostics.FileVersionInfo.GetVersionInfo(path);
        fileSizeText.text = "视频大小：" + (fileInfo.Length / 1024.0 / 1024.0f).ToString("F2") + "M";
        fileExtensionText.text = "文件格式：" + Path.GetExtension(path);
    }
}