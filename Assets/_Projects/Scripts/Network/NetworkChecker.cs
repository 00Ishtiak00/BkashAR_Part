using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace _Projects.Scripts.Network
{
    public class NetworkChecker : MonoBehaviour
    {
        public static event System.Action<bool> OnConnectionStatusChanged;  // Event to notify status changes

        [Header("Network Settings")]
        [SerializeField] private float checkInterval = 5f;  // Time in seconds between connection checks
        [SerializeField] private float timeout = 3f;        // Timeout threshold for connection check

        private bool isConnected;

        private void Start()
        {
            StartCoroutine(CheckInternetConnection());
        }

        private IEnumerator CheckInternetConnection()
        {
            while (true)
            {
                using (UnityWebRequest request = UnityWebRequest.Head("https://www.google.com"))
                {
                    request.timeout = (int)timeout;
                    yield return request.SendWebRequest();

                    bool connectionStatus = request.result == UnityWebRequest.Result.Success;

                    if (connectionStatus != isConnected)
                    {
                        isConnected = connectionStatus;
                        OnConnectionStatusChanged?.Invoke(isConnected);  // Notify status change
                    }
                }

                yield return new WaitForSeconds(checkInterval);
            }
        }
    }
}