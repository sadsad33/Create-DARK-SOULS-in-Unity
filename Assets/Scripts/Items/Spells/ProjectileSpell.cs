using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem {
        [Header("Projectile Damage")]
        public float baseDamage;

        [Header("Projectile Physics")]
        public float projectileForwardVelocity;
        public float projectileUpwardVelocity;
        public float projectileMass;
        public bool isEffectedByGravity;
        Rigidbody rigidbody;

        public override void AttemptToCastSpell(PlayerAnimatorManager animatorHandler, PlayerStatsManager playerStats, PlayerWeaponSlotManager weaponSlotManager) {
            base.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager);
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, weaponSlotManager.rightHandSlot.transform);
            instantiatedWarmUpSpellFX.gameObject.transform.localScale = new Vector3(3, 3, 3);
            animatorHandler.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SuccessfullyCastSpell(PlayerAnimatorManager animatorHandler, PlayerStatsManager playerStatsManager, CameraHandler cameraHandler, PlayerWeaponSlotManager weaponSlotManager) {
            base.SuccessfullyCastSpell(animatorHandler, playerStatsManager, cameraHandler, weaponSlotManager);
            GameObject instantiatedSpellFX = Instantiate(spellCastFX, weaponSlotManager.rightHandSlot.transform.position, cameraHandler.cameraPivotTransform.rotation);
            rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
            //spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();
            SpellDamageCollider spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();
            
            spellDamageCollider.teamIDNumber = playerStatsManager.teamIDNumber; // 피아식별을 위한 팀ID 설정
            
            if (cameraHandler.currentLockOnTarget != null) {
                // 락온 상태라면 락온된 대상의 방향으로 투사체가 날아간다.
                instantiatedSpellFX.transform.LookAt(cameraHandler.currentLockOnTarget.transform);
            } else {
                // 투사체가 발사되는 높이는 카메라에 의해 제어된다.
                // 락온 상태가 아닐경우 플레이어가 바라보는 방향으로 투사체가 나가게끔 한다.
                instantiatedSpellFX.transform.rotation = Quaternion.Euler(cameraHandler.cameraPivotTransform.eulerAngles.x, playerStatsManager.transform.eulerAngles.y, 0);
            }

            rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
            rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
            rigidbody.useGravity = isEffectedByGravity;
            rigidbody.mass = projectileMass;
            instantiatedSpellFX.transform.parent = null;
        }
    }
}