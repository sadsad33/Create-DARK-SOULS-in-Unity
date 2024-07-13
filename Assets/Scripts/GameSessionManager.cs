using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace SoulsLike {
    public class GameSessionManager : MonoBehaviour {

        public static GameSessionManager instance;

        // ��Ʈ��ũ �����
        [Header("Debug Join Game")]
        [SerializeField] bool startGameAsClient;
        [SerializeField] bool shutdownNetwork;

        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Update() {
            if (startGameAsClient) {
                startGameAsClient = false;
                NetworkManager.Singleton.StartClient();
            }
            if (shutdownNetwork) {
                shutdownNetwork = false;
                NetworkManager.Singleton.Shutdown();
            }
        }
    }
}
