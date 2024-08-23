using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class MeleeWeaponDamageCollider : DamageCollider {
        [Header("Weapon Buff Damage")]
        public float physicalBuffDamage;
        public float fireBuffDamage;
        public float poiseBuffDamage;

        protected override void Start() {
            for (int i = 0; i < characterCausingDamage.characterColliders.Length; i++) {
                Physics.IgnoreCollision(damageCollider, characterCausingDamage.characterColliders[i]);
            }
        }
        protected override void DealDamage(CharacterManager damageTarget) {
            float finalPhysicalDamage = physicalDamage + physicalBuffDamage;
            float finalFireDamage = fireDamage + fireBuffDamage;
            TakeDamageEffect takeDamageEffect = Instantiate(WorldEffectsManager.instance.takeDamageEffect);
            takeDamageEffect.physicalDamage = finalPhysicalDamage;
            takeDamageEffect.fireDamage = finalFireDamage;
            takeDamageEffect.poiseDamage = poiseDamage;
            takeDamageEffect.contactPoint = contactPoint;
            takeDamageEffect.angleHitFrom = angleHitFrom;
            
            damageTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRpc(damageTarget.NetworkObjectId,
                takeDamageEffect.physicalDamage,
                takeDamageEffect.fireDamage,
                takeDamageEffect.poiseDamage,
                takeDamageEffect.contactPoint.x,
                takeDamageEffect.contactPoint.y,
                takeDamageEffect.contactPoint.z,
                takeDamageEffect.angleHitFrom,
                characterCausingDamage.NetworkObjectId);
        }

        protected override void CheckForBlock(CharacterManager damageTarget, BlockingCollider shield, CharacterStatsManager enemyStats) {
            if (shield != null && damageTarget.characterNetworkManager.isBlocking.Value) {

                if (enemyStats != null) {
                    shieldHasBeenHit = true;
                    TakeBlockedDamageEffect takeBlockedDamage = Instantiate(WorldEffectsManager.instance.takeBlockedDamageEffect);
                    takeBlockedDamage.physicalDamage = physicalDamage;
                    takeBlockedDamage.fireDamage = fireDamage;
                    takeBlockedDamage.poiseDamage = poiseDamage;
                    takeBlockedDamage.staminaDamage = poiseDamage;

                    damageTarget.characterNetworkManager.NotifyServerOfCharacterBlockedDamageServerRpc(damageTarget.NetworkObjectId,
                    takeBlockedDamage.physicalDamage,
                    takeBlockedDamage.fireDamage,
                    takeBlockedDamage.poiseDamage,
                    takeBlockedDamage.staminaDamage,
                    characterCausingDamage.NetworkObjectId);
                }
            }
        }
    }
}
