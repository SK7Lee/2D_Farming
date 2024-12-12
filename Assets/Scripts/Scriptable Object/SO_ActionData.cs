using UnityEngine;
using System.Collections.Generic;
namespace FarmSystem
{

    [System.Serializable]
    public enum EToolType
    {
        Pickaxe,
        Axe,
        WaterCan,
        Shovel,
        Rod,
        Sword,
        Hammer,
        ByHand
    }
    [System.Serializable]
    public struct ActionData
    {
        public string name;
        public EToolType toolRequired;
        public ETargetObjectType targetObjectTypeRequired;
        public int playAnimTimes;
    }
    [CreateAssetMenu(fileName = "Action Data", menuName = "Farm System/Data/Action Data")]
    public class SO_ActionData : ScriptableObject
    {
        public ActionData data;
    }
}
