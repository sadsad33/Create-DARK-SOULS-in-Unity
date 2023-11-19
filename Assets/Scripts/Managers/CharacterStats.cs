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

        public bool isDead;
    }
}
