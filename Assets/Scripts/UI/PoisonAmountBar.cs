using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class PoisonAmountBar : MonoBehaviour {
        public Slider slider;
        private void Start() {
            slider = GetComponent<Slider>();
            slider.maxValue = 100;
            slider.value = 100;
            gameObject.SetActive(false);
        }

        public void SetPoisonAmount(float poisonAmount) {
            if (poisonAmount > 0) gameObject.SetActive(true);
            else gameObject.SetActive(false);
            slider.value = poisonAmount;
        }
    }
}
