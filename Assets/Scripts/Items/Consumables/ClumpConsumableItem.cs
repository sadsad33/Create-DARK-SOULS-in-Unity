using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Consumables/CureEffectClump")]
    public class ClumpConsumableItem : ConsumableItem {
        [Header("Recovery FX")]
        public GameObject clumpConsumeFX;

        [Header("Cure FX")]
        public bool curePoison;
        // 출혈 치료
        // 동상 치료 ...

        public override void AttemptToConsumeItem(PlayerAnimatorManager playerAnimatorManager, PlayerWeaponSlotManager weaponSlotManager, PlayerEffectsManager playerEffectsManager) {
            base.AttemptToConsumeItem(playerAnimatorManager, weaponSlotManager, playerEffectsManager);
            GameObject clump = Instantiate(itemModel, weaponSlotManager.rightHandSlot.transform);
            playerEffectsManager.currentParticleFX = clumpConsumeFX;
            playerEffectsManager.instantiatedFXModel = clump;
            
            if (curePoison) {
                playerEffectsManager.poisonBuildUp = 0;
                playerEffectsManager.poisonAmount = playerEffectsManager.defaultPoisonAmount;
                playerEffectsManager.isPoisoned = false;
                if (playerEffectsManager.currentPoisonedParticleFX != null) {
                    Destroy(playerEffectsManager.currentPoisonedParticleFX);
                }
            }
            weaponSlotManager.rightHandSlot.UnloadWeapon();
        }
    }
}
