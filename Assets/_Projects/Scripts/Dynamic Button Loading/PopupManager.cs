using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DoTween namespace

public class PopupManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject popupPanel; // The reusable popup panel
    [SerializeField] private Image popupImage; // Image component in the popup
    
    [Header("Buttons")]
    [SerializeField] private Button closeButton; // Button to close the popup
    [SerializeField] private Button deepLinkButton; // Button to redirect to URL
    [SerializeField] private Button xButton; // Button to redirect to URL
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Audio source for playing clips
    [SerializeField] private AudioClip audioClip; // Audio source for playing clips
    
    [SerializeField] private ButtonDataList buttonDataList; // Reference to the ScriptableObject
    
    [SerializeField] private string commonDeepLinkURL = "https://example.com"; // Common URL for all buttons
    [SerializeField] private float animationDuration = 0.5f; // Duration of the tween

    private RectTransform popupRectTransform;

    [SerializeField]private ARDragZoom ARDragZoom;
    private void Start()
    {
        // Get RectTransform of the popup panel
        popupRectTransform = popupPanel.GetComponent<RectTransform>();

        // Ensure popup panel is initially hidden
        popupPanel.SetActive(false);

        // Add close button listener
        closeButton.onClick.AddListener(ClosePopup);
        xButton.onClick.AddListener(ClosePopup);

        // Add the deep link action once (since all buttons share the same URL)
        deepLinkButton.onClick.RemoveAllListeners();
        deepLinkButton.onClick.AddListener(() => OpenDeepLink());
        
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void OnButtonClicked(int buttonIndex)
    {
        if (ARDragZoom.isDragging)
        {
            return;
        }
        if (buttonIndex < 0 || buttonIndex >= buttonDataList.buttonData.Count)
        {
            Debug.LogError("Invalid button index.");
            return;
        }

        // Get the data for the clicked button
        ButtonData data = buttonDataList.buttonData[buttonIndex];
        
        Debug.Log($"Button Pressed: Index = {buttonIndex}, Image = {data.image.name}");


        // Set the image
        popupImage.sprite = data.image;
        
        
        // Stop the audio and deactivate the specified GameObject
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        

        // Play the audio
        audioSource.clip = data.audioClip;
        audioSource.Play();

        // Show and animate the popup
        ShowPopup();
    }

    private void ShowPopup()
    {
        popupPanel.SetActive(true); // Activate the panel
        popupRectTransform.localScale = Vector3.zero; // Start at 0 scale

        // Scale the popup to full size with DoTween
        popupRectTransform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
    }

    private void ClosePopup()
    {
        // Animate scale to 0 before hiding
        popupRectTransform.DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                popupPanel.SetActive(false); // Hide after animation
                audioSource.Stop(); // Stop audio if playing
            });
    }

    private void OpenDeepLink()
    {
        Application.OpenURL(commonDeepLinkURL); // Open the common URL
    }
}
