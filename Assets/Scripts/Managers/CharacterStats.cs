using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    public class CharacterStats : MonoBehaviour {
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

        [Header("Armor Absorptions")]
        public float physicalDamageAbsorptionHead;
        public float physicalDamageAbsorptionBody;
        public float physicalDamageAbsorptionLegs;
        public float physicalDamageAbsorptionHands;

        public bool isDead;

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
    }
}
