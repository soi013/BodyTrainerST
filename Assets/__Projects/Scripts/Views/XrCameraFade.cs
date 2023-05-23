using CameraFading;
using UnityEngine;

public class XrCameraFade : MonoBehaviour
{
    void Start()
    {
        //CameraFade.Out(2);
    }
    internal void ChangeFade(bool enableFade)
    {
        if (enableFade)
            CameraFade.Out(2);
        else
            CameraFade.In(2);
    }
}
