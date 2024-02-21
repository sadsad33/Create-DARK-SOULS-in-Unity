using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class CharacterEffectsManager : MonoBehaviour {
        CharacterStatsManager characterStatsManager;
        [Header("Weapon FX")]
        public WeaponFX rightWeaponFX;
        public WeaponFX leftWeaponFX;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        [Header("Poison")]
        public GameObject defaultPoisonedParticleFX; // 플레이어가 기본적으로 가지고 있는 독 상태이상 이펙트
        public GameObject currentPoisonedParticleFX; // 독 상태이상에 걸렸다면 사용, 상태이상이 해제되면 파괴
        public Transform buildUpTransform; // 축적 이펙트가 생성될 위치
        public bool isPoisoned;
        public float poisonBuildUp = 0; // 독 축적을 위한 수치, 이 수치가 100이 되면 독 상태이상에 걸린다
        public float poisonAmount = 100; // 독 상태에서 독 상태이상이 해제되기 위해 필요한 수치
        public float defaultPoisonAmount = 100; // 독 상태이상에 걸렸을 경우 설정될 독 수치
        public float poisonDamage;
        public float poisonTimer = 2; // 매 2초마다 독 피해
        float timer;

        protected virtual void Awake() {
            characterStatsManager = GetComponent<CharacterStatsManager>();
        }

        public virtual void PlayWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponFX != null) {
                    rightWeaponFX.PlayWeaponFX();
                }
            } else {
                if (leftWeaponFX != null) {
                    leftWeaponFX.PlayWeaponFX();
                }
            }
        }

        public virtual void StopWeaponFX(bool isLeft) {
            if (!isLeft) {
                if (rightWeaponFX != null) {
                    rightWeaponFX.StopWeaponFX();
                }
            } else {
                if (leftWeaponFX != null) {
                    leftWeaponFX.StopWeaponFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation) {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }

        // 독 축적
        protected virtual void HandlePoisonBuildUp() {
            if (isPoisoned) return; // 이미 독 상태에 걸려있다면 더이상 축적하지 않는다
            if (poisonBuildUp > 0 && poisonBuildUp < 100) { // 독 상태에 걸리진 않았지만 독 수치가 누적되어 있다면
                poisonBuildUp -= Time.deltaTime;
            } else if (poisonBuildUp >= 100) { // 독 축적치가 100 이상이라면
                isPoisoned = true; // 독 상태이상
                poisonBuildUp = 0;
                
                if (buildUpTransform != null) {
                    currentPoisonedParticleFX = Instantiate(defaultPoisonedParticleFX, buildUpTransform.transform);
                } else {
                    currentPoisonedParticleFX = Instantiate(defaultPoisonedParticleFX, characterStatsManager.transform);
                }
            }
        }

        // 독 상태이상
        protected virtual void HandlePoisonedEffect() {
            if (isPoisoned) {
                if (poisonAmount > 0) {
                    timer += Time.deltaTime;

                    if (timer >= poisonTimer) {
                        characterStatsManager.TakePoisonDamage(poisonDamage);
                        timer = 0;
                    }
                    poisonAmount -= Time.deltaTime;
                } else {
                    isPoisoned = false;
                    poisonAmount = defaultPoisonAmount;
                    Destroy(currentPoisonedParticleFX);
                }
            }
        }

        // 모든 축적/상태이상 적용
        public virtual void HandleAllBuildUpEffects() {
            if (characterStatsManager.isDead) return;
            HandlePoisonBuildUp();
            HandlePoisonedEffect();
        }
    }
}