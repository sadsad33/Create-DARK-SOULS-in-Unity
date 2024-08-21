using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    [CreateAssetMenu(menuName = "World/WorldEvent/Boss Fight")]
    public class BossFightEvent : WorldEvent {
        public int bossCharacterID;

        // 플레이어가 보스전을 시작하기 전 모든 이벤트 오브젝트만 활성화 된 상태
        public override void SetAllEventObjects() {
            foreach (EventObject obj in eventObjectList) {
                //Debug.Log("이벤트의 모든 오브젝트 세팅");
                obj.ActivateEventObject();
            }
        }

        // 보스전 활성화(플레이어가 이벤트를 시작한 상태)
        public override void ActivateWorldEvent() {
            //Debug.Log("보스전 시작");

            BossHealthBar bossHealthBar = UIManager.instance.bossFightUI.GetComponent<BossHealthBar>();
            BossManager bossCharacter = WorldAIManager.instance.GetAICharacterByID(bossCharacterID) as BossManager;
            if (bossCharacter != null) {
                bossHealthBar.SetBossName(bossCharacter.bossName);
                bossHealthBar.SetUIHealthBarToActive();
                bossCharacter.aiStatsManager.enemyHealthBar = bossHealthBar;
                bossCharacter.aiStatsManager.enemyHealthBar.SetMaxHealth(bossCharacter.aiStatsManager.currentHealth);
            }

            foreach (EventObject obj in eventObjectList) {
                FogWall fogWall = obj.instantiatedModel.GetComponent<FogWall>();
                //Debug.Log("FogWall : " + fogWall);
                if (fogWall != null) {
                    fogWall.CloseFogWall();
                }
            }
        }
        

        // 보스전 비활성화(보스전이 비활성화 되는 경우는 보스가 격파된 후 뿐)
        public override void DeactivateWorldEvent() {
            Debug.Log("보스전 끝");
            isCleared = true;
            UIManager.instance.bossFightUI.GetComponent<BossHealthBar>().SetUIHealthBarToInactive();
            foreach (EventObject obj in eventObjectList) {
                obj.DeactivateEventObject();
            }
            WorldEventManager.instance.RemoveCompletedEventFromList(this);
        }
    }
}
