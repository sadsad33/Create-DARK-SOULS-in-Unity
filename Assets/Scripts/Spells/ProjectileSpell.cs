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

        public override void AttemptToCastSpell(PlayerAnimatorManager animatorHandler, PlayerStats playerStats, WeaponSlotManager weaponSlotManager) {
            base.AttemptToCastSpell(animatorHandler, playerStats, weaponSlotManager);
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, weaponSlotManager.rightHandSlot.transform);
            instantiatedWarmUpSpellFX.gameObject.transform.localScale = new Vector3(3, 3, 3);
            animatorHandler.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SuccessfullyCastSpell(PlayerAnimatorManager animatorHandler, PlayerStats playerStats, CameraHandler cameraHandler, WeaponSlotManager weaponSlotManager) {
            base.SuccessfullyCastSpell(animatorHandler, playerStats, cameraHandler, weaponSlotManager);
            GameObject instantiatedSpellFX = Instantiate(spellCastFX, weaponSlotManager.rightHandSlot.transform.position, cameraHandler.cameraPivotTransform.rotation);
            rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
            //spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();

            rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
            rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
            rigidbody.useGravity = isEffectedByGravity;
            rigidbody.mass = projectileMass;
            instantiatedSpellFX.transform.parent = null;
        }
    }
}