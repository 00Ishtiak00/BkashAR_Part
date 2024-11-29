using System.Collections.Generic;
using UnityEngine;

namespace _Projects.Scripts.Managers
{
    public class AudioCacheManager : MonoBehaviour
    {
        private Dictionary<string, float> clipUsageTimes = new Dictionary<string, float>();
        private float cacheExpiryTime = 300f; // Cache expiry in seconds

        private void Update()
        {
            ClearExpiredCache();
        }

        private void ClearExpiredCache()
        {
            float currentTime = Time.time;
            List<string> clipsToRemove = new List<string>();

            foreach (var kvp in clipUsageTimes)
            {
                if (currentTime - kvp.Value > cacheExpiryTime)
                {
                    clipsToRemove.Add(kvp.Key);
                }
            }

            foreach (string clipName in clipsToRemove)
            {
                if (AudioManager.instance.RemoveAudioClipFromCache(clipName))
                {
                    clipUsageTimes.Remove(clipName);
                    Debug.Log($"Audio clip '{clipName}' removed from cache.");
                }
            }
        }

        public void UpdateClipUsageTime(string clipName)
        {
            clipUsageTimes[clipName] = Time.time;
        }
    }
}