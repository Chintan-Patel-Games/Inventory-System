using UnityEngine;

namespace GDS.Basic {

    // Highlights an object (changes object material) on mouse over
    [RequireComponent(typeof(Renderer))]
    public class HighlightObject : MonoBehaviour {
        Renderer Renderer;
        Material Initial;
        [SerializeField] Material Highlight;
        void Awake() {
            Renderer = GetComponent<Renderer>();
            Initial = Renderer.material;
        }

        void OnMouseEnter() {
            Renderer.material = Highlight;
        }

        void OnMouseExit() {
            Renderer.material = Initial;
        }

    }
}