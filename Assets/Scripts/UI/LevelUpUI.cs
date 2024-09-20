using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulsLike {
    public class LevelUpUI : MonoBehaviour {
        //public PlayerStatsManager playerStatsManager;
        //public PlayerWeaponSlotManager playerWeaponSlotManager;
        public StatSelectUI statSelectUI;
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
            initStats[0] = UIManager.instance.player.playerStatsManager.maxHealth;
            initStats[1] = UIManager.instance.player.playerStatsManager.maxFocus;
            initStats[2] = UIManager.instance.player.playerStatsManager.maxStamina;
            //initStats[3] = playerStatsManager.maxStrength;
            //initStats[4] = playerStatsManager.maxAttunement;
            //initStats[5] = playerStatsManager.maxFaith;
            initStats[6] = (float)UIManager.instance.player.playerStatsManager.level;
            initStats[7] = UIManager.instance.player.playerStatsManager.soulCount;
        }

        // 스탯 포인트에따른 현재 능력치들을 UI에 출력한다
        private void PrintCurrentStats(int index) {
            switch (index) {
                case 0:
                    currentStats[index].text = "Current Health : ";
                    currentStats[index].text += UIManager.instance.player.playerStatsManager.maxHealth.ToString();
                    // 최대값이 달라졌다면(올랐다면) 빨간색으로 변경
                    if (initStats[index] != UIManager.instance.player.playerStatsManager.maxHealth) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 1:
                    currentStats[index].text = "Current Focus : ";
                    currentStats[index].text += UIManager.instance.player.playerStatsManager.maxFocus.ToString();
                    if (initStats[index] != UIManager.instance.player.playerStatsManager.maxFocus) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 2:
                    currentStats[index].text = "Current Stamina : ";
                    currentStats[index].text += UIManager.instance.player.playerStatsManager.maxStamina.ToString();
                    if (initStats[index] != UIManager.instance.player.playerStatsManager.maxStamina) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 3:
                    currentStats[index].text = "Current ATK(Right Hand) : ";
                    currentStats[index].text += UIManager.instance.player.playerWeaponSlotManager.rightHandSlot.currentWeapon.physicalDamage.ToString();
                    break;
                case 4:
                    currentStats[index].text = "Current ATK(Left Hand) : ";
                    currentStats[index].text += UIManager.instance.player.playerWeaponSlotManager.leftHandSlot.currentWeapon.physicalDamage.ToString();
                    break;
                case 5:
                    currentStats[index].text = "Current DEF : ";
                    currentStats[5].text += (Mathf.Round(UIManager.instance.player.playerStatsManager.totalPhysicalDamageDefenseRate * 100)).ToString() + '%';
                    break;
                case 6:
                    currentStats[index].text = "Current Level : ";
                    currentStats[6].text += UIManager.instance.player.playerStatsManager.level.ToString();
                    if (initStats[index] != UIManager.instance.player.playerStatsManager.level) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
                case 7:
                    currentStats[index].text = "Current Souls : ";
                    currentStats[7].text += UIManager.instance.player.playerStatsManager.soulCount.ToString();
                    if (initStats[index] != UIManager.instance.player.playerStatsManager.soulCount) currentStats[index].color = Color.red;
                    else currentStats[index].color = Color.white;
                    break;
            }
        }

        // 현재 스탯 포인트들을 UI에 출력한다
        private void PrintCurrentStatPoints(int index) {
            switch (index) {
                case 0:
                    currentStatPoints[index].text = UIManager.instance.player.playerStatsManager.healthLevel.ToString();
                    break;
                case 1:
                    currentStatPoints[index].text = UIManager.instance.player.playerStatsManager.focusLevel.ToString();
                    break;
                case 2:
                    currentStatPoints[index].text = UIManager.instance.player.playerStatsManager.staminaLevel.ToString();
                    break;
                case 7:
                    requiredSoulsText.text = "Required Souls : ";
                    requiredSoulsText.text += (UIManager.instance.player.playerStatsManager.level * 65).ToString();
                    if (initStats[6] * 65 != UIManager.instance.player.playerStatsManager.level * 65) requiredSoulsText.color = Color.red;
                    else requiredSoulsText.color = Color.white;
                    break;
            }
        }

        // 해당 인덱스의 능력치값에 변경된 포인트값을 적용한다
        public void ChangeSelectedStat(int index, int value) {
            int delta = value;
            switch (index) {
                case 0:
                    delta -= UIManager.instance.player.playerStatsManager.healthLevel;
                    UIManager.instance.player.playerStatsManager.maxHealth = value * 10;
                    UIManager.instance.player.playerStatsManager.healthLevel = value;
                    break;
                case 1:
                    delta -= UIManager.instance.player.playerStatsManager.focusLevel;
                    UIManager.instance.player.playerStatsManager.maxFocus = value * 10;
                    UIManager.instance.player.playerStatsManager.focusLevel = value;
                    break;
                case 2:
                    delta -= UIManager.instance.player.playerStatsManager.staminaLevel;
                    UIManager.instance.player.playerStatsManager.maxStamina = value * 10;
                    UIManager.instance.player.playerStatsManager.staminaLevel = value;
                    break;
            }
            UIManager.instance.player.playerStatsManager.level += delta;
            PrintCurrentStats(index);
            PrintCurrentStats(6);
            PrintCurrentStats(7);
            PrintCurrentStatPoints(7);
        }

        public void SaveChange() {
            for (int i = 0; i < currentStatPoints.Length; i++) {
                currentStatPoints[i].color = Color.white;
            }
            for (int i = 0; i < statSelectUI.upButtonSet.Length; i++) {
                statSelectUI.upButtonSet[i].GetComponent<LevelUpButton>().initValue = int.Parse(currentStatPoints[i].text);
                statSelectUI.downButtonSet[i].GetComponent<LevelUpButton>().initValue = int.Parse(currentStatPoints[i].text);
            }
        }
    }
}
