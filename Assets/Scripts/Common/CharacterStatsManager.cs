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

        // ���ε� : ���۾Ƹ� ������ ���� �ʿ� ��ġ
        [Header("Poise")]
        public float totalPoiseDefense; // ������ ��꿡���� �� ���ε�
        public float offensivePoiseBonus; // ���ݸ���� ���ε� ���ʽ�
        public float armorPoiseBonus; // ������ �������ν� ��� ���ε�
        public float totalPoiseResetTime; // ���ε� �ʱ�ȭ �ð�
        public float poiseResetTimer = 0; // ���ε� �ʱ�ȭ Ÿ�̸�
        public bool isStuned; // �׷α� ����
        

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

        // ���ε� �ʱ�ȭ Ÿ�̸�
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
