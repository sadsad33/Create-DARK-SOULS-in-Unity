using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SoulsLike {
    [CreateAssetMenu(menuName = "AI/NPC Actions/NPC Script")]
    public class NPCScript : ScriptableObject {
        public int questCode;
        public bool isStart;
        public bool isEnd;
        public bool isQuest;
        public string script;
    }
}