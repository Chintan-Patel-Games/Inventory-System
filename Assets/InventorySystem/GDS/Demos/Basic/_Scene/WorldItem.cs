using UnityEngine;
using GDS.Core;
using GDS.Core.Events;
using GDS.Basic.Events;

namespace GDS.Basic {
    // A world item is created when you drop items on the ground
    // It can be picked up by clicking on it
    public class WorldItem : MonoBehaviour {
        public Item Item;
        [SerializeField] SpriteRenderer sr;

        public void SetItem(Item item) {
            Item = item;
            sr.sprite = Resources.Load<Sprite>(item.ItemBase.IconPath);
        }

        void OnMouseDown() {
            if (Store.Instance.IsInventoryOpen.Value) return;
            EventBus.GlobalBus.Publish(new PickWorldItem(this));
        }
    }
}