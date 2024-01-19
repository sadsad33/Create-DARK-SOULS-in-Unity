using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� ���Ⱑ ������ ���� Ŭ����
namespace sg {
    public class DamageCollider : MonoBehaviour {
        public CharacterManager characterManager;
        public bool enabledDamageColliderOnStartUp = false;
        //public WeaponFX weaponFX;
        Collider damageCollider;

        [Header("PoiseDamage")]
        public float poiseBreak;
        public float offensivePoiseBonus;

        [Header("Damage")]
        public float currentWeaponDamage;
        private void Awake() {
           // weaponFX = gameObject.GetComponentInChildren<WeaponFX>();
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.enabled = false; // Collider �� ����
            damageCollider.isTrigger = true;
            damageCollider.enabled = enabledDamageColliderOnStartUp;
        }

        public void EnableDamageCollider() {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider() {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                PlayerStatsManager playerStats = other.GetComponent<PlayerStatsManager>();
                CharacterManager playerCharacterManager = other.GetComponent<CharacterManager>();
                CharacterEffectsManager playerEffectsManager = other.GetComponent<CharacterEffectsManager>();
                BlockingCollider shield = other.transform.GetComponentInChildren<BlockingCollider>();
                if (playerCharacterManager != null) {
                    if (playerCharacterManager.isParrying) {
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    } else if (shield != null && playerCharacterManager.isBlocking) {
                        float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption) / 100;
                        if (playerStats != null) {
                            playerStats.TakeDamage(physicalDamageAfterBlock, "Block Impact");
                            return;
                        }
                    }
                }
                if (playerStats != null) {
                    playerStats.poiseResetTimer = playerStats.totalPoiseResetTime;
                    playerStats.totalPoiseDefense = playerStats.totalPoiseDefense - poiseBreak;
                    
                    // Ÿ�� ����
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    playerEffectsManager.PlayBloodSplatterFX(contactPoint);

                    if (playerStats.totalPoiseDefense > poiseBreak) { // �����ϰ�� �ǰݽ� �ִϸ��̼� ��� X
                        playerStats.TakeDamageNoAnimation(currentWeaponDamage);
                    } else {
                        playerStats.TakeDamage(currentWeaponDamage);
                    }
                }
            }

            if (other.tag == "Enemy") {
                EnemyStatsManager enemyStats = other.GetComponent<EnemyStatsManager>();
                CharacterManager enemyCharacterManager = other.GetComponent<CharacterManager>();
                CharacterEffectsManager enemyEffectsManager = other.GetComponent<CharacterEffectsManager>();
                BlockingCollider shield = other.transform.GetComponentInChildren<BlockingCollider>();
                if (enemyCharacterManager != null) {
                    if (enemyCharacterManager.isParrying) {
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    } else if (shield != null && enemyCharacterManager.isBlocking) {
                        float physicalDamageAfterBlock = currentWeaponDamage - (currentWeaponDamage * shield.blockingPhysicalDamageAbsorption) / 100;
                        if (enemyStats != null) {
                            enemyStats.TakeDamage(physicalDamageAfterBlock, "Block Impact");
                            return;
                        }
                    }
                }
                if (enemyStats != null) {
                    enemyStats.poiseResetTimer = enemyStats.totalPoiseResetTime;
                    enemyStats.totalPoiseDefense = enemyStats.totalPoiseDefense - poiseBreak;

                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    enemyEffectsManager.PlayBloodSplatterFX(contactPoint);

                    if (enemyStats.isBoss) {
                        if (enemyStats.totalPoiseDefense > poiseBreak) { // �����ϰ�� �ǰݽ� �ִϸ��̼� ��� X
                            enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                        } else {
                            enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                            enemyStats.BreakGuard();
                        }
                    } else {
                        if (enemyStats.totalPoiseDefense > poiseBreak) { // �����ϰ�� �ǰݽ� �ִϸ��̼� ��� X
                            enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                        } else {
                            enemyStats.TakeDamage(currentWeaponDamage);
                        }
                    }
                }
            }

            if (other.tag == "IllusionaryWall") {
                IllusionaryWall illusionaryWall = other.GetComponent<IllusionaryWall>();
                illusionaryWall.illusionaryWallHealthPoint -= 1;
            }
        }
    }
}
