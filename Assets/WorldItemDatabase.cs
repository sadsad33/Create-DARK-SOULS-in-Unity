using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace SoulsLike {
    public class WorldItemDatabase : MonoBehaviour {
        public static WorldItemDatabase instance;
        
        public WeaponItem unarmedWeapon;
        public List<Item> allItems = new List<Item>();
        public List<WeaponItem> weaponItems = new List<WeaponItem>();
        public List<SpellItem> spellItems = new List<SpellItem>();
        public List<EquipmentItem> equipmentItems = new List<EquipmentItem>();
        public List<RingItem> ringItems = new List<RingItem>();
        public List<ConsumableItem> consumableItems = new List<ConsumableItem>();

        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            // allItems 리스트에 게임에 존재하는 모든 아이템들을 담고 식별번호를 붙임
            foreach (var item in weaponItems) {
                allItems.Add(item);
            }

            foreach (var item in equipmentItems) {
                allItems.Add(item);
            }

            foreach (var item in ringItems) {
                allItems.Add(item);
            }

            foreach (var item in spellItems) {
                allItems.Add(item);
            }

            foreach (var item in consumableItems) {
                allItems.Add(item);
            }

            for (int i = 0; i < allItems.Count; i++) {
                allItems[i].itemID = i;
            }
        }

        private void Start() {
            DontDestroyOnLoad(this);
        }

        public WeaponItem GetWeaponItemByID(int weaponID) {
            // 전체 리스트를 탐색하여 id가 일치하는 가장 첫번째 WeaponItem을 반환, 찾지 못하면 null 반환
            return weaponItems.FirstOrDefault(weapon => weapon.itemID == weaponID);
        }

        public EquipmentItem GetEquipmentItemByID(int equipmentID) {
            return equipmentItems.FirstOrDefault(equipment => equipment.itemID == equipmentID);
        }

        public RingItem GetRingItemByID(int ringID) {
            return ringItems.FirstOrDefault(ring => ring.itemID == ringID);
        }

        public SpellItem GetSpellItemByID(int spellID) {
            return spellItems.FirstOrDefault(spell => spell.itemID == spellID);
        }

        [Obsolete("현재 월드 아이템 데이터 베이스는 소모품을 식별하지 않음")] 
        public ConsumableItem GetConsumableItemByID(int consumableID) {
            return consumableItems.FirstOrDefault(consumable => consumable.itemID == consumableID);
        }
    }
}
