using ECSharp.Time;
using UnityEngine;

/// <summary>
/// ECSharp���֧�Žű�
/// </summary>
public class ECSharpScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TimeFlowManager.OnUnityUpdate());
    }
}
