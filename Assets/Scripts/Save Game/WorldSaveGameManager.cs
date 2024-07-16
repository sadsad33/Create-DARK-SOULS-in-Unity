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
            // �����ϴ� ��� ĳ���� �������� �ε�
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
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath; // ��ġ�� ���� �����θ� ��������
            saveGameDataWriter.dataSaveFileName = fileName;

            // ĳ���� �����͸� ���� ���̺� ���Ͽ� ����
            player.SaveCharacterDataToCurrentSaveData(ref currentCharacterSaveData);

            // json ���Ͽ� ���� ĳ���� �����͸� ���� ��ġ�� ����
            saveGameDataWriter.WriteCharacterDataToSaveFile(currentCharacterSaveData);

            Debug.Log("Saving Game...");
            Debug.Log("File Save As: " + fileName);
        }

        public void LoadGame() {
            // ������ ĳ���� ����(���̺� ����)�� ����Ͽ� �ε��� ��
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
                // �ε� ���� �ִٸ� ���⼭ �ε� ���� �����ְ� �����̴��� ����Ʈ������ progress�� ��Ÿ�� �� �ִ�
                yield return null;
            }

            // Scene�� �ε���� ĳ���� ������ �ҷ��;��� �׷��� ������ Scene�� �ε�Ǹ鼭 ĳ������ ��ǥ�� �ʱ�ȭ��
            player.LoadCharacterDataFromCurrentCharacterSaveData(ref currentCharacterSaveData);
        }
    }
}
