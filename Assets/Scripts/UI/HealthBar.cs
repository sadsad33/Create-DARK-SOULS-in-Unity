using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class HealthBar : MonoBehaviour {
        public Slider slider;
        private void Awake() {
            slider = GetComponent<Slider>();
        }

        public virtual void SetMaxHealth(float maxHealth) { // 최대 체력
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public virtual void SetCurrentHealth(float currentHealth) { // 현재 체력
            slider.value = currentHealth;
        }
    }
}
