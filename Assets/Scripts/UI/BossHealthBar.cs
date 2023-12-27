using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class BossHealthBar : MonoBehaviour {
        public Text bossName; // 화면에 표시할 이름을 받아올 변수
        Slider slider; // 체력바를 관리할 변수

        private void Awake() {
            slider = GetComponentInChildren<Slider>();
            bossName = GetComponentInChildren<Text>();
        }

        private void Start() {
            slider.gameObject.SetActive(false);
        }

        public void SetBossName(string name) {
            bossName.text = name;
        }

        public void SetUIHealthBarToActive() {
            slider.gameObject.SetActive(true);    
        }

        public void SetHealthBarToInactive() {
            slider.gameObject.SetActive(false);
        }

        public void SetBossMaxHealth(float maxHealth) {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetBossCurrentHealth(float currentHealth) {
            slider.value = currentHealth;
        }
    }
}