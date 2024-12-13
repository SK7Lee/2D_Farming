using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace FarmSystem
{
    public enum ETargetObjectType
    {
        None = 0,
        River,
        Soil,
        Tree,
        Storage,
        Minerals
    }
    public class TargetingComponent : MonoBehaviour
    {
        [Header("Owner")]
        public Character owner;
        public Transform target;

        public Transform lastTarget;
        [Header("Current Target Object Type")]
        public ETargetObjectType currentTargetObjectType;

        [Header("Targeting Paramaters")]
        public float radius = 10f; // Bán kính của CircleCast
        public Vector2 direction = Vector2.right; // Hướng CircleCast
        public float distance = 10f; // Khoảng cách CircleCast

        //Delay Update
        public float timeExecuted = 0.0f;
        public float timeDelay = 1.0f;

        //Delay Update Fixed
        public float timeExecuteFixed = 0.0f;
        public float overTargetingTime = 4.0f;
        public float countTimeOverTarget = 0.0f;

        public bool isUpdateTarget = true;
        public void Awake()
        {
            owner = GetComponent<Character>();
        }
        private void Update()
        {
            //isCurrentSoilTargetInProcess();
            if (isUpdateTarget && Time.time > timeExecuted + timeDelay)
            {
                UpdatePriorityTarget();
                timeExecuted = Time.time;
            }
        }
        private void FixedUpdate()
        {
            ChangeTargetOverTargetingTime();
        }
        //Fixed
        public void ChangeTargetOverTargetingTime()
        {
            countTimeOverTarget += Time.deltaTime;
            if(countTimeOverTarget > overTargetingTime && !owner.isCarrying)
            {
                isUpdateTarget = true;
            }
        }
        public void UpdatePriorityTarget()
        {
            if (!owner.isCarrying)
            {
                bool hasTarget = ChangeTargetAI(ETargetObjectType.Soil, ESoilState.HasPlant, false);
                if (hasTarget)
                {
                    isUpdateTarget = false;
                    return;
                }
                hasTarget = ChangeTargetAI(ETargetObjectType.Soil, ESoilState.CanHarvest, true);
                if ((hasTarget))
                {
                    isUpdateTarget = false;
                    return;
                }
            }
            else
            {
                bool hasTarget = ChangeTargetAI(ETargetObjectType.Storage);
                if ((hasTarget))
                {
                    isUpdateTarget = false;
                    return;
                }
            }
        }
        //public void StartUpdateTargetAI(ETargetObjectType targetObjectType, ESoilState soilStateDeny = ESoilState.None, bool checkExist = false)
        //{
        //    if (C_UpdateTargetAI != null)
        //    {
        //        StopCoroutine(C_UpdateTargetAI);
        //    }
        //    C_UpdateTargetAI = StartCoroutine(UpdateTargetAI(targetObjectType, soilStateDeny, checkExist));
        //}
        //IEnumerator UpdateTargetAI(ETargetObjectType targetObjectType, ESoilState soilStateDeny = ESoilState.None, bool checkExist = false)
        //{

        //    while(lastTarget == target)
        //    {
        //        ChangeTargetAI(targetObjectType, soilStateDeny, checkExist);
        //        yield return new WaitForSeconds(timeDelay);
        //        yield return null;
        //    }
        //}
        public bool ChangeTargetAI(ETargetObjectType targetObjectType, ESoilState soilStateDeny = ESoilState.None, bool checkExist = false)
        {
            // Lấy tất cả các đối tượng nằm trong vùng CircleCast
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, direction, distance);

            Debug.Log("Changing Target");
            Transform nearestTarget = null;
            float nearestDistance = float.MaxValue;

            // Lặp qua tất cả các đối tượng bị phát hiện
            foreach (var hit in hits)
            {
                float distanceToHit = Vector2.Distance(transform.position, hit.transform.position);

                switch (targetObjectType)
                {
                    case ETargetObjectType.Storage:
                        Storage storage = hit.collider.GetComponent<Storage>();
                        if (storage != null && distanceToHit < nearestDistance)
                        {
                            nearestDistance = distanceToHit;
                            nearestTarget = hit.collider.transform;
                        }
                        break;

                    case ETargetObjectType.Soil:
                        Soil soil = hit.collider.GetComponent<Soil>();
                        // Kiểm tra điều kiện soilState
                        if (checkExist)
                        {
                            if (soil != null && ((soil.soilState & soilStateDeny) != 0) 
                                && ((soil.soilState & ESoilState.InProcess) == 0) 
                                && distanceToHit < nearestDistance)
                            {
                                nearestDistance = distanceToHit;
                                nearestTarget = hit.collider.transform;
                            }
                        }
                        else
                        {
                            if (soil != null && ((soil.soilState & soilStateDeny) == 0) 
                                && ((soil.soilState & ESoilState.InProcess) == 0) 
                                && distanceToHit < nearestDistance)
                            {
                                nearestDistance = distanceToHit;
                                nearestTarget = hit.collider.transform;
                            }
                        }
                        break;

                    case ETargetObjectType.River:
                        River river = hit.collider.GetComponent<River>();
                        if (river != null && distanceToHit < nearestDistance)
                        {
                            nearestDistance = distanceToHit;
                            nearestTarget = hit.collider.transform;
                        }
                        break;
                    default:
                        break;
                }
            }

            // Cập nhật target gần nhất nếu tìm thấy
            if (nearestTarget != null)
            {
                target = nearestTarget;

                if(lastTarget == null || lastTarget != target)
                {
                    lastTarget = target;
                    countTimeOverTarget = 0.0f;
                }
                Debug.Log($"New target set: {target.name}");
                return true;
            }
            else
            {
                Debug.Log("No suitable target found.");
            }
            return false;
        }

        public bool isCurrentSoilTargetInProcess()
        {
            CharacterAI ownerAI = owner as CharacterAI;
            if (ownerAI != null && target != null)
            {
                Soil soil = target.GetComponent<Soil>();
                if (soil != null && (soil.soilState & ESoilState.InProcess) != 0)
                {
                    isUpdateTarget = true;
                    return true;
                }
            }
            return false;
        }

        
        private void OnDrawGizmos()
        {
            // Vẽ Gizmos để dễ dàng kiểm tra hình tròn trong Scene
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}