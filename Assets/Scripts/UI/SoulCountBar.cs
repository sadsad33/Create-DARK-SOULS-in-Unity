using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    public class SoulCountBar : MonoBehaviour {
        public Text soulCountText;
        public void SetSoulCountText(int soulCount) {
            soulCountText.text = soulCount.ToString();
        }
    }
}
