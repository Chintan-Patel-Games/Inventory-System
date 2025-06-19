using GDS.Basic.Events;
using GDS.Core;
using GDS.Core.Events;
using UnityEngine;

namespace GDS.Basic {

    // Listens to pick and drop item events
    // When an item is dropped from the inventory it will be created as a world item 
    // in a random spot on a circle around the player
    public class WorldItemEvents : MonoBehaviour {

        public Transform Player;
        public GameObject ItemPrefab;
        public ParticleSystem PickupVFX;
        Vector3 ItemOffset = new Vector3(0, 0.25f, 0);

        private void OnEnable() {
            EventBus.GlobalBus.Subscribe<PickWorldItem>(OnPickupWorldItem);
            Store.Bus.Subscribe<DropItemSuccess>(OnDropItemSuccess);
        }

        private void OnDisable() {
            EventBus.GlobalBus.Unsubscribe<PickWorldItem>(OnPickupWorldItem);
            Store.Bus.Unsubscribe<DropItemSuccess>(OnDropItemSuccess);
        }

        void OnPickupWorldItem(CustomEvent e) {
            if (e is not PickWorldItem evt) return;
            if (Store.Instance.Main.IsFull()) return;

            Store.Bus.Publish(new AddItemEvent(evt.WorldItem.Item));
            Instantiate(PickupVFX, evt.WorldItem.transform.position, Quaternion.identity);
            Destroy(evt.WorldItem.gameObject);
        }

        void OnDropItemSuccess(CustomEvent e) {
            Debug.Log("inventory event " + e);
            if (e is not DropItemSuccess ev) return;
            var radius = 2;
            var pos = Player.position + RandomPointOnCircle(radius) + ItemOffset;
            pos.y = 0.4f;
            var instance = Instantiate(ItemPrefab, pos, Quaternion.identity);
            instance.GetComponent<WorldItem>().SetItem(ev.Item);
        }

        Vector3 RandomPointOnCircle(int radius) {
            float angle = UnityEngine.Random.Range(0, 360);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            return new Vector3(x, 0, z);
        }
    }

}