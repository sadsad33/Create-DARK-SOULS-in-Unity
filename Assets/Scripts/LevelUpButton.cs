using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class LevelUpButton : MonoBehaviour {
        public LevelUpUI levelUpUI;
        public int btnIndex;
        public Text stat; // 변경할 스탯 값
        public bool isUpperDirection;
        public int initValue;

        private void Start() {
            initValue = int.Parse(stat.text);
            levelUpUI = transform.parent.parent.parent.parent.GetComponent<LevelUpUI>();
        }

        int value = 0;
        // 스탯 수치 증가/감소
        // 스탯 텍스트 색깔 변경
        public void OnClickButton() {
            value = int.Parse(stat.text);
            if (isUpperDirection) {
                value += 1;
            } else if (value > initValue) {
                value -= 1;
            }

            levelUpUI.ChangeSelectedStat(btnIndex, value);

            stat.text = value.ToString();
            if (initValue != value) {
                stat.color = Color.red;
            } else {
                stat.color = Color.white;
            }
        }
    }
}
