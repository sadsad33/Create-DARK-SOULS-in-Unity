using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class WorldSaveGameManager : MonoBehaviour {
        public static WorldSaveGameManager instance;
        [SerializeField] public PlayerManager playerManager;
        [Header("Save Data Writer")]
        SaveGameDataWriter saveGameDataWriter;

        [Header("Current Character Data")]
        public CharacterSaveData currentCharacterSaveData;
        [SerializeField]private string fileName;

        [Header("Save/Load")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start() {
            DontDestroyOnLoad(gameObject);
            // 존재하는 모든 캐릭터 정보들을 로드
        }

        private void Update() {
            if (saveGame) {
                saveGame = false;
                SaveGame();
            } else if(loadGame) {
                loadGame = false;
                // 진행상태 불러오기
            }
        }

        public void SaveGame() {
            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath; // 장치의 최적 저장경로를 지정해줌
            saveGameDataWriter.dataSaveFileName = fileName;

            // 캐릭터 데이터를 현재 세이브 파일에 전달
            playerManager.SaveCharacterDataToCurrentSaveData(ref currentCharacterSaveData);

            // json 파일에 현재 캐릭터 데이터를 쓰고 장치에 저장
            saveGameDataWriter.WriteCharacterDataToSaveFile(currentCharacterSaveData);

            Debug.Log("Saving Game...");
            Debug.Log("File Save As: " + fileName);
        }
    }
}
