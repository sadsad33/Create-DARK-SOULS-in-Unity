using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class UIEnemyHealthBar : HealthBar {
        //public Slider slider;
        float timeUntilBarIsHidden;
        [SerializeField] protected UIYellowBar yellowBar;
        [SerializeField] protected float yellowBarTimer = 2;
        [SerializeField] protected Text damageText;
        [SerializeField] protected float currentDamageTaken;

        //public CameraHandler mainCamera;
        protected virtual void Awake() {
            slider = GetComponentInChildren<Slider>();
            yellowBar = GetComponentInChildren<UIYellowBar>();
            //damageText = GetComponentInChildren<Text>();
        }

        private void OnDisable() {
            currentDamageTaken = 0;
        }

        protected virtual void LateUpdate() {
            if (slider != null) {
                transform.forward = CameraHandler.instance.transform.forward;
            }
        }

        public virtual void UpdateHealth(float health) {
            if (yellowBar != null) {
                yellowBar.gameObject.SetActive(true);
                yellowBar.timer = yellowBarTimer; // 데미지를 받을때마다 타이머 리셋 
                if (health > slider.value) {
                    yellowBar.slider.value = health;
                }
            }
            currentDamageTaken += (slider.value - health);
            damageText.text = currentDamageTaken.ToString();
            slider.value = health;
            timeUntilBarIsHidden = 3;
        }

        public override void SetMaxHealth(float maxHealth) {
            base.SetMaxHealth(maxHealth);
            if (yellowBar != null) yellowBar.SetMaxStat(maxHealth);
        }

        // 체력바가 화면에 표시된후 일정 시간이 지나면 다시 사라지도록 한다.
        protected virtual void Update() {
            timeUntilBarIsHidden -= Time.deltaTime;
            if (slider != null) {
                if (timeUntilBarIsHidden < 0) {
                    timeUntilBarIsHidden = 0;
                    slider.gameObject.SetActive(false);
                } else {
                    if (!slider.gameObject.activeInHierarchy) {
                        slider.gameObject.SetActive(true);
                    }
                }
                if (slider.value <= 0) {
                    Destroy(slider.gameObject, 1.5f);
                }
            }
        }
    }
}
