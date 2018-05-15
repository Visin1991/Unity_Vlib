using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class SceneInitCommandQueue : Singleton<GameCentalPr>
    {

        protected SceneInitCommandQueue() { }

        static Queue<SceneInitCommandInfo> sceneInitCommandQueue = new Queue<SceneInitCommandInfo>();

        public static void Enqueue(SceneInitCommandInfo info)
        {
            sceneInitCommandQueue.Enqueue(info);
        }

        public static int Count
        {
            get { return sceneInitCommandQueue.Count; }
        }

        public static SceneInitCommandInfo DequeueInit()
        {
            return sceneInitCommandQueue.Dequeue();
        }

    }

    public struct SceneInitCommandInfo
    {
        public readonly System.Action<object> callback;
        public readonly object parameter;
        public SceneInitCommandInfo(System.Action<object> _callback, object _parameter)
        {
            callback = _callback;
            parameter = _parameter;
        }
    }
}