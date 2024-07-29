using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class MeleeWeaponDamageCollider : DamageCollider {
        [Header("Weapon Buff Damage")]
        public float physicalBuffDamage;
        public float fireBuffDamage;
        public float poiseBuffDamage;

        protected override void DealDamage(CharacterManager target) {
            float finalPhysicalDamage = physicalDamage + physicalBuffDamage;
            float finalFireDamage = fireDamage + fireBuffDamage;
            
            if (target.characterStatsManager.isBoss) {
                if (target.characterStatsManager.totalPoiseDefense < 0 && !target.characterStatsManager.isStuned) {
                    target.characterStatsManager.isStuned = true;
                    target.characterStatsManager.transform.GetComponent<CharacterAnimatorManager>().PlayTargetAnimation("BreakGuard", true);
                }
                target.characterStatsManager.TakeDamageNoAnimation(finalPhysicalDamage, finalFireDamage);
            } else {
                TakeDamageEffect takeDamageEffect = Instantiate(WorldEffectsManager.instance.takeDamageEffect);
                takeDamageEffect.physicalDamage = finalPhysicalDamage;
                takeDamageEffect.fireDamage = finalFireDamage;
                takeDamageEffect.poiseDamage = poiseDamage;
                takeDamageEffect.contactPoint = contactPoint;
                takeDamageEffect.angleHitFrom = angleHitFrom;
                target.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
            }
        }
    }
}
