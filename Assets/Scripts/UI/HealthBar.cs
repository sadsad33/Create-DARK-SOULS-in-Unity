using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace sg {
    public class HealthBar : MonoBehaviour {
        [SerializeField]
        Slider slider;

        private void Awake() {
            slider = GetComponent<Slider>();
        }

        public void SetMaxHealth(float maxHealth) { // 최대 체력
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetCurrentHealth(float currentHealth) { // 현재 체력
            slider.value = currentHealth;
        }
    }
}
