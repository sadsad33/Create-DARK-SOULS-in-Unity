using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sg {
    [CreateAssetMenu(menuName = "Items/Equipment/Helmet Equipment")]
    public class HelmetEquipment : EquipmentItem {
        public string helmetModelName; // 게임 내에서 UI에 표시될 아이템의 이름이 아닌 데이터 상에서의 이름
    }
}