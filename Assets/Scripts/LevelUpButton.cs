using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class LevelUpButton : MonoBehaviour {
        public LevelUpUI levelUpUI;
        public int btnIndex;
        public Text stat; // ������ ���� ��
        public bool isUpperDirection;
        public int initValue;

        private void Start() {
            initValue = int.Parse(stat.text);
            levelUpUI = transform.parent.parent.parent.parent.GetComponent<LevelUpUI>();
        }

        int value = 0;
        int usedSouls = 0;

        // ������
        // ���� ��ġ ����/����
        // ���� �ؽ�Ʈ ���� ����
        public void OnClickButton() {
            value = int.Parse(stat.text);
            if (isUpperDirection && levelUpUI.playerStatsManager.soulCount >= levelUpUI.playerStatsManager.level * 65) {
                value += 1;
                levelUpUI.playerStatsManager.soulCount -= levelUpUI.playerStatsManager.level * 65;
            } else if (value > initValue) { // ���� ��������Ʈ���� ���������� ����
                value -= 1;
                levelUpUI.playerStatsManager.soulCount += (levelUpUI.playerStatsManager.level-1) * 65;
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