using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SoulsLike {
    public class CharacterStatsManager : NetworkBehaviour {
        CharacterManager character;

        // CharacterCombatManager
        [Header("마지막으로 받은 강인도 데미지의 총량")]
        public int previousPoiseDamageTaken;

        [Header("Team I.D")]
        public int teamIDNumber;
        
        public int soulsAwardedOnDeath;

        [Header("Stats")]
        public int healthLevel = 10;
        public float maxHealth;
        public float currentHealth;
        public int staminaLevel = 10;
        public float maxStamina;
        public float currentStamina;
        public int focusLevel = 10;
        public float maxFocus;
        public float currentFocus;
        public float blockingStabilityRating;
        
        public int soulCount = 0;

        public bool isBoss;

        // 중량 시스템을 만들면 사용할 것
        //[Header("Equip Load")]
        //public float currentEquipLoad = 0;
        //public float maxEquipLoad = 0;

        protected virtual void Awake() {
            character = GetComponent<CharacterManager>();
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
        public float totalPhysicalDamageDefenseRate;

        public float fireDamageAbsorptionHead;
        public float fireDamageAbsorptionBody;
        public float fireDamageAbsorptionLegs;
        public float fireDamageAbsorptionHands;
        public float totalFireDamageDefenseRate;

        [Header("Resistances")]
        public float poisonResistance;

        public bool isDead;

        // 캐릭터가 가하는 데미지의 배율
        [Header("Damage Type Modifiers")]
        public float physicalDamagePercentageModifier = 100;
        public float fireDamagePercentageModifier = 100;

        // 캐릭터가 받는 데미지의 배율
        [Header("Damage Absorption Modifiers")]
        public float physicalAbsorptionPercentageModifier = 0;
        public float fireAbsorptionPercentageModifier = 0;

        [Header("Poison")]
        public bool isPoisoned;
        public float poisonBuildUp = 0; // 독 축적을 위한 수치, 이 수치가 100이 되면 독 상태이상에 걸린다
        public float poisonAmount = 100; // 독 상태에서 독 상태이상이 해제되기 위해 필요한 수치

        protected virtual void Update() {
            HandlePoiseResetTimer();
        }

        protected virtual void Start() {
            totalPoiseDefense = armorPoiseBonus;
        }

        // 피격 애니메이션이 여러개라면 사용
        public string GetRandomDamageAnimationFromList(List<string> animationList) {
            int randomValue = Random.Range(0, animationList.Count);
            return animationList[randomValue];
        }

        public virtual void TakeDamageNoAnimation(float physicalDamage, float fireDamage = 0) {
            if (isDead) return;

            float totalPhysicalDamageAbsorption = 1 - 
                (1 - physicalDamageAbsorptionHead / 100) * 
                (1 - physicalDamageAbsorptionBody / 100) * 
                (1 - physicalDamageAbsorptionLegs / 100) * 
                (1 - physicalDamageAbsorptionHands / 100);
            physicalDamage -= (physicalDamage * totalPhysicalDamageAbsorption);

            float totalFireDamageAbsorption = 1 - 
                (1 - fireDamageAbsorptionHead / 100) * 
                (1 - fireDamageAbsorptionBody / 100) * 
                (1 - fireDamageAbsorptionLegs / 100) * 
                (1 - fireDamageAbsorptionHands / 100);
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

        public virtual void DeductStamina(float staminaToDeduct) {
            currentStamina -= staminaToDeduct;
        }

        public virtual float SetMaxHealthFromHealthLevel() {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public virtual float SetMaxFocusFromFocusLevel() {
            maxFocus = focusLevel * 10;
            return maxFocus;
        }

        public virtual float SetMaxStaminaFromStaminaLevel() {
            maxStamina = staminaLevel * 10;
            return maxStamina;
        }

        public virtual void HealPlayer(float healAmount) {
            currentHealth += healAmount;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }
    }
}
