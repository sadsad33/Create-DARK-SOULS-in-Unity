using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 무기가 가지고 있을 클래스
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
            damageCollider.enabled = false; // Collider 만 끄기
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
                CharacterManager enemyCharacterManager = other.GetComponent<CharacterManager>();
                BlockingCollider shield = other.transform.GetComponentInChildren<BlockingCollider>();
                if (enemyCharacterManager != null) {
                    if (enemyCharacterManager.isParrying) {
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    } else if (shield != null && enemyCharacterManager.isBlocking) {
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
                    Debug.Log("Player's Poise is currently " + playerStats.totalPoiseDefense);
                    if (playerStats.totalPoiseDefense > poiseBreak) { // 보스일경우 피격시 애니메이션 재생 X
                        playerStats.TakeDamageNoAnimation(currentWeaponDamage);
                        Debug.Log("Enemy Poise is currently " + playerStats.totalPoiseDefense);
                    } else {
                        playerStats.TakeDamage(currentWeaponDamage);
                    }
                }
            }

            if (other.tag == "Enemy") {
                EnemyStatsManager enemyStats = other.GetComponent<EnemyStatsManager>();
                CharacterManager enemyCharacterManager = other.GetComponent<CharacterManager>();
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
                    //Debug.Log("Enemy's Poise is currently " + enemyStats.totalPoiseDefense);

                    if (enemyStats.isBoss) {
                        if (enemyStats.totalPoiseDefense > poiseBreak) { // 보스일경우 피격시 애니메이션 재생 X
                            enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                        } else {
                            enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                            enemyStats.BreakGuard();
                        }
                    } else {
                        if (enemyStats.totalPoiseDefense > poiseBreak) { // 보스일경우 피격시 애니메이션 재생 X
                            enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                            //Debug.Log("Enemy Poise is currently " + enemyStats.totalPoiseDefense);
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
