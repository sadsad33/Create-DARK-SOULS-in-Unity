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

        public override void AttemptToConsumeItem(PlayerAnimatorManager playerAnimatorManager, PlayerWeaponSlotManager weaponSlotManager, PlayerEffectsManager playerEffectsManager) {
            if (currentItemAmount > 0) {
                playerAnimatorManager.PlayTargetAnimation(consumeAnimation, true);
                GameObject bombModel = Instantiate(itemModel, weaponSlotManager.rightHandSlot.transform.position, Quaternion.identity, weaponSlotManager.rightHandSlot.transform);
                playerEffectsManager.instantiatedFXModel = bombModel;
                weaponSlotManager.rightHandSlot.UnloadWeapon();
            } else {
                playerAnimatorManager.PlayTargetAnimation("Shrug", true);
            }
        }
    }
}
