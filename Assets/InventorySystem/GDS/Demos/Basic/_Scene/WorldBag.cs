using System;
using System.Collections.Generic;

using System.Linq;
using GDS.Core;
using GDS.Core.Events;
using UnityEngine;

namespace GDS.Basic {
    public enum BagType { Chest, Stash, Shop }

    // Adds a bag to a world interactible object
    public class WorldBag : MonoBehaviour {
        [Tooltip("Must be unique")]
        [SerializeField] string Id;
        [SerializeField] BagType BagType = BagType.Chest;
        [SerializeField][Range(1, 80)] int MaxSize = 10;
        [Space(10)]
        [SerializeReference] List<ItemInfo> Items;
        public Bag Bag { get; private set; }

        Bag CreateBag(BagType bagType, string id, int size) => bagType switch {
            BagType.Chest => Factory.CreateChest(id, size),
            BagType.Stash => Factory.CreateStash(id, size),
            BagType.Shop => Factory.CreateShop(id, size),
            _ => Bag.NoBag
        };

        void OnEnable() {
            Store.Bus.Subscribe<ResetEvent>(OnReset);
        }
        void OnDisable() {
            Store.Bus.Unsubscribe<ResetEvent>(OnReset);
        }

        void Awake() {
            if (Id == "") Id = "Interactible" + UnityEngine.Random.Range(0, 10000);
            Bag = CreateBag(BagType, Id, MaxSize);
            OnReset(CustomEvent.NoEvent);
        }

        void OnReset(CustomEvent _) {
            Bag.SetState(Items.Select(item => Factory.CreateItem(Bases.Get(item.BaseId), item.Rarity, item.Quant)).ToArray());
        }

        void OnMouseDown() {
            if (Store.Instance.IsInventoryOpen.Value) return;
            Store.Bus.Publish(new OpenSideWindowEvent(Bag));
        }
    }
}
