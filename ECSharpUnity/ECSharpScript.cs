using ECSharp.Time;
using UnityEngine;

/// <summary>
/// ECSharp¿ò¼ÜÖ§³Å½Å±¾
/// </summary>
public class ECSharpScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TimeFlowManager.OnUnityUpdate());
    }
}
