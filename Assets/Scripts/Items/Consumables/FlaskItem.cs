using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Consumables/Flask")]
    public class FlaskItem : ConsumableItem {
        [Header("Flask Type")]
        public bool estusFlask;
        public bool ashenFlask;

        [Header("Recovery Amount")]
        public int healthRecoverAmount;
        public int focusPointRecoveryAmount;

        [Header("Recovery FX")]
        public GameObject recoveryFX;

        public override void AttemptToConsumeItem(PlayerManager player) {
            base.AttemptToConsumeItem(player);
            GameObject flask = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform);

            //Player recovery fx when/if we drink without being hit
            player.playerEffectsManager.currentParticleFX = recoveryFX;
            //Add Health or Focus Point
            player.playerEffectsManager.amountToBeHealed = healthRecoverAmount;
            //Instantiate flask in hand and play drink animation
            player.playerEffectsManager.instantiatedFXModel = flask;
            player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
        }
    }
}
