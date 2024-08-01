using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace SoulsLike {
    public class MainMenuManager : MonoBehaviour {
        [SerializeField] int worldSceneIndex;
        public void StartNetwokrAsHost() {
            NetworkManager.Singleton.StartHost();            
        }

        public void StartNewGames() {
            SceneManager.LoadScene(worldSceneIndex);
            PlayerManager player = FindObjectOfType<PlayerManager>();
            //if (player != null) {
            //    UIManager.instance.transform.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            //}
        }
    }
}