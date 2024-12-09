using UnityEngine;

namespace FarmSystem
{
    public enum ETargetObjectType
    {
        None = 0,
        Water,
        Land,
        Tree,
        Minerals

    }
    public class TargetingComponent : MonoBehaviour
    {
        [Header("Owner")]
        public Character owner;
        public Transform target;

        [Header("Current Target Object Type")]
        public ETargetObjectType currentTargetObjectType;

        public void Awake()
        {
            owner = GetComponent<Character>();
        }

    }
}