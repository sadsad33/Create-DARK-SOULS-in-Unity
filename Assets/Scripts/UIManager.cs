using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace sg {
    public class UIManager : MonoBehaviour {
        public GameObject selectWindow;
        public void OpenSelectWindow() {
            selectWindow.SetActive(true);
        }

        public void CloseSelectWindow() {
            selectWindow.SetActive(false);
        }
    }
}
