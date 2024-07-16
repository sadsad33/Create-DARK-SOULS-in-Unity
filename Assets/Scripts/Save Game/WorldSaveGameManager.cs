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
            // �����ϴ� ��� ĳ���� �������� �ε�
        }

        private void Update() {
            if (saveGame) {
                saveGame = false;
                SaveGame();
            } else if(loadGame) {
                loadGame = false;
                // ������� �ҷ�����
            }
        }

        public void SaveGame() {
            saveGameDataWriter = new SaveGameDataWriter();
            saveGameDataWriter.saveDataDirectoryPath = Application.persistentDataPath; // ��ġ�� ���� �����θ� ��������
            saveGameDataWriter.dataSaveFileName = fileName;

            // ĳ���� �����͸� ���� ���̺� ���Ͽ� ����
            playerManager.SaveCharacterDataToCurrentSaveData(ref currentCharacterSaveData);

            // json ���Ͽ� ���� ĳ���� �����͸� ���� ��ġ�� ����
            saveGameDataWriter.WriteCharacterDataToSaveFile(currentCharacterSaveData);

            Debug.Log("Saving Game...");
            Debug.Log("File Save As: " + fileName);
        }
    }
}
