using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ ���Ⱑ ������ ���� Collider�� ��ũ��Ʈ
namespace sg {
    public class DamageCollider : MonoBehaviour {
        public CharacterManager characterManager;
        public bool enabledDamageColliderOnStartUp = false;
        Collider damageCollider;

        public float currentWeaponDamage;
        private void Awake() {
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
                PlayerStats playerStats = other.GetComponent<PlayerStats>();
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
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

            if (other.tag == "Enemy") {
                EnemyStats enemyStats = other.GetComponent<EnemyStats>();
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
                    if (enemyStats.isBoss) { // �����ϰ�� �ǰݽ� �ִϸ��̼� ��� X
                        enemyStats.TakeDamageNoAnimation(currentWeaponDamage);
                    } else {
                        enemyStats.TakeDamage(currentWeaponDamage);
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
