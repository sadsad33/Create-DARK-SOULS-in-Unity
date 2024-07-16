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
            // ��ü ����Ʈ�� Ž���Ͽ� id�� ��ġ�ϴ� ���� ù��° WeaponItem�� ��ȯ, ã�� ���ϸ� null ��ȯ
            return weaponItems.FirstOrDefault(weapon => weapon.itemID == weaponID);
        }

        public EquipmentItem GetEquipmentItemByID(int equipmentID) {
            return equipmentItems.FirstOrDefault(equipment => equipment.itemID == equipmentID);
        }
    }
}
