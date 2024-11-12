using UnityEngine;
using UnityEngine.Video;

public class PlatformSpecificVideo : MonoBehaviour
{
    public string url;

    void Awake()
    {
        #if UNITY_WEBGL
            GetComponent<VideoPlayer>().source = VideoSource.Url;
            GetComponent<VideoPlayer>().url = url;
        #endif
    }
}
