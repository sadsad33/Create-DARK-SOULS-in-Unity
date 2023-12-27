using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // �ʿ� �����ϴ� �̺�Ʈ ����
    public class WorldEventManager : MonoBehaviour {

        public BossHealthBar bossHealthBar;
        public BossManager boss;

        public bool bossFightIsActive; // ������ ���� ����
        public bool bossHasBeenAwakened; // ���� �ൿ ���� Ȥ�� �ƽ� ���
        public bool bossHasBeenDefeated; // ���� ����

        private void Awake() {
            bossHealthBar = FindObjectOfType<BossHealthBar>();
        }

        public void ActivateBossFight() {
            bossFightIsActive = true;
            bossHasBeenAwakened = true;
            bossHealthBar.SetUIHealthBarToActive();
            // �Ȱ��� ����
        }

        public void BossHasBeenDefeated() {
            bossHasBeenDefeated = true;
            bossFightIsActive = false;
            // �Ȱ��� �Ҹ�
        }
    }
}
