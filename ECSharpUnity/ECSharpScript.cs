using ECSharp;
using ECSharp.Time;
using UnityEngine;

/// <summary>
/// ECSharp框架支撑脚本
/// </summary>
public class ECSharpScript : MonoBehaviour
{
    internal static void InitECSharp()
    {
        if(FindObjectOfType<ECSharpScript>() != null)
        {
            return;
        }

        var obj = new GameObject("ECSharpRuntime");
        obj.AddComponent<ECSharpScript>();
        DontDestroyOnLoad(obj);
    }

    private void Start()
    {
        Application.logMessageReceived += HandleLog;

        StartCoroutine(TimeFlowManager.OnUnityUpdate());
    }

    void HandleLog(string logString, string stackTrace, UnityEngine.LogType type)
    {
        if (type == UnityEngine.LogType.Exception)
        {
            // 未捕获的处理异常
            string data = string.Format("{0}\r\n   Message:{1}\r\n   Method:{2}\r\n   StackTrace:\r\n{3}", "", logString, "", stackTrace);
            Log.WriteLog(ECSharp.LogType.FATAL, data);
        }
    }
}
