using UnityEngine;
using UnityEditor;

namespace V
{
    public class EditorTime_Auto_LE_Initializer : EditorWindow
    {

        [MenuItem("Tools/LE Initializer")]
        static void CreateLEEditorInitializerWindow()
        {
            EditorWindow.GetWindow<EditorTime_Auto_LE_Initializer>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Editor Time Initalization"))
            {
                MonoBehaviour[] ms = FindObjectsOfType<MonoBehaviour>();
                foreach (MonoBehaviour m in ms)
                {
                    LEEditorTimeAutoInitializer initializer = m as LEEditorTimeAutoInitializer;
                    if (initializer != null)
                    {
                        initializer.ET_Init();
                    }
                }
            }
        }
    }
}
