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

        protected bool shieldHasBeenHit;
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

        protected virtual void Start() {
            
        }

        public void EnableDamageCollider() {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider() {
            damageCollider.enabled = false;
        }

        [Obsolete("캐릭터에게 데미지를 주는 부분을 추출해서 메서드로 만듬, 만약 NPC 관련해서 문제가 발생한다면 이곳 참조")]
        protected void OnTriggerEnter(Collider other) {
            if (!characterCausingDamage.IsOwner) return;
            if (other.CompareTag("Character")) {
                shieldHasBeenHit = false;
                hasBeenParried = false;
                CharacterManager damageTarget = other.GetComponent<CharacterManager>();
                BlockingCollider shield = other.transform.GetComponentInChildren<BlockingCollider>();
                //Debug.Log(damageTarget.characterStatsManager.isDead);
                if (damageTarget.characterStatsManager.isDead) return;
                //Debug.Log("passed DeathCheck");
                if (damageTarget != null) {
                    if (damageTarget.characterStatsManager != null) {
                        if (damageTarget.characterStatsManager.teamIDNumber == teamIDNumber) return;
                        CheckForParry(damageTarget);
                        CheckForBlock(damageTarget, shield, damageTarget.characterStatsManager);
                        if (hasBeenParried) return;
                        if (shieldHasBeenHit) return;
                        damageTarget.characterStatsManager.poiseResetTimer = damageTarget.characterStatsManager.totalPoiseResetTime; // 강인도 리셋 시간 설정
                        damageTarget.characterStatsManager.totalPoiseDefense -= poiseDamage;

                        if (damageTarget.characterStatsManager.teamIDNumber == 0) {
                            NPCManager npcManager = damageTarget.transform.GetComponent<NPCManager>();
                            if (teamIDNumber == 1) {
                                npcManager.aggravationToEnemy += 30;
                            } else if (teamIDNumber == 2) {
                                npcManager.aggravationToPlayer += 10;
                            }
                            if (npcManager.currentTarget != characterCausingDamage.transform.GetComponent<CharacterStatsManager>()) {
                                npcManager.changeTargetTimer -= (npcManager.changeTargetTime / 2f);
                            }
                        }

                        contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                        angleHitFrom = (Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up));
                        DealDamage(damageTarget);
                    }
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
            
            TakeDamageEffect takeDamageEffect = Instantiate(WorldEffectsManager.instance.takeDamageEffect);
            takeDamageEffect.physicalDamage = finalPhysicalDamage;
            takeDamageEffect.fireDamage = finalFireDamage;
            takeDamageEffect.poiseDamage = poiseDamage;
            takeDamageEffect.contactPoint = contactPoint;
            takeDamageEffect.angleHitFrom = angleHitFrom;
            target.characterEffectsManager.ProcessEffectInstantly(takeDamageEffect);
        }

        protected virtual void CheckForParry(CharacterManager enemyManager) {
            if (enemyManager.isParrying) {
                //Debug.Log("패리 성공");
                characterCausingDamage.transform.GetComponent<CharacterAnimatorManager>().PlayTargetAnimation("Parried", true);
                hasBeenParried = true;
            }
        }

        protected virtual void CheckForBlock(CharacterManager enemyManager, BlockingCollider shield, CharacterStatsManager enemyStats) {
            if (shield != null && enemyManager.characterNetworkManager.isBlocking.Value) {

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
