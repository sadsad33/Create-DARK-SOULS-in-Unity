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
                if (target.characterStatsManager.totalPoiseDefense > poiseBreak) {
                    target.characterStatsManager.TakeDamageNoAnimation(finalPhysicalDamage, finalFireDamage);
                } else {
                    //enemyStats.isStuned = true;
                    target.characterStatsManager.TakeDamage(finalPhysicalDamage, finalFireDamage, currentDamageAnimation, characterCausingDamage);
                }
            }
        }
    }
}
