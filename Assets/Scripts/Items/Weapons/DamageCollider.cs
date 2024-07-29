using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// 모든 무기가 가지고 있을 클래스
namespace SoulsLike {
    public class DamageCollider : MonoBehaviour {
        public CharacterManager characterCausingDamage;
        public bool enabledDamageColliderOnStartUp = false;
        protected Collider damageCollider;

        [Header("Team I.D")]
        public int teamIDNumber; // 피아식별에 사용할 ID

        [Header("PoiseDamage")]
        public float poiseDamage;
        public float offensivePoiseBonus;

        [Header("Damage")]
        public float physicalDamage;
        public float fireDamage;

        bool shieldHasBeenHit;
        bool hasBeenParried;
        protected string currentDamageAnimation;

        protected Vector3 contactPoint;
        protected float angleHitFrom;

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

        [Obsolete("캐릭터에게 데미지를 주는 부분을 추출해서 메서드로 만듬, 만약 NPC 관련해서 문제가 발생한다면 이곳 참조")]
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Character")) {
                shieldHasBeenHit = false;
                hasBeenParried = false;
                CharacterManager target = other.GetComponent<CharacterManager>();
                BlockingCollider shield = other.transform.GetComponentInChildren<BlockingCollider>();
                
                if (target.characterStatsManager.isDead) return;
                
                if (target != null) {
                    if (target.characterStatsManager.teamIDNumber == teamIDNumber) return;
                    CheckForParry(target);
                    CheckForBlock(target, shield, target.characterStatsManager);
                }
                
                if (target.characterStatsManager != null) {
                    if (target.characterStatsManager.teamIDNumber == teamIDNumber) return;
                   
                    if (hasBeenParried) return;
                    
                    if (shieldHasBeenHit) return;
                    
                    target.characterStatsManager.poiseResetTimer = target.characterStatsManager.totalPoiseResetTime; // 강인도 리셋 시간 설정
                    target.characterStatsManager.totalPoiseDefense -= poiseDamage;

                    if (target.characterStatsManager.teamIDNumber == 0) {
                        NPCManager npcManager = target.transform.GetComponent<NPCManager>();
                        if (teamIDNumber == 1) {
                            npcManager.aggravationToEnemy += 30;
                        } else if (teamIDNumber == 2) {
                            npcManager.aggravationToPlayer += 10;
                        }
                        if (npcManager.currentTarget != characterCausingDamage.transform.GetComponent<CharacterStatsManager>()) {
                            Debug.Log("타겟 설정 시간 감소");
                            npcManager.changeTargetTimer -= (npcManager.changeTargetTime / 2f);
                        }
                    }

                    // 타격 지점
                    contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                    angleHitFrom = (Vector3.SignedAngle(characterCausingDamage.transform.forward, target.transform.forward, Vector3.up));
                    DealDamage(target);
                }
            }

            if (other.CompareTag("IllusionaryWall")) {
                IllusionaryWall illusionaryWall = other.GetComponent<IllusionaryWall>();
                illusionaryWall.illusionaryWallHealthPoint -= 1;
            }
        }

        protected virtual void DealDamage(CharacterManager target) {
            float finalPhysicalDamage = physicalDamage;
            float finalFireDamage = fireDamage;

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

        protected virtual void CheckForParry(CharacterManager enemyManager) {
            if (enemyManager.isParrying) {
                //Debug.Log("패리 성공");
                characterCausingDamage.transform.GetComponent<CharacterAnimatorManager>().PlayTargetAnimation("Parried", true);
                hasBeenParried = true;
            }
        }

        protected virtual void CheckForBlock(CharacterManager enemyManager, BlockingCollider shield, CharacterStatsManager enemyStats) {
            if (shield != null && enemyManager.isBlocking) {

                if (enemyStats != null) {
                    //enemyStats.TakeDamage(physicalDamageAfterBlock, fireDamageAfterBlock, "Block Impact", characterCausingDamage);
                    shieldHasBeenHit = true;
                    TakeBlockedDamageEffect takeBlockedDamage = Instantiate(WorldEffectsManager.instance.takeBlockedDamageEffect);
                    takeBlockedDamage.physicalDamage = physicalDamage;
                    takeBlockedDamage.fireDamage = fireDamage;
                    takeBlockedDamage.poiseDamage = poiseDamage;
                    takeBlockedDamage.staminaDamage = poiseDamage;

                    enemyManager.characterEffectsManager.ProcessEffectInstantly(takeBlockedDamage);
                }
            }
        }
    }
}
