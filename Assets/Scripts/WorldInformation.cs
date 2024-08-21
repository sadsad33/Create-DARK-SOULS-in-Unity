using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public class WorldInformation : ScriptableObject {
        // 해당 월드에서 드롭되는 아이템 목록, 출현하는 적 목록, 보스 목록, 이벤트 목록 등 ...
        public int worldID;
        public bool hasBeenCleared;
    }
}
