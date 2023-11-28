using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace sg {
    public class UIEnemyHealthBar : MonoBehaviour {
        Slider slider;
        float timeUntilBarIsHidden;
        public CameraHandler mainCamera;
        private void Awake() {
            slider = GetComponentInChildren<Slider>();
        }

        private void LateUpdate() {
            if (slider != null) {
                transform.forward = mainCamera.transform.forward;
            }
        }

        public void SetHealth(float health) {
            slider.value = health;
            timeUntilBarIsHidden = 3;
        }

        public void SetMaxHealth(float maxHealth) {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        private void Update() {
            timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;
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
                    Destroy(slider.gameObject);
                }
            }
        }
    }
}
