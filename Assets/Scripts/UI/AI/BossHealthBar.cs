using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class BossHealthBar : UIEnemyHealthBar {
        public Text bossName; // 화면에 표시할 이름을 받아올 변수
        float damageTextResetTimer = 2;
        //Slider slider;
        protected override void Awake() {
            base.Awake();
            //bossName = GetComponentInChildren<Text>();
        }

        private void Start() {
            slider.gameObject.SetActive(false);
        }

        protected override void LateUpdate() {

        }

        public void SetBossName(string name) {
            bossName.enabled = true;
            bossName.text = name;
        }

        public void SetUIHealthBarToActive() {
            slider.gameObject.SetActive(true);    
        }

        public void SetUIHealthBarToInactive() {
            slider.gameObject.SetActive(false);
        }

        public override void SetMaxHealth(float maxHealth) {
            base.SetMaxHealth(maxHealth);
        }

        public override void UpdateHealth(float health) {
            if (yellowBar != null) {
                yellowBar.gameObject.SetActive(true);
                yellowBar.timer = yellowBarTimer; // 데미지를 받을때마다 타이머 리셋 
                if (health > slider.value) {
                    yellowBar.slider.value = health;
                }
            }
            damageText.enabled = true;
            currentDamageTaken += (slider.value - health);
            damageText.text = currentDamageTaken.ToString();
            slider.value = health;
            damageTextResetTimer = 2;
        }

        protected override void Update() {
            damageTextResetTimer -= Time.deltaTime;
            if (slider != null) {
                if (damageTextResetTimer < 0) {
                    damageTextResetTimer = 0;
                    damageText.enabled = false;
                }
            }
        }
    }
}