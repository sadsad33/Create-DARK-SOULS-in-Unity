using UnityEngine;
using System;
using System.IO;

namespace SoulsLike {
    public class SaveGameDataWriter{
        public string saveDataDirectoryPath = "";
        public string dataSaveFileName = "";

        public CharacterSaveData LoadCharacterDataFromJson() {
            string savePath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);
            CharacterSaveData loadedSaveData = null;
            if (File.Exists(savePath)) {
                try {
                    string saveDataToLoad = "";
                    using (FileStream stream = new FileStream(savePath, FileMode.Open)) {
                        using (StreamReader reader = new StreamReader(stream)) {
                            saveDataToLoad = reader.ReadToEnd();
                        }
                    }

                    loadedSaveData = JsonUtility.FromJson<CharacterSaveData>(saveDataToLoad);
                } catch (Exception e) {
                    Debug.LogWarning(e.Message);
                }
            } else {
                Debug.Log("SAVE FILE DOES NOT EXIST");
            }
            return loadedSaveData;
        }

        public void WriteCharacterDataToSaveFile(CharacterSaveData characterData) {

            string savePath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("SAVE PATH = " + savePath);

                // C# ���� ������ ������Ʈ�� JSON���� ����ȭ
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // �ý��ۿ� ���� �ۼ�
                using (FileStream stream = new FileStream(savePath, FileMode.Create)) {
                    using (StreamWriter writer = new StreamWriter(stream)) {
                        writer.Write(dataToStore);
                    }
                }
            } catch (Exception e) {
                Debug.LogError("ERROR WHILE TRYING TO SAVE DATA, GAME COULD NOT BE SAVED" + e);
            }
        }

        public void DeletedSaveFile() {
            File.Delete(Path.Combine(saveDataDirectoryPath, dataSaveFileName));
        }

        public bool CheckSaveFileExists() {
            if (File.Exists(Path.Combine(saveDataDirectoryPath, dataSaveFileName))) return true;
            else return false;   
        }
    }
}
