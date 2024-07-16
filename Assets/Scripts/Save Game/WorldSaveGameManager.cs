using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulsLike {
    public class WorldSaveGameManager : MonoBehaviour {
        public static WorldSaveGameManager instance;
        public PlayerManager player;
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
                LoadGame();
            }
        }

        public void SaveGame() {
            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath; // 장치의 최적 저장경로를 지정해줌
            saveGameDataWriter.dataSaveFileName = fileName;

            // 캐릭터 데이터를 현재 세이브 파일에 전달
            player.SaveCharacterDataToCurrentSaveData(ref currentCharacterSaveData);

            // json 파일에 현재 캐릭터 데이터를 쓰고 장치에 저장
            saveGameDataWriter.WriteCharacterDataToSaveFile(currentCharacterSaveData);

            Debug.Log("Saving Game...");
            Debug.Log("File Save As: " + fileName);
        }

        public void LoadGame() {
            // 선택한 캐릭터 슬롯(세이브 슬롯)에 기반하여 로드할 것
            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveGameDataWriter.dataSaveFileName = fileName;
            currentCharacterSaveData = saveGameDataWriter.LoadCharacterDataFromJson();

            StartCoroutine(LoadWorldSceneAsyncronously());
        }

        IEnumerator LoadWorldSceneAsyncronously() {
            if (player == null) player = FindObjectOfType<PlayerManager>();

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);

            while (!loadOperation.isDone) {
                float loadingProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
                // 로딩 씬이 있다면 여기서 로딩 씬을 보여주고 슬라이더나 이펙트등으로 progress를 나타낼 수 있다
                yield return null;
            }

            // Scene이 로드된후 캐릭터 정보를 불러와야함 그렇지 않으면 Scene이 로드되면서 캐릭터의 좌표도 초기화됨
            player.LoadCharacterDataFromCurrentCharacterSaveData(ref currentCharacterSaveData);
        }
    }
}
