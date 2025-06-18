using GDS.Core.Events;
using UnityEngine;
namespace GDS.Basic {

    // Use this if you want to reference a Store bag
    // Clicking this item will send an event to the store which in turn will open a window with the specified Id
    public class StoreBag : MonoBehaviour {
        // The Id should match a bag Id from the store
        [Tooltip("The Id should match a Bag Id from the 'Store'")]
        public string Id;

        void OnMouseDown() {
            if (Store.Instance.IsInventoryOpen.Value) return;
            Store.Bus.Publish(new OpenSideWindowByIdEvent(Id));
        }
    }

}
