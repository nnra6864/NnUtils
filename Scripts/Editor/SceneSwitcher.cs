#if UNITY_EDITOR

// Original https://gist.github.com/alexanderameye/c1f99c6b84162697beedc8606027ed9c
// Edited by nnra

namespace NnUtils.Scripts.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Overlays;
    using UnityEditor.SceneManagement;
    using UnityEditor.Toolbars;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UIElements;

    public static class SceneSwitcher
    {
        public static readonly List<string> ScenePaths = new();

        public static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        public static void LoadScenes()
        {
            // Clear scenes
            ScenePaths.Clear();

            // Find all scenes in the Assets folder
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

            foreach (var sceneGuid in sceneGuids)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));
                ScenePaths.Add(scenePath);
            }
        }
    }

    [Icon("d_SceneAsset Icon")]
    [Overlay(typeof(SceneView), OverlayID, "Scene Switcher")]
    public class SceneSwitcherToolbarOverlay : ToolbarOverlay
    {
        public const string OverlayID = "scene-switcher-overlay";
        private SceneSwitcherToolbarOverlay() : base(SceneDropdown.ID) { }

        public override void OnCreated()
        {
            // Load the scenes when the toolbar overlay is initially created
            SceneSwitcher.LoadScenes();

            // Subscribe to the event where scene assets were potentially modified
            EditorApplication.projectChanged += OnProjectChanged;
        }

        // Called when an Overlay is about to be destroyed.
        // Usually this corresponds to the EditorWindow in which this Overlay resides closing.
        // (Scene View in this case)
        // Unsubscribe from the event where scene assets were potentially modified
        public override void OnWillBeDestroyed() => EditorApplication.projectChanged -= OnProjectChanged;

        // Reload the scenes whenever scene assets were potentially modified
        private void OnProjectChanged() => SceneSwitcher.LoadScenes();
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    public class SceneDropdown : EditorToolbarDropdown
    {
        public const string ID = SceneSwitcherToolbarOverlay.OverlayID + "/scene-dropdown";
        private const string Tooltip = "Switch scene.";

        public SceneDropdown()
        {
            var content = EditorGUIUtility.TrTextContentWithIcon(
                SceneManager.GetActiveScene().name, Tooltip, "d_SceneAsset Icon");
            text = content.text;
            tooltip = content.tooltip;
            icon = content.image as Texture2D;

            // Hacky: the text element is the second one here so we can set the padding
            //        but this is not really robust I think
            ElementAt(1).style.paddingLeft = 5;
            ElementAt(1).style.paddingRight = 5;

            clicked += ToggleDropdown;

            // Keep track of panel events
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected virtual void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // Subscribe to the event where the play mode has changed
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // Subscribe to the event where scene assets were potentially modified
            EditorApplication.projectChanged += OnProjectChanged;

            // Subscribe to the event where a scene has been opened
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        protected virtual void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // Unsubscribe from the event where the play mode has changed
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            // Unsubscribe from the event where scene assets were potentially modified
            EditorApplication.projectChanged -= OnProjectChanged;

            // Unsubscribe from the event where a scene has been opened
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            switch (stateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    SetEnabled(true);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    // Don't allow switching scenes while in play mode
                    SetEnabled(false);
                    break;
            }
        }

        // Update the dropdown label whenever the active scene has potentially been renamed
        private void OnProjectChanged() => text = SceneManager.GetActiveScene().name;

        // Update the dropdown label whenever a scene has been opened
        private void OnSceneOpened(Scene scene, OpenSceneMode mode) => text = scene.name;

        private void ToggleDropdown()
        {
            var menu = new GenericMenu();
            foreach (var scenePath in SceneSwitcher.ScenePaths)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                menu.AddItem(new(sceneName), text == sceneName,
                    () => OnDropdownItemSelected(sceneName, scenePath));
            }

            menu.DropDown(worldBound);
        }

        private void OnDropdownItemSelected(string sceneName, string scenePath)
        {
            text = sceneName;
            SceneSwitcher.OpenScene(scenePath);
        }
    }
}

#endif