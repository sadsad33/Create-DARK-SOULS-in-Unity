using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class SpellItem : Item {
        public GameObject spellWarmUpFX;
        public GameObject spellCastFX;
        public string spellAnimation;

        [Header("Spell Cost")]
        public float focusCost;

        [Header("Spell Type")]
        public bool isFaithSpell;
        public bool isMagicSpell;
        public bool isPyroSpell;

        [Header("Spell Description")]
        [TextArea]
        public string spellDescription;

        public virtual void AttemptToCastSpell(PlayerAnimatorManager animatorHandler, PlayerStats playerStats, WeaponSlotManager weaponSlotManager) {
            Debug.Log("주문 영창 시도");
        }

        public virtual void SuccessfullyCastSpell(PlayerAnimatorManager animatorHandler, PlayerStats playerStats, CameraHandler cameraHandler, WeaponSlotManager weaponSlotManager) {
            Debug.Log("주문 영창 성공");
            playerStats.DeductFocus(focusCost);
        }
    }
}
