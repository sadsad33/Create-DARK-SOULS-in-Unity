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

        public override void AttemptToConsumeItem(PlayerManager player) {
            base.AttemptToConsumeItem(player);
            GameObject clump = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);
            player.playerEffectsManager.currentParticleFX = clumpConsumeFX;
            player.playerEffectsManager.instantiatedFXModel = clump;
            
            if (curePoison) {
                player.playerEffectsManager.poisonBuildUp = 0;
                player.playerEffectsManager.poisonAmount = player.playerEffectsManager.defaultPoisonAmount;
                player.playerEffectsManager.isPoisoned = false;
                if (player.playerEffectsManager.currentPoisonedParticleFX != null) {
                    Destroy(player.playerEffectsManager.currentPoisonedParticleFX);
                }
            }
            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
        }
    }
}
