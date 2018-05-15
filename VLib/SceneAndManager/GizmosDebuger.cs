using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class GizmosDebuger : Singleton<GizmosDebuger>
    {
        protected GizmosDebuger() { }

        public Vector3 spherePos = new Vector3(999, 999, 999);


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spherePos, 0.2f);
            Gizmos.color = Color.white;
        }
    }
}
