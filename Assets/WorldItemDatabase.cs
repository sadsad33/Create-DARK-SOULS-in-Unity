using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SoulsLike {
    public class WorldItemDatabase : MonoBehaviour {
        public static WorldItemDatabase instance;

        public List<WeaponItem> weaponItems = new List<WeaponItem>();
        public List<EquipmentItem> equipmentItems = new List<EquipmentItem>();

        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
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
