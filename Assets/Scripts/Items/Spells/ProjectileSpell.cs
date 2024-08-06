using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem {

        private Vector3 projectileShootingDirection;
        private float yRotationDuringFire;

        [Header("Projectile Damage")]
        public float baseDamage;

        [Header("Projectile Physics")]
        public float projectileForwardVelocity;
        public float projectileUpwardVelocity;
        public float projectileMass;
        public bool isEffectedByGravity;
        Rigidbody rigidbody;

        public override void AttemptToCastSpell(CharacterManager character) {
            base.AttemptToCastSpell(character);
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, character.characterWeaponSlotManager.rightHandSlot.transform);
            instantiatedWarmUpSpellFX.gameObject.transform.localScale = new Vector3(3, 3, 3);
            character.characterAnimatorManager.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SuccessfullyCastSpell(CharacterManager character) {
            base.SuccessfullyCastSpell(character);
            PlayerManager player = character as PlayerManager;
            if (player != null) {
                if (player.characterNetworkManager.isUsingRightHand.Value) {
                    player.playerCombatManager.currentSpelledProjectile = this;
                    GameObject instantiatedSpellFX = Instantiate(spellCastFX, character.characterWeaponSlotManager.rightHandSlot.transform.position, player.cameraHandler.cameraPivotTransform.rotation);
                    rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();
                    SpellDamageCollider spellDamageCollider = instantiatedSpellFX.GetComponent<SpellDamageCollider>();
                    spellDamageCollider.characterSpelledThis = player;
                    spellDamageCollider.teamIDNumber = player.playerStatsManager.teamIDNumber; // 피아식별을 위한 팀ID 설정

                    if (spellDamageCollider.characterSpelledThis.currentTarget != null) {
                        // 락온 상태라면 락온된 대상의 방향으로 투사체가 날아간다.
                        //instantiatedSpellFX.transform.LookAt(cameraHandler.currentLockOnTarget.transform);
                        Quaternion flameRotation = Quaternion.LookRotation(player.currentTarget.lockOnTransform.position - instantiatedSpellFX.gameObject.transform.position);
                        instantiatedSpellFX.transform.rotation = flameRotation;
                    } else {
                        // 투사체가 발사되는 높이는 카메라에 의해 제어된다.
                        // 락온 상태가 아닐경우 플레이어가 바라보는 방향으로 투사체가 나가게끔 한다.
                        instantiatedSpellFX.transform.rotation = Quaternion.Euler(player.cameraHandler.cameraPivotTransform.eulerAngles.x, player.playerStatsManager.transform.eulerAngles.y, 0);
                        projectileShootingDirection = CameraHandler.instance.cameraPivotTransform.forward;
                        yRotationDuringFire = character.transform.localEulerAngles.y;
                    }

                    rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
                    rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
                    rigidbody.useGravity = isEffectedByGravity;
                    rigidbody.mass = projectileMass;
                    instantiatedSpellFX.transform.parent = null;

                    player.playerNetworkManager.NotifyServerOfReleaseProjectileServerRpc(player.OwnerClientId,
                        itemID,
                        projectileShootingDirection.x, 
                        projectileShootingDirection.y, 
                        projectileShootingDirection.z,
                        yRotationDuringFire,
                        CameraHandler.instance.cameraPivotTransform.transform.localEulerAngles.x);
                }
            }
        }
    }
}