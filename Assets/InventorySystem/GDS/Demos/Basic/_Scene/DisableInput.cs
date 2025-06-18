using GDS.Basic;
using UnityEngine;

namespace GDS.Basic {

    // Disables a script (player input) when inventory is open
    public class DisableInput : MonoBehaviour {
        public MonoBehaviour Script;
        void OnEnable() {
            Store.Instance.IsInventoryOpen.OnChange += OnInventoryOpenChange;
        }

        void OnDisable() {
            Store.Instance.IsInventoryOpen.OnChange -= OnInventoryOpenChange;
        }

        void OnInventoryOpenChange(bool value) {
            if (!Script) return;
            Script.enabled = !value;
        }
    }
}