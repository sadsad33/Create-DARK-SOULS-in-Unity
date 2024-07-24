using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    // 공격을 받아서 체력이 줄어들때, 줄어든양을 눈에 띄게 표시해주는 안쪽(?)의 체력바
    // 일정 시간에 따라 줄어들어 현재 체력양과 같아짐
    public class UIYellowBar : MonoBehaviour {
        [SerializeField] public Slider slider;
        UIEnemyHealthBar parentHealthBar;
        public float timer;

        private void Awake() {
            slider = GetComponent<Slider>();
            parentHealthBar = GetComponentInParent<UIEnemyHealthBar>();
        }
        public void SetMaxStat(float maxStat) {
            slider.maxValue = maxStat;
            slider.value = maxStat;
        }

        private void OnEnable() {
            if (timer <= 0) timer = 0.5f; // 노란색 체력바 남아있는 시간
        }

        private void Update() {
            if (timer <= 0) {
                if (slider.value > parentHealthBar.slider.value) {
                    slider.value -= 2;
                } else if (slider.value <= parentHealthBar.slider.value) {
                    gameObject.SetActive(false);
                }
            } else {
                timer -= Time.deltaTime;
            }
        }
    }
}
