using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class BonfireWindow : MonoBehaviour {
        
        Button leaveButton;

        private void Awake() {
            leaveButton = transform.GetChild(0).GetChild(0).GetComponentInChildren<Button>();
            leaveButton.onClick.AddListener(UIManager.instance.player.playerInteractionManager.LeaveBonfire);
        }
    }
}
