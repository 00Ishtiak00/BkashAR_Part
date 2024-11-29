using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Canvas canvas;
    public TMP_InputField inputField;
    
    public void SetPosition()
    {
        RectTransform rec = canvas.GetComponent<RectTransform>();
        rec.localPosition = new Vector3(rec.localPosition.x, rec.localPosition.y, float.Parse(inputField.text));
    }
}
