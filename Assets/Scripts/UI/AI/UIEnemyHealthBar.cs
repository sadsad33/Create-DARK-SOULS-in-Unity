using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class UIEnemyHealthBar : MonoBehaviour {
        public Slider slider;
        float timeUntilBarIsHidden;
        [SerializeField] private UIYellowBar yellowBar;
        [SerializeField] float yellowBarTimer = 2;
        [SerializeField] Text damageText;
        [SerializeField] float currentDamageTaken;

        //public CameraHandler mainCamera;
        private void Awake() {
            slider = GetComponentInChildren<Slider>();
            yellowBar = GetComponentInChildren<UIYellowBar>();
        }

        private void OnDisable() {
            currentDamageTaken = 0;
        }

        private void LateUpdate() {
            if (slider != null) {
                transform.forward = CameraHandler.instance.transform.forward;
            }
        }

        public void UpdateHealth(float health) {
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

        public void SetMaxHealth(float maxHealth) {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            if (yellowBar != null) yellowBar.SetMaxStat(maxHealth);
        }

        // 체력바가 화면에 표시된후 일정 시간이 지나면 다시 사라지도록 한다.
        private void Update() {
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
