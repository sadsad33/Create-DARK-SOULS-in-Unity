using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class MeleeWeaponDamageCollider : DamageCollider {
        [Header("Weapon Buff Damage")]
        public float physicalBuffDamage;
        public float fireBuffDamage;
        public float poiseBuffDamage;

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

            if (characterCausingDamage.IsOwner) {
                damageTarget.characterNetworkManager.NotifyServerOfCharacterDamageServerRpc(damageTarget.NetworkObjectId, 
                    takeDamageEffect.physicalDamage, 
                    takeDamageEffect.fireDamage, 
                    takeDamageEffect.poiseDamage,
                    takeDamageEffect.contactPoint.x,
                    takeDamageEffect.contactPoint.y,
                    takeDamageEffect.contactPoint.z,
                    characterCausingDamage.NetworkObjectId);
            }
        }
    }
}
