using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    // Item�̶� �̸��� ScriptableObject�� �����.
    // ScriptableObject�� �Ӽ����� Sprite�� string�� ������.
    public class Item : ScriptableObject {
        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
    }
}
