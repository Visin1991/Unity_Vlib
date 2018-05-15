using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace V
{
    /// <summary>
    ///     As the class name indicate, this class is used for execute SceneCommand.
    /// When we load the save data from disk and apply to the build in Scene, We need to make sure
    /// The Unity complete the loading, and then we can find the Object in the Scene, and apply the
    /// change to the scene Object.
    /// 
    /// So:
    ///     When The Start function of the SceneCommandExecuter get call, this means all of object originally
    ///     from the Scene already been loaded.
    ///     
    ///     The function FindObjectsOfType<T> can be called efficaciously.
    ///     
    ///     Because After call SceneManager.LoadScene(....), FindObjectsOfType<T> will return null
    ///     untill the new Scene complete Loading.....
    /// 
    /// </summary>
    public class SceneInitCommandExecuter : MonoBehaviour
    {

        private void Start()
        {
            while (SceneInitCommandQueue.Count > 0)
            {
                SceneInitCommandInfo info = SceneInitCommandQueue.DequeueInit();
                info.callback(info.parameter);
            }
        }
    }
}
