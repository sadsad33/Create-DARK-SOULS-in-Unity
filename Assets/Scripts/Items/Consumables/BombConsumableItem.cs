using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Items/Consumables/Bomb Item")]
    public class BombConsumableItem : ConsumableItem {
        [Header("Velocity")]
        public int upwardVelocity;
        public int forwardVelocity;
        public int bombMass = 1;

        [Header("Live Bomb Model")]
        public GameObject liveBombModel;

        [Header("Base Damage")]
        public int baseDamage = 200; // 직격시 가할 데미지
        public float explosiveDamage = 75; // 주변 폭발 데미지

        public override void AttemptToConsumeItem(PlayerManager player) {
            if (currentItemAmount > 0) {
                player.playerAnimatorManager.PlayTargetAnimation(consumeAnimation, true);
                GameObject bombModel = Instantiate(itemModel, player.playerWeaponSlotManager.rightHandSlot.transform.position, Quaternion.identity, player.playerWeaponSlotManager.rightHandSlot.transform);
                player.playerEffectsManager.instantiatedFXModel = bombModel;
                player.playerWeaponSlotManager.rightHandSlot.UnloadWeapon();
            } else {
                player.playerAnimatorManager.PlayTargetAnimation("Shrug", true);
            }
        }
    }
}
