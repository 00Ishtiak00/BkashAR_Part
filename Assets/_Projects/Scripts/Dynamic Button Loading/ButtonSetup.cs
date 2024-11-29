using UnityEngine;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour
{
    [SerializeField] private PopupManager popupManager;
    [SerializeField] private Button button;
    [SerializeField] private int buttonIndex; // Index for this button

    private void Start()
    {
        button.onClick.AddListener(() => popupManager.OnButtonClicked(buttonIndex));
    }
}