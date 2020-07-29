using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Main : MonoBehaviour
{
    private AsyncOperation asyncOperation;

    private readonly Stopwatch sw = new Stopwatch();

    public Text timeText;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadScene()
    {
        sw.Start();
        asyncOperation = SceneManager.LoadSceneAsync("VideoDemo", LoadSceneMode.Additive);
        StartCoroutine(TimeTest());
    }

    private IEnumerator TimeTest()
    {
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        yield return null;
        sw.Stop();
        timeText.text = string.Format("加载场景时间: {0} ms", sw.ElapsedMilliseconds);
    }


    // Update is called once per frame
    void Update()
    {
    }
}