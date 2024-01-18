using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterStatsManager : MonoBehaviour {
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

        // 강인도 : 슈퍼아머 유지를 위한 필요 수치
        [Header("Poise")]
        public float totalPoiseDefense; // 데미지 계산에서의 총 강인도
        public float offensivePoiseBonus; // 공격모션중 강인도 보너스
        public float armorPoiseBonus; // 갑옷을 입음으로써 얻는 강인도
        public float totalPoiseResetTime = 15; // 강인도 초기화 시간
        public float poiseResetTimer = 0; // 강인도 초기화 타이머
        

        [Header("Armor Absorptions")]
        public float physicalDamageAbsorptionHead;
        public float physicalDamageAbsorptionBody;
        public float physicalDamageAbsorptionLegs;
        public float physicalDamageAbsorptionHands;

        public bool isDead;

        protected virtual void Update() {
            HandlePoiseResetTimer();
        }

        private void Start() {
            totalPoiseDefense = armorPoiseBonus;
        }

        public virtual void TakeDamage(float physicalDamage, string damageAnimation = "Damage") {
            if (isDead) return;

            float totalPhysicalDamageAbsorption = 1 - (1 - physicalDamageAbsorptionHead / 100) * (1 - physicalDamageAbsorptionBody / 100) * (1 - physicalDamageAbsorptionLegs / 100) * (1 - physicalDamageAbsorptionHands / 100);
            physicalDamage -= (physicalDamage * totalPhysicalDamageAbsorption);
            //Debug.Log("Total Physical Damage Absorption is " + totalPhysicalDamageAbsorption + "%");
            float finalDamage = physicalDamage;
            currentHealth -= finalDamage;
            //Debug.Log("Total Damage Dealt is " + finalDamage);

            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public virtual void TakeDamageNoAnimation(float damage) {
            currentHealth -= damage;
            if (currentHealth <= 0) {
                currentHealth = 0;
                isDead = true;
            }
        }

        public virtual void HandlePoiseResetTimer() {
            if (poiseResetTimer > 0) {
                poiseResetTimer -= Time.deltaTime;
            } else {
                totalPoiseDefense = armorPoiseBonus;
            }
        }
    }
}
