using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulsLike {
    public abstract class WorldEvent : ScriptableObject {
        public int worldID;

        // 각 이벤트는 트리거를 갖는다.
        
        public int eventID;
        public bool isCleared;
        public WorldEventType worldEventType;
        public List<EventObject> eventObjectList;
        public abstract void SetAllEventObjects();
        public abstract void ActivateWorldEvent();
        public abstract void DeactivateWorldEvent();
    }
}
