using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterInventoryManager : MonoBehaviour {

        protected CharacterManager character;

        [Header("Quick Slot Items")]
        public SpellItem currentSpell;
        public WeaponItem rightWeapon, leftWeapon;
        public ConsumableItem currentConsumable;

        public SpellItem[] memorizedSpells = new SpellItem[5];
        public ConsumableItem[] selectedConsumables = new ConsumableItem[3];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3]; // 왼손 무기슬롯
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3]; // 오른손 무기슬롯

        [Header("Current Equipment")]
        public HelmetEquipment currentHelmetEquipment;
        public TorsoEquipment currentTorsoEquipment;
        public LegEquipment currentLegEquipment;
        public GuntletEquipment currentGuntletEquipment;
        public RingItem ringSlot01;
        public RingItem ringSlot02;
        public RingItem ringSlot03;
        public RingItem ringSlot04;

        public int currentRightWeaponIndex;
        public int currentLeftWeaponIndex;
        public int currentSpellIndex;
        public int currentConsumableIndex;

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
            rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            if (memorizedSpells[currentSpellIndex] != null) currentSpell = memorizedSpells[currentSpellIndex];
            if (selectedConsumables[currentConsumableIndex] != null) currentConsumable = selectedConsumables[currentConsumableIndex];
        }

        protected virtual void Start() {
            character.characterWeaponSlotManager.LoadBothWeaponsOnSlots();
        }
    }
}
