using UnityEngine;

namespace GDS.Basic {
    public class CameraFollow : MonoBehaviour {
        public Transform target;
        public Vector3 offset = new(0, 6f, -7f);

        // Camera will follow the target (player)
        void LateUpdate() => transform.position = target.position + offset;
    }
}