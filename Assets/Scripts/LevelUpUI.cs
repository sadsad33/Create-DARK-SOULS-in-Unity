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
        public Text[] currentStatPoints;
        public Text requiredSoulsText;

        // 인덱스 6은 레벨, 7은 소지 소울
        private float[] initStats = new float[8];

        // 화톳불에서 레벨업을 선택하여 UI가 활성화 되면
        private void OnEnable() {
            
            currentStats = new Text[currentStatsBackground.transform.childCount];
            Transform curStatsWindow = transform.GetChild(0);
            for (int i = 0; i < currentStats.Length; i++) {
                currentStats[i] = curStatsWindow.GetChild(i).GetComponentInChildren<Text>();
            }

            Transform statSelectWindowTextSpace = transform.GetChild(1).GetChild(0).GetChild(2);
            currentStatPoints = new Text[statSelectWindowTextSpace.childCount];
            for (int i = 0; i < currentStatPoints.Length; i++) {
                currentStatPoints[i] = statSelectWindowTextSpace.GetChild(i).GetComponent<Text>();
            }

            GetCurrentStatPoints();

            // 플레이어의 스탯 포인트들을 UI에 출력
            for (int i = 0; i < 3; i++) {
                PrintCurrentStatPoints(i);
            }
            PrintCurrentStatPoints(7);

            for (int i = 0; i < currentStats.Length; i++) {
                PrintCurrentStats(i);
            }
        }

        // 각 스탯의 기존 최대치를 기록
        private void GetCurrentStatPoints() {
            initStats[0] = playerStatsManager.maxHealth;
            initStats[1] = playerStatsManager.maxFocus;
            initStats[2] = playerStatsManager.maxStamina;
            //initStats[3] = playerStatsManager.maxStrength;
            //initStats[4] = playerStatsManager.maxAttunement;
            //initStats[5] = playerStatsManager.maxFaith;
            initStats[6] = (float)playerStatsManager.level;
            initStats[7] = playerStatsManager.soulCount;
        }

        // 스탯 포인트에따른 현재 능력치들을 UI에 출력한다
        private void PrintCurrentStats(int index) {
            switch (index) {
                case 0:
                    currentStats[index].text = "Current Health : ";
                    currentStats[index].text += playerStatsManager.maxHealth.ToString();
                    // 최대값이 달라졌다면(올랐다면) 빨간색으로 변경
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
                    if (initStats[index] != playerStatsManager.level) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 7:
                    currentStats[index].text = "Current Souls : ";
                    currentStats[7].text += playerStatsManager.soulCount.ToString();
                    if (initStats[index] != playerStatsManager.soulCount) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
            }
        }

        // 현재 스탯 포인트들을 UI에 출력한다
        private void PrintCurrentStatPoints(int index) {
            switch (index) {
                case 0:
                    currentStatPoints[index].text = playerStatsManager.healthLevel.ToString();
                    break;
                case 1:
                    currentStatPoints[index].text = playerStatsManager.focusLevel.ToString();
                    break;
                case 2:
                    currentStatPoints[index].text = playerStatsManager.staminaLevel.ToString();
                    break;
                case 7:
                    requiredSoulsText.text = "Required Souls : ";
                    requiredSoulsText.text += (playerStatsManager.level * 65).ToString();
                    if (initStats[6] * 65 != playerStatsManager.level * 65) requiredSoulsText.color = Color.red;
                    else requiredSoulsText.color = Color.white;
                    break;
            }
        }

        // 해당 인덱스의 능력치값에 변경된 포인트값을 적용한다
        public void ChangeSelectedStat(int index, int value) {
            int delta = value;
            switch (index) {
                case 0:
                    delta -= playerStatsManager.healthLevel;
                    playerStatsManager.maxHealth = value * 10;
                    playerStatsManager.healthLevel = value;
                    break;
                case 1:
                    delta -= playerStatsManager.focusLevel;
                    playerStatsManager.maxFocus = value * 10;
                    playerStatsManager.focusLevel = value;
                    break;
                case 2:
                    delta -= playerStatsManager.staminaLevel;
                    playerStatsManager.maxStamina = value * 10;
                    playerStatsManager.staminaLevel = value;
                    break;
            }
            playerStatsManager.level += delta;
            PrintCurrentStats(index);
            PrintCurrentStats(6);
            PrintCurrentStats(7);
            PrintCurrentStatPoints(7);
        }

        public void SaveChange() {
            for (int i = 0; i < currentStatPoints.Length; i++) {
                currentStatPoints[i].color = Color.white;
            }
        }
    }
}
