using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    // 맵에 존재하는 이벤트 관리
    public class WorldEventManager : MonoBehaviour {
        public static WorldEventManager instance;

        public List<WorldEvent> worldEventList;
        public List<WorldEvent> eventsInCurrentScene;
        public WorldEvent currentEvent;
        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(this);

        }

        private void Start() {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChangedEventHandler;
        }

        // 씬이 변경되면 이벤트 목록에서 현재 씬에서 일어날 수 있는 이벤트의 목록을 추림
        private void SceneChangedEventHandler(UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene) {
            for (int i = 0; i < worldEventList.Count; i++) {
                if (worldEventList[i].worldID == newScene.buildIndex) {
                    eventsInCurrentScene.Add(worldEventList[i]);
                }
            }
            SetAllEventsInThisScene();
        }

        // 현재 씬에서 일어날 수 있는 모든 이벤트를 세팅
        public void SetAllEventsInThisScene() {
            for (int i = 0; i < eventsInCurrentScene.Count; i++) {
                //Debug.Log("현재 씬의 모든 이벤트 세팅");
                eventsInCurrentScene[i].SetAllEventObjects();
            }
        }

        // 해당하는 이벤트의 조건이 완료됐다면 이벤트 진행
        public void ProcessCurrentWorldEvent(WorldEvent worldEvent) {
            switch (worldEvent.worldEventType) {
                case WorldEventType.BossFight:
                    currentEvent = worldEvent;
                    worldEvent.ActivateWorldEvent();
                    break;
                default:
                    break;
            }
        }

        public void TerminateCurrentEvent() {
            currentEvent.DeactivateWorldEvent();
        }

        // 완료된 이벤트는 목록에서 제거
        public void RemoveCompletedEventFromList(WorldEvent worldEvent) {
            worldEvent.isCleared = true;
            eventsInCurrentScene.Remove(worldEvent);
        }
    }
}
