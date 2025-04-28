using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace NnUtils.Scripts.Editor
{
    public class EditorShortcuts : MonoBehaviour
    {
        [Shortcut("NnUtils/New Script", KeyCode.S, ShortcutModifiers.Control)]
        private static void NewScript() =>
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                "Assets/NnUtils/Scripts/Editor/NnScriptTemplate.txt", "Script.cs");
    }
}
