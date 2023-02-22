using ECSharp.Time;
using UnityEngine;

public class ECSharpScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
#if UNITY_WEBGL
        TimeFlowManager.OnUnityUpdate();
#endif
    }
}
