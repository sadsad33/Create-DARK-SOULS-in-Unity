using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterInventoryManager : MonoBehaviour {

        protected CharacterWeaponSlotManager characterWeaponSlotManager;

        [Header("Quick Slot Items")]
        public SpellItem currentSpell;
        public WeaponItem rightWeapon, leftWeapon;
        public ConsumableItem currentConsumable;

        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3]; // ¿Þ¼Õ ¹«±â½½·Ô
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3]; // ¿À¸¥¼Õ ¹«±â½½·Ô

        [Header("Current Equipment")]
        public HelmetEquipment currentHelmetEquipment;
        public TorsoEquipment currentTorsoEquipment;
        public LegEquipment currentLegEquipment;
        public GuntletEquipment currentGuntletEquipment;

        public int currentRightWeaponIndex;
        public int currentLeftWeaponIndex;

        private void Awake() {
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
        }

        private void Start() {
            characterWeaponSlotManager.LoadBothWeaponsOnSlots();
        }
    }
}
