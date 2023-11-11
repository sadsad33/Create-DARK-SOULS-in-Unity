using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // Item이란 이름의 ScriptableObject의 형식을 만든다.
    // ScriptableObject는 속성으로 Sprite와 string을 가진다.
    public class Item : ScriptableObject {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
    }
}
