using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    [CreateAssetMenu(menuName = "Spells/Healing Spell")]
    public class HealingSpell : SpellItem {
        public float healAmount;

        public override void AttemptToCastSpell(PlayerAnimatorManager animatorHandler, PlayerStats playerStats, WeaponSlotManager weaponSlotManager) {
            base.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager);
            Instantiate(spellWarmUpFX, animatorHandler.transform);
            animatorHandler.PlayTargetAnimation(spellAnimation, true);
            Debug.Log("영창 중..");
        }

        public override void SuccessfullyCastSpell(PlayerAnimatorManager animatorHandler, PlayerStats playerStats, CameraHandler cameraHandler, WeaponSlotManager weaponSlotManager) {
            base.SuccessfullyCastSpell(animatorHandler, playerStats, cameraHandler, weaponSlotManager);
            Instantiate(spellCastFX, animatorHandler.transform);
            playerStats.HealPlayer(healAmount);
            Debug.Log("시전!!");
        }
    }
}
