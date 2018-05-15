using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace V
{
    /// <summary>
    /// Only the build-in Scene Object support multiple SerializableData attachment.
    /// All run time spawn Object should only use one SerializableData to serialize and deserilize
    /// all data------and also we need to use the SerilizabelData as a prefeb reference.....
    /// </summary>

    [ExecuteInEditMode]
    public abstract class BaseSerializableData : MonoBehaviour
    {

        static Dictionary<string, BaseSerializableData> allGuids = new Dictionary<string, BaseSerializableData>();

        public string uniqueId = "runTimeSpawn";

        public string type;

        protected virtual void Start()
        {
            SetUpDataType();
        }

        // Only compile the code in an editor build
#if UNITY_EDITOR

        // Whenever something changes in the editor (note the [ExecuteInEditMode])
        protected virtual void Update()
        {
            // Don't do anything when running the game
            if (Application.isPlaying)
                return;

            // Construct the name of the scene with an underscore to prefix to the Guid
            string sceneName = gameObject.scene.name + "_";

            // if we are not part of a scene then we are a prefab so do not attempt to set 
            // the id
            if (sceneName == null) return;

            // Test if we need to make a new id
            bool hasSceneNameAtBeginning = (uniqueId != null &&
                uniqueId.Length > sceneName.Length &&
                uniqueId.Substring(0, sceneName.Length) == sceneName);

            bool anotherComponentAlreadyHasThisID = (uniqueId != null &&
                allGuids.ContainsKey(uniqueId) &&
                allGuids[uniqueId] != this);

            if (!hasSceneNameAtBeginning || anotherComponentAlreadyHasThisID)
            {
                uniqueId = sceneName + System.Guid.NewGuid();
                EditorUtility.SetDirty(this);
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
            // We can be sure that the key is unique - now make sure we have 
            // it in our list
            if (!allGuids.ContainsKey(uniqueId))
            {
                allGuids.Add(uniqueId, this);
            }
        }

        // When we get destroyed (which happens when unloading a level)
        // we must remove ourselves from the global list otherwise the
        // entry still hangs around when we reload the same level again
        // but now the THIS pointer has changed and end up changing 
        // our ID
        void OnDestroy()
        {
            allGuids.Remove(uniqueId);
        }

#endif

        public void SerializeData(BinaryWriter writer)
        {
            //We need to right down the Data Type first for all SerializableData
            writer.Write(uniqueId);
            writer.Write(type);
            SerializeDataInternal(writer);
        }

        public abstract void DeSerializeData(BinaryReader reader);

        protected abstract void SerializeDataInternal(BinaryWriter writer);

        protected abstract void DeSerializeDataInternal(BinaryReader reader);

        public abstract void SetUpDataType();
    }
}