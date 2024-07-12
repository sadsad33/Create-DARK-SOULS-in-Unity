using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class CharacterStatsManager : NetworkBehaviour {
        CharacterAnimatorManager characterAnimatorManager;
        [Header("Team I.D")]
        public int teamIDNumber;
        
        public int soulsAwardedOnDeath;

        public int healthLevel = 10;
        public float maxHealth;
        public float currentHealth;

        public int staminaLevel = 10;
        public float maxStamina;
        public float currentStamina;

        public int focusLevel = 10;
        public float maxFocus;
        public float currentFocus;

        public int soulCount = 0;

        public bool isBoss;

        protected virtual void Awake() {
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        }

        // 강인도 : 슈퍼아머 유지를 위한 필요 수치
        [Header("Poise")]
        public float totalPoiseDefense; // 데미지 계산에서의 총 강인도
        public float offensivePoiseBonus; // 공격모션중 강인도 보너스
        public float armorPoiseBonus; // 갑옷을 입음으로써 얻는 강인도
        public float totalPoiseResetTime; // 강인도 초기화 시간
        public float poiseResetTimer = 0; // 강인도 초기화 타이머
        public bool isStuned; // 그로기 상태
        

        [Header("Armor Absorptions")]
        public float physicalDamageAbsorptionHead;
        public float physicalDamageAbsorptionBody;
        public float physicalDamageAbsorptionLegs;
        public float physicalDamageAbsorptionHands;
        public float totalPhysicalDamageAbsorption;

        public float fireDamageAbsorptionHead;
        public float fireDamageAbsorptionBody;
        public float fireDamageAbsorptionLegs;
        public float fireDamageAbsorptionHands;
        public float totalFireDamageAbsorption;

        public bool isDead;

        protected virtual void Update() {
            HandlePoiseResetTimer();
        }

        protected virtual void Start() {
            totalPoiseDefense = armorPoiseBonus;
        }

        public virtual void TakeDamage(float physicalDamage, float fireDamage, string damageAnimation) {
            if (isDead) return;
            characterAnimatorManager.EraseHandIKForWeapon();
            physicalDamage -= (physicalDamage * totalPhysicalDamageAbsorption);
            fireDamage -= (fireDamage * totalFireDamageAbsorption);

            float finalDamage = physicalDamage + fireDamage;
            currentHealth -= finalDamage;

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public virtual void TakeDamageNoAnimation(float physicalDamage, float fireDamage = 0) {
            if (isDead) return;

            float totalPhysicalDamageAbsorption = 1 - (1 - physicalDamageAbsorptionHead / 100) * (1 - physicalDamageAbsorptionBody / 100) * (1 - physicalDamageAbsorptionLegs / 100) * (1 - physicalDamageAbsorptionHands / 100);
            physicalDamage -= (physicalDamage * totalPhysicalDamageAbsorption);

            float totalFireDamageAbsorption = 1 - (1 - fireDamageAbsorptionHead / 100) * (1 - fireDamageAbsorptionBody / 100) * (1 - fireDamageAbsorptionLegs / 100) * (1 - fireDamageAbsorptionHands / 100);
            fireDamage -= (fireDamage * totalFireDamageAbsorption);

            float finalDamage = physicalDamage + fireDamage;
            currentHealth -= finalDamage;

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public virtual void TakePoisonDamage(float damage) {
            currentHealth -= damage;
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        // 강인도 초기화 타이머
        public virtual void HandlePoiseResetTimer() {
            if (poiseResetTimer > 0) {
                poiseResetTimer -= Time.deltaTime;
                //Debug.Log(poiseResetTimer);
            } else {
                totalPoiseDefense = armorPoiseBonus;
            }
        }
    }
}
