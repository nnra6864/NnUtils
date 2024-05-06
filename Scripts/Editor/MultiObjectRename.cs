using UnityEditor;
using UnityEngine;

namespace NnUtils.Scripts.Editor
{
    public class MultiObjectRename : EditorWindow
    {
        private static MultiObjectRename _mor;
        private string _newName = "Object Name";

        [MenuItem("GameObject/NnUtils/Rename Selected Objects", false, 0)]
        private static void RenameSelectedObjects()
        {
            if (_mor != null || Selection.objects.Length < 1) return;
            _mor = CreateInstance<MultiObjectRename>();
            _mor.ShowUtility();
        }

        private void OnGUI()
        {
            _newName = EditorGUILayout.TextField("New Name: ", _newName);
            if (GUILayout.Button("Rename"))
            {
                Undo.RecordObjects(Selection.objects, "Rename Objects");
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Selection.objects[i].name = $"{_newName} {i + 1}";
                    EditorUtility.SetDirty(Selection.objects[i]);
                }
                for (int i = 0; i < Selection.assetGUIDs.Length; i++)
                    AssetDatabase.RenameAsset(AssetDatabase.AssetPathToGUID(Selection.assetGUIDs[i]), $"{_newName} {i + 1}");
            }
            if (GUILayout.Button("Close")) Close();
        }
    }
}