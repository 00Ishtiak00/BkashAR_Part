using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Projects.Scripts.Network
{
    public class NetworkUIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject slowInternetUI;  // UI to show when internet is slow or unavailable
        [SerializeField] private TMP_Text statusText;            // Optional text to display connection status

        private void OnEnable()
        {
            NetworkChecker.OnConnectionStatusChanged += UpdateUI;
        }

        private void OnDisable()
        {
            NetworkChecker.OnConnectionStatusChanged -= UpdateUI;
        }

        private void UpdateUI(bool isConnected)
        {
            if (isConnected)
            {
                HideSlowInternetUI();
            }
            else
            {
                ShowSlowInternetUI("Slow or no internet connection.");
            }
        }

        private void ShowSlowInternetUI(string message)
        {
            if (slowInternetUI != null)
                slowInternetUI.SetActive(true);

            if (statusText != null)
                statusText.text = message;
        }

        private void HideSlowInternetUI()
        {
            if (slowInternetUI != null)
                slowInternetUI.SetActive(false);
        }
    }
}