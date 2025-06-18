using UnityEngine;

namespace GDS.Basic {
    // Forces the object to always face the camera
    // Used on world item sprites
    public class Billboard : MonoBehaviour {
        void LateUpdate() {
            transform.forward = Camera.main.transform.forward;
        }
    }
}