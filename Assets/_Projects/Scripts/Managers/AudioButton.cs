using UnityEngine;
using UnityEngine.UI;

namespace _Projects.Scripts.Managers
{
    [RequireComponent(typeof(Button))]
    public class AudioButton : MonoBehaviour
    {
        public string clipName; // Set this to the corresponding audio clip name
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Button button = GetComponent<Button>();
            button.onClick.AddListener(PlayAudioClip);
        }

        private async void PlayAudioClip()
        {
            if (string.IsNullOrEmpty(clipName))
            {
                Debug.LogWarning("Clip name is not set for this button.");
                return;
            }
        
            await AudioManager.instance.PlayAudio(clipName, audioSource);
        }
    }
}
