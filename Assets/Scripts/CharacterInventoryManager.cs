using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterInventoryManager : MonoBehaviour {

        protected CharacterWeaponSlotManager characterWeaponSlotManager;

        [Header("Quick Slot Items")]
        public SpellItem currentSpell;
        public WeaponItem rightWeapon, leftWeapon;
        public ConsumableItem currentConsumable;

        public SpellItem[] memorizedSpells = new SpellItem[5];
        public ConsumableItem[] selectedConsumables = new ConsumableItem[5];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3]; // ¿Þ¼Õ ¹«±â½½·Ô
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3]; // ¿À¸¥¼Õ ¹«±â½½·Ô

        [Header("Current Equipment")]
        public HelmetEquipment currentHelmetEquipment;
        public TorsoEquipment currentTorsoEquipment;
        public LegEquipment currentLegEquipment;
        public GuntletEquipment currentGuntletEquipment;

        public int currentRightWeaponIndex;
        public int currentLeftWeaponIndex;
        public int currentSpellIndex;
        public int currentConsumableIndex;
        protected int rightWeaponUsedSize = 0;
        protected int leftWeaponUsedSize = 0;
        protected int spellUsedSize = 0;
        protected int consumableUsedSize = 0;

        private void Awake() {
            characterWeaponSlotManager = GetComponent<CharacterWeaponSlotManager>();
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            if (memorizedSpells[currentSpellIndex] != null) currentSpell = memorizedSpells[currentSpellIndex];
            if (selectedConsumables[currentConsumableIndex] != null) currentConsumable = selectedConsumables[currentConsumableIndex];
        }

        private void Start() {
            characterWeaponSlotManager.LoadBothWeaponsOnSlots();
            for (int i = 0; i < weaponsInRightHandSlots.Length; i++) {
                if (weaponsInLeftHandSlots[i] != null) leftWeaponUsedSize++;
                if (weaponsInRightHandSlots[i] != null) rightWeaponUsedSize++;
            }
            for (int i = 0; i < memorizedSpells.Length; i++) {
                if (memorizedSpells[i] != null) spellUsedSize++;
                if (selectedConsumables[i] != null) consumableUsedSize++;
            }
        }
    }
}
