using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour
{
    [SerializeField] private PopupManager popupManager;
    [SerializeField] private Button button;
    [SerializeField] private int buttonIndex; // Index for this button
    [SerializeField] private GameObject _audioSourceParentGO; // Parent GameObject containing all AudioSources
    [SerializeField] private AudioSource _audioSource; // Parent GameObject containing all AudioSources
    
    private void Start()
    {
        //Loop through all the gameobjects in the parent GameObject
        foreach (Transform child in _audioSourceParentGO.transform)
        {
            //Check if the child has an AudioSource component
            if (child.TryGetComponent(out AudioSource audioSource))
            {
                if(child.name == buttonIndex.ToString())
                {
                    _audioSource = audioSource;
                    break;
                }
            }
        }
        
        button.onClick.AddListener(() => popupManager.OnButtonClicked(buttonIndex, _audioSource));
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}