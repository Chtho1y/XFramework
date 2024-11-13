using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;


namespace XEngine.Editor
{
    public class MissingScriptsAndEventsFinder : EditorWindow
    {
        private List<GameObject> objectsWithMissingScripts = new List<GameObject>();
        private List<GameObject> objectsWithInvalidEvents = new List<GameObject>();

        private enum SearchMode
        {
            Scene,
            Prefab
        }

        private SearchMode searchMode = SearchMode.Scene;

        [MenuItem("Tools/Find Missing Scripts and Invalid Events")]
        public static void ShowWindow()
        {
            GetWindow<MissingScriptsAndEventsFinder>("Find Missing Scripts and Invalid Events");
        }

        private void OnGUI()
        {
            GUILayout.Label("Search Mode", EditorStyles.boldLabel);
            searchMode = (SearchMode)EditorGUILayout.EnumPopup("Mode:", searchMode);

            if (GUILayout.Button("Find Missing Scripts and Invalid Events"))
            {
                if (searchMode == SearchMode.Scene)
                {
                    FindMissingScriptsAndInvalidEventsInScene();
                }
                else
                {
                    FindMissingScriptsAndInvalidEventsInPrefab();
                }
            }

            if (objectsWithMissingScripts.Count > 0)
            {
                GUILayout.Label("GameObjects with Missing Scripts:");
                foreach (GameObject obj in objectsWithMissingScripts)
                {
                    if (GUILayout.Button(obj.name))
                    {
                        Selection.activeGameObject = obj;
                    }
                }
            }
            else
            {
                GUILayout.Label("No GameObjects with missing scripts found.");
            }

            if (objectsWithInvalidEvents.Count > 0)
            {
                GUILayout.Label("GameObjects with Invalid Events:");
                foreach (GameObject obj in objectsWithInvalidEvents)
                {
                    if (GUILayout.Button(obj.name))
                    {
                        Selection.activeGameObject = obj;
                    }
                }

                if (GUILayout.Button("Remove Invalid Events"))
                {
                    RemoveInvalidEvents();
                }
            }
            else
            {
                GUILayout.Label("No GameObjects with invalid events found.");
            }
        }

        private void FindMissingScriptsAndInvalidEventsInScene()
        {
            objectsWithMissingScripts.Clear();
            objectsWithInvalidEvents.Clear();
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                CheckObject(obj);
            }

            Debug.Log($"Found {objectsWithMissingScripts.Count} GameObjects with missing scripts in the scene.");
            Debug.Log($"Found {objectsWithInvalidEvents.Count} GameObjects with invalid events in the scene.");
        }

        private void FindMissingScriptsAndInvalidEventsInPrefab()
        {
            objectsWithMissingScripts.Clear();
            objectsWithInvalidEvents.Clear();
            GameObject prefab = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot;
            if (prefab != null)
            {
                CheckObject(prefab);
                Debug.Log($"Found {objectsWithMissingScripts.Count} GameObjects with missing scripts in the prefab.");
                Debug.Log($"Found {objectsWithInvalidEvents.Count} GameObjects with invalid events in the prefab.");
            }
            else
            {
                Debug.LogWarning("No prefab is currently being edited.");
            }
        }

        private void CheckObject(GameObject obj)
        {
            bool hasMissingScripts = false;
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null)
                {
                    objectsWithMissingScripts.Add(obj);
                    hasMissingScripts = true;
                    break;
                }
            }

            if (!hasMissingScripts)
            {
                CheckForInvalidEvents(obj);
            }

            foreach (Transform child in obj.transform)
            {
                CheckObject(child.gameObject);
            }
        }

        private void CheckForInvalidEvents(GameObject obj)
        {
            Button[] buttons = obj.GetComponents<Button>();
            foreach (Button button in buttons)
            {
                var persistentEventCount = button.onClick.GetPersistentEventCount();
                for (int i = 0; i < persistentEventCount; i++)
                {
                    if (button.onClick.GetPersistentTarget(i) == null)
                    {
                        objectsWithInvalidEvents.Add(obj);
                        break;
                    }
                }
            }
        }

        private void RemoveInvalidEvents()
        {
            foreach (GameObject obj in objectsWithInvalidEvents)
            {
                Button[] buttons = obj.GetComponents<Button>();
                foreach (Button button in buttons)
                {
                    var persistentEventCount = button.onClick.GetPersistentEventCount();
                    for (int i = persistentEventCount - 1; i >= 0; i--)
                    {
                        if (button.onClick.GetPersistentTarget(i) == null)
                        {
                            UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick, i);
                        }
                    }
                }
            }

            Debug.Log("Removed invalid events.");
            objectsWithInvalidEvents.Clear();
        }
    }
}