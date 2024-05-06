using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class LevelUpUI : MonoBehaviour {
        public PlayerStatsManager playerStatsManager;
        public PlayerWeaponSlotManager playerWeaponSlotManager;
        public GameObject currentStatsBackground;
        public GameObject statSelectBackground;
        public Text[] currentStats;
        public Text[] statSelectTexts;
        public Text requiredSouls;

        private float[] initStats = new float[3];

        private void OnEnable() {
            currentStats = new Text[currentStatsBackground.transform.childCount];
            Transform curStatsWindow = transform.GetChild(0);
            for (int i = 0; i < currentStats.Length; i++) {
                currentStats[i] = curStatsWindow.GetChild(i).GetComponentInChildren<Text>();
            }

            Transform statSelectWindowTextSpace = transform.GetChild(1).GetChild(0).GetChild(2);
            statSelectTexts = new Text[statSelectWindowTextSpace.childCount];
            for (int i = 0; i < statSelectTexts.Length; i++) {
                statSelectTexts[i] = statSelectWindowTextSpace.GetChild(i).GetComponent<Text>();
            }

            GetCurrentStatPoints();

            for (int i = 0; i < 3; i++) {
                PrintCurrentStatPoints(i);
            }

            for (int i = 0; i < currentStats.Length; i++) {
                PrintCurrentStats(i);
            }
        }

        // �� ������ ���� �ִ�ġ�� ���
        private void GetCurrentStatPoints() {
            initStats[0] = playerStatsManager.maxHealth;
            initStats[1] = playerStatsManager.maxFocus;
            initStats[2] = playerStatsManager.maxStamina;
        }

        // ���� ����Ʈ������ ���� �ɷ�ġ���� UI�� ����Ѵ�
        private void PrintCurrentStats(int index) {
            switch (index) {
                case 0:
                    currentStats[index].text = "Current Health : ";
                    currentStats[index].text += playerStatsManager.maxHealth.ToString();
                    // �ִ밪�� �޶����ٸ�(�ö��ٸ�) ���������� ����
                    if (initStats[index] != playerStatsManager.maxHealth) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 1:
                    currentStats[index].text = "Current Focus : ";
                    currentStats[index].text += playerStatsManager.maxFocus.ToString();
                    if (initStats[index] != playerStatsManager.maxFocus) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 2:
                    currentStats[index].text = "Current Stamina : ";
                    currentStats[index].text += playerStatsManager.maxStamina.ToString();
                    if (initStats[index] != playerStatsManager.maxStamina) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 3:
                    currentStats[index].text = "Current ATK(Right Hand) : ";
                    currentStats[index].text += playerWeaponSlotManager.rightHandSlot.currentWeapon.physicalDamage.ToString();
                    break;
                case 4:
                    currentStats[index].text = "Current ATK(Left Hand) : ";
                    currentStats[index].text += playerWeaponSlotManager.leftHandSlot.currentWeapon.physicalDamage.ToString();
                    break;
                case 5:
                    currentStats[index].text = "Current DEF : ";
                    currentStats[5].text += (Mathf.Round(playerStatsManager.totalPhysicalDamageAbsorption * 100)).ToString() + '%';
                    break;
                case 6:
                    currentStats[index].text = "Current Level : ";
                    currentStats[6].text += playerStatsManager.level.ToString();
                    break;
                case 7:
                    currentStats[index].text = "Current Souls : ";
                    currentStats[7].text += playerStatsManager.soulCount.ToString();
                    break;
            }
        }

        // ���� ���� ����Ʈ���� UI�� ����Ѵ�
        private void PrintCurrentStatPoints(int index) {
            switch (index) {
                case 0:
                    statSelectTexts[index].text = playerStatsManager.healthLevel.ToString();
                    break;
                case 1:
                    statSelectTexts[index].text = playerStatsManager.focusLevel.ToString();
                    break;
                case 2:
                    statSelectTexts[index].text = playerStatsManager.staminaLevel.ToString();
                    break;
            }
        }

        // �ش� �ε����� �ɷ�ġ���� ����� ����Ʈ���� �����Ѵ�
        public void ChangeSelectedStat(int index, int value) {
            switch (index) {
                case 0:
                    playerStatsManager.maxHealth = value * 10;
                    playerStatsManager.healthLevel = value;
                    break;
                case 1:
                    playerStatsManager.maxFocus = value * 10;
                    playerStatsManager.focusLevel = value;
                    break;
                case 2:
                    playerStatsManager.maxStamina = value * 10;
                    playerStatsManager.staminaLevel = value;
                    break;
            }
            PrintCurrentStats(index);
        }

        public void SaveChange() {
            for (int i = 0; i < statSelectTexts.Length; i++) {
                statSelectTexts[i].color = Color.white;
            }
        }
    }
}
