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
            if (damageTarget.characterStatsManager.isBoss) {
                if (damageTarget.characterStatsManager.totalPoiseDefense < 0 && !damageTarget.characterStatsManager.isStuned) {
                    damageTarget.characterStatsManager.isStuned = true;
                    damageTarget.characterStatsManager.transform.GetComponent<CharacterAnimatorManager>().PlayTargetAnimation("BreakGuard", true);
                }
                //damageTarget.characterStatsManager.TakeDamageNoAnimation(finalPhysicalDamage, finalFireDamage);
            } else {
                takeDamageEffect.physicalDamage = finalPhysicalDamage;
                takeDamageEffect.fireDamage = finalFireDamage;
                takeDamageEffect.poiseDamage = poiseDamage;
                takeDamageEffect.contactPoint = contactPoint;
                takeDamageEffect.angleHitFrom = angleHitFrom;
                //damageTarget.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
            }

            // 이 클라이언트가 데미지를 입혔다면
            //if (characterCausingDamage.IsOwner) {
            // 데미지를 받는쪽에서 서버RPC 호출
            damageTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRpc(damageTarget.NetworkObjectId,
                takeDamageEffect.physicalDamage,
                takeDamageEffect.fireDamage,
                takeDamageEffect.poiseDamage,
                takeDamageEffect.contactPoint.x,
                takeDamageEffect.contactPoint.y,
                takeDamageEffect.contactPoint.z,
                characterCausingDamage.NetworkObjectId);
            //}
        }

        protected override void CheckForBlock(CharacterManager damageTarget, BlockingCollider shield, CharacterStatsManager enemyStats) {
            if (shield != null && damageTarget.characterNetworkManager.isBlocking.Value) {

                if (enemyStats != null) {
                    //enemyStats.TakeDamage(physicalDamageAfterBlock, fireDamageAfterBlock, "Block Impact", characterCausingDamage);
                    shieldHasBeenHit = true;
                    TakeBlockedDamageEffect takeBlockedDamage = Instantiate(WorldEffectsManager.instance.takeBlockedDamageEffect);
                    takeBlockedDamage.physicalDamage = physicalDamage;
                    takeBlockedDamage.fireDamage = fireDamage;
                    takeBlockedDamage.poiseDamage = poiseDamage;
                    takeBlockedDamage.staminaDamage = poiseDamage;

                    //damageTarget.characterEffectsManager.ProcessEffectInstantly(takeBlockedDamage);
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
