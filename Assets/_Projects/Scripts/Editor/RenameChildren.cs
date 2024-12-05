using UnityEngine;
using UnityEditor;

public class RenameChildren : EditorWindow
{
    private GameObject parentObject;

    [MenuItem("Tools/Rename Children")]
    private static void ShowWindow()
    {
        GetWindow<RenameChildren>("Rename Children");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rename All Child GameObjects", EditorStyles.boldLabel);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        if (GUILayout.Button("Rename Children"))
        {
            if (parentObject != null)
            {
                RenameChildObjects(parentObject);
            }
            else
            {
                Debug.LogWarning("Please assign a parent GameObject.");
            }
        }
    }

    private static void RenameChildObjects(GameObject parent)
    {
        int counter = 0;
        foreach (Transform child in parent.transform)
        {
            Undo.RecordObject(child.gameObject, "Rename Child Object");
            child.name = counter.ToString();
            counter++;
        }
        Debug.Log($"Renamed {counter} child GameObjects.");
    }
}