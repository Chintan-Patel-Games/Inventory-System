using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDS.Core;
using UnityEngine;

namespace GDS.Basic {
    [System.Serializable]
    public class InventoryItem {
        public int pos;
        public string baseId;
        public int id;
        public int quant;
        public Rarity rarity;
    }

    [System.Serializable]
    public class EquipmentItem {
        public string slot;
        public string baseId;
        public int id;
        public int quant;
        public Rarity rarity;
    }

    [System.Serializable]
    public class PlayerData {
        public string playerName;
        public int level;
        public List<EquipmentItem> equipment;
        public List<InventoryItem> inventory;

        public PlayerData(string playerName, int level, SetBag equipment, ListBag inventory) {
            this.playerName = playerName;
            this.level = level;
            this.equipment = equipment.Slots
                    .Where(kv => kv.Value.IsNotEmpty())
                    .Select(kv => new EquipmentItem() {
                        slot = kv.Key,
                        baseId = kv.Value.Item.ItemBase.Id,
                        id = kv.Value.Item.Id,
                        quant = kv.Value.Item.Quant(),
                        rarity = kv.Value.Item.Rarity()
                    })
                    .ToList();
            this.inventory = inventory.Slots
                    .Where(slot => slot.IsNotEmpty())
                    .Select(slot => new InventoryItem() {
                        pos = slot.Index,
                        baseId = slot.Item.ItemBase.Id,
                        id = slot.Item.Id,
                        quant = slot.Item.Quant(),
                        rarity = slot.Item.Rarity()

                    }).ToList();

        }
    }

    public static class SaveSystem {
        private static string path => Application.persistentDataPath + "/basic-save.json";

        public static void Save(PlayerData data) {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log("Saved to: " + path);
        }

        public static PlayerData Load() {
            Debug.Log("should load from path: " + path);

            if (!File.Exists(path)) {
                Debug.LogWarning("Save file not found.");
                return null;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}