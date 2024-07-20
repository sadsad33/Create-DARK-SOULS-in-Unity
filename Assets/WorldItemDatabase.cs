using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SoulsLike {
    public class WorldItemDatabase : MonoBehaviour {
        public static WorldItemDatabase instance;
        
        public WeaponItem unarmedWeapon;
        public List<Item> allItems = new List<Item>();
        public List<WeaponItem> weaponItems = new List<WeaponItem>();
        public List<EquipmentItem> equipmentItems = new List<EquipmentItem>();
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

            for (int i = 0; i < allItems.Count; i++) {
                allItems[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponItemByID(int weaponID) {
            // 전체 리스트를 탐색하여 id가 일치하는 가장 첫번째 WeaponItem을 반환, 찾지 못하면 null 반환
            return weaponItems.FirstOrDefault(weapon => weapon.itemID == weaponID);
        }

        public EquipmentItem GetEquipmentItemByID(int equipmentID) {
            return equipmentItems.FirstOrDefault(equipment => equipment.itemID == equipmentID);
        }
    }
}
