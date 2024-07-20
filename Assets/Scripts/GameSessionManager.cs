using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace SoulsLike {
    public class GameSessionManager : MonoBehaviour {

        public static GameSessionManager instance;

        // 네트워크 실험용
        [Header("Debug Join Game")]
        [SerializeField] bool startGameAsClient;
        [SerializeField] bool shutdownNetwork;

        [Header("Player In Game")]
        public List<PlayerManager> players = new List<PlayerManager>();
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

        // 게임에 참가중인 모든 플레이어를 리스트에 등록
        public void AddPlayerToActivePlayerList(PlayerManager player) {
            if (!players.Contains(player)) players.Add(player);

            // 플레이어 리스트의 항목중 비어있는 항목은 삭제
            for (int i = players.Count - 1; i > -1; i--) {
                if (players[i] == null) {
                    players.RemoveAt(i);
                }
            }
        }
    }
}
