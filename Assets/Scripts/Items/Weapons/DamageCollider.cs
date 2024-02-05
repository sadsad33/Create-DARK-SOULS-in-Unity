using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 무기가 가지고 있을 클래스
namespace sg {
    public class DamageCollider : MonoBehaviour {
        public CharacterManager characterManager;
        public bool enabledDamageColliderOnStartUp = false;
        protected Collider damageCollider;

        [Header("Team I.D")]
        public int teamIDNumber; // 피아식별에 사용할 ID

        [Header("PoiseDamage")]
        public float poiseBreak;
        public float offensivePoiseBonus;

        [Header("Damage")]
        public float physicalDamage;
        public float fireDamage;

        bool shieldHasBeenHit;
        bool hasBeenParried;
        protected string currentDamageAnimation;
        protected virtual void Awake() {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
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
            if (other.CompareTag("Character")) {
                shieldHasBeenHit = false;
                hasBeenParried = false;

                CharacterStatsManager enemyStats = other.GetComponent<CharacterStatsManager>();
                CharacterManager enemyManager = other.GetComponent<CharacterManager>();
                CharacterEffectsManager enemyEffectsManager = other.GetComponent<CharacterEffectsManager>();
                BlockingCollider shield = other.transform.GetComponentInChildren<BlockingCollider>();

                if (enemyManager != null) {
                    if (enemyStats.teamIDNumber == teamIDNumber) return;
                    CheckForParry(enemyManager);
                    CheckForBlock(enemyManager, shield, enemyStats);
                }
                if (enemyStats != null) {
                    if (enemyStats.teamIDNumber == teamIDNumber) return;
                    if (hasBeenParried) return;
                    if (shieldHasBeenHit) return;
                    enemyStats.poiseResetTimer = enemyStats.totalPoiseResetTime;
                    enemyStats.totalPoiseDefense = enemyStats.totalPoiseDefense - poiseBreak;

                    // 타격 지점
                    Vector3 contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    float directionHitFrom = (Vector3.SignedAngle(characterManager.transform.forward, enemyManager.transform.forward, Vector3.up));
                    ChooseWhichDirectionDamageCameFrom(directionHitFrom);
                    enemyEffectsManager.PlayBloodSplatterFX(contactPoint);
                    if (enemyStats.totalPoiseDefense > poiseBreak) { // 보스일경우 피격시 애니메이션 재생 X
                        enemyStats.TakeDamageNoAnimation(physicalDamage);
                    } else {
                        enemyStats.TakeDamage(physicalDamage, 0, currentDamageAnimation);
                    }
                }
            }

            if (other.tag == "IllusionaryWall") {
                IllusionaryWall illusionaryWall = other.GetComponent<IllusionaryWall>();
                illusionaryWall.illusionaryWallHealthPoint -= 1;
            }
        }

        protected virtual void CheckForParry(CharacterManager enemyManager) {
            if (enemyManager.isParrying) {
                characterManager.GetComponentInChildren<CharacterAnimatorManager>().PlayTargetAnimation("Parried", true);
                hasBeenParried = true;
            }
        }

        protected virtual void CheckForBlock(CharacterManager enemyManager, BlockingCollider shield, CharacterStatsManager enemyStats) {
             if (shield != null && enemyManager.isBlocking) {
                float physicalDamageAfterBlock = physicalDamage - (physicalDamage * shield.blockingPhysicalDamageAbsorption) / 100;
                float fireDamageAfterBlock = fireDamage - (fireDamage * shield.blockingFireDamageAbsorption) / 100;

                if (enemyStats != null) {
                    enemyStats.TakeDamage(physicalDamageAfterBlock, fireDamageAfterBlock, "Block Impact");
                    shieldHasBeenHit = true;
                }
            }
        }
        protected virtual void ChooseWhichDirectionDamageCameFrom(float direction) {
            if (direction >= 145 && direction <= 180) {
                currentDamageAnimation = "Damage_Forward_1";
            } else if (direction <= -145 && direction >= -180) {
                currentDamageAnimation = "Damage_Forward_1";
            } else if (direction >= -45 && direction <= 45) {
                //currentDamageAnimation = "Damage_Back_1";
                currentDamageAnimation = "Damage_Forward_1";
            } else if (direction >= -144 && direction <= -45) {
                currentDamageAnimation = "Damage_Right_1";
            } else if (direction >= 45 && direction <= 144) {
                currentDamageAnimation = "Damage_Left_1";
            }
        }
    }
}
