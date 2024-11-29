using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonDataList", menuName = "ScriptableObjects/ButtonDataList", order = 1)]
public class ButtonDataList : ScriptableObject
{
    public List<ButtonData> buttonData = new List<ButtonData>();
}