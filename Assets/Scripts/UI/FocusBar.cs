using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
namespace SoulsLike {
    public class FocusBar : MonoBehaviour {
        public Slider slider;
        private void Awake() {
            slider = GetComponent<Slider>();
        }

        public void SetCurrentFocus(float currentFocus) {
            slider.value = currentFocus;
        }

        public void SetMaxFocus(float maxFocus) {
            slider.maxValue = maxFocus;
            slider.value = maxFocus;
        }
    }
}