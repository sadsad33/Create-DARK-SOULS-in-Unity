using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    // Item�̶� �̸��� ScriptableObject�� ������ �����.
    // ScriptableObject�� �Ӽ����� Sprite�� string�� ������.
    public class Item : ScriptableObject {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
        public string flavorText;
        public int itemID;
    }
}
