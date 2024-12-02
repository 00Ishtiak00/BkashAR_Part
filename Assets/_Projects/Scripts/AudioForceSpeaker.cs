using UnityEngine;
using System.Runtime.InteropServices;

public class AudioForceSpeaker : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ForceAudioToSpeaker();
#endif

    public void InitializeAudioOnIOS()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ForceAudioToSpeaker();
#else
        Debug.Log("Audio initialization only available in WebGL build and on iOS.");
#endif
    }
}