using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace NnUtils.Scripts.Editor
{
    public class EditorShortcuts : MonoBehaviour
    {
        [Shortcut("NewNnScript", KeyCode.S, ShortcutModifiers.Control | ShortcutModifiers.Alt)]
        private static void NewScript() =>
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/NnUtils/Scripts/Editor/NnScriptTemplate.txt", "NnScript.cs");
    }
}
