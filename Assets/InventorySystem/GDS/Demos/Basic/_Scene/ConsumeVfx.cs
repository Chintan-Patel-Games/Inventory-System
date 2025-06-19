using UnityEngine;
using GDS.Core.Events;

namespace GDS.Basic {
    // Create a green particle effect when consuming items
    public class ConsumeVfx : MonoBehaviour {
        [SerializeField] GameObject ConsumeVFX;

        private void OnEnable() {
            Store.Bus.Subscribe<ConsumeItemSuccess>(OnConsumeItemSuccess);
        }

        private void OnDisable() {
            Store.Bus.Unsubscribe<ConsumeItemSuccess>(OnConsumeItemSuccess);
        }

        void OnConsumeItemSuccess(CustomEvent e) {
            var pos = transform.position;
            pos.y = 2f;
            Instantiate(ConsumeVFX, pos, Quaternion.identity);
        }
    }
}