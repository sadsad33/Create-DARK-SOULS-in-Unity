using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class PoisonBuildUpBar : MonoBehaviour {
        public Slider slider;
        private void Start() {
            slider = GetComponent<Slider>();
            slider.maxValue = 100;
            slider.value = 0;
            gameObject.SetActive(false);
        }

        public void SetPoisonBuildUpAmount(float currentPoisonBuildUp) {
            slider.value = currentPoisonBuildUp;

            if (currentPoisonBuildUp <= 0) gameObject.SetActive(false);
        }
    }
}