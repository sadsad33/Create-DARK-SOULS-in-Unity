using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각각의 무기가 가지고 있을 Collider의 스크립트
namespace sg {
    public class DamageCollider : MonoBehaviour {
        public CharacterManager characterManager;
        Collider damageCollider;
        public float currentWeaponDamage;
        private void Awake() {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.enabled = false; // Collider 만 끄기
            damageCollider.isTrigger = true;
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

                if (enemyCharacterManager != null) {
                    if (enemyCharacterManager.isParrying) {
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    }
                }
                if (playerStats != null) {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

            if (other.tag == "Enemy") {
                EnemyStats enemyStats = other.GetComponent<EnemyStats>();
                CharacterManager enemyCharacterManager = other.GetComponent<CharacterManager>();

                if (enemyCharacterManager != null) {
                    if (enemyCharacterManager.isParrying) {
                        characterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    }
                }
                if (enemyStats != null) {
                    enemyStats.TakeDamage(currentWeaponDamage);
                }
            }
        }
    }
}
