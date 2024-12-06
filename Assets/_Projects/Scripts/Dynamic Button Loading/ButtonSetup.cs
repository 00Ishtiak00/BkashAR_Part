using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonSetup : MonoBehaviour
{
    [SerializeField] private PopupManager popupManager;
    [SerializeField] private Button button;
    [SerializeField] private int buttonIndex; // Index for this button
    [SerializeField] private GameObject _audioSourceParentGO; // Parent GameObject containing all AudioSources
    [SerializeField] private GameObject _audioSourceGO; // Parent GameObject containing all AudioSources
    
    private void Start()
    {
        //Loop through all the gameobjects in the parent GameObject
        foreach (Transform child in _audioSourceParentGO.transform)
        {
            if(child.name == buttonIndex.ToString())
            {
                _audioSourceGO = child.gameObject;
                break;
            }
        }
        
        button.onClick.AddListener(() => popupManager.OnButtonClicked(buttonIndex, _audioSourceGO));
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}