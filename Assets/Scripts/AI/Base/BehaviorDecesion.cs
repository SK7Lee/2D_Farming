using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace FarmSystem
{
    [System.Serializable]
    public struct SNormalActionData
    {
        public string name;
        public NormalAction action;
        public Condition condition;
        public int priority;
    }
    [System.Serializable]
    public struct SCombatActionData
    {
        public string name;
        public CombatAction action;
        public Condition condition;
        public int priority;
    }
    [System.Serializable]
    public struct SFarmingActionData
    {
        public string name;
        public FarmingAction action;
        public Condition condition;
        public int priority;
    }
    [System .Serializable]
    public enum EBehaviorState
    {
        None = 0,
        Executing,
        Finish
    }
    public class BehaviorDecesion : MonoBehaviour
    {
        [Header("Owner")]
        public CharacterEnemy ownerAI;
        [Header("Current Behavior")]
        public EBehaviorState currentBehaviorState = EBehaviorState.None;
        public SCombatActionData currentCombatActionData;
        public SFarmingActionData currentFarmingActionData;
        public SNormalActionData currentNormalActionData;

        public Action currentAction;

        [Header("Actions")]
        public List<SNormalActionData> normalActionDatas;
        public List<SCombatActionData> combatActionDatas;
        public List<SFarmingActionData> farmingActionDatas;

        //Coroutine
        Coroutine C_ExecuteBehavior;

        public float timeExecuted = 0.0f;
        public float timeDelay = 1.5f;

        private void Awake()
        {
            ownerAI = GetComponent<CharacterEnemy>();
        }
        private void Start()
        {
            ResetAllActionsState();
            StartExecuteBehavior();
        }
        public void ResetAllActionsState()
        {
            //Normal Actions
            if (normalActionDatas.Count > 0)
            {
                foreach (var normalActionData in normalActionDatas)
                {
                    normalActionData.action.EBehaviorCooldown = EBehaviorCooldown.Ready;
                }
            }
            if (combatActionDatas.Count > 0)
            {
                foreach (var combatActiobData in combatActionDatas)
                {
                    combatActiobData.action.EBehaviorCooldown = EBehaviorCooldown.Ready;
                }
            }

            if (farmingActionDatas.Count > 0)
            {
                foreach (var farmingActionData in farmingActionDatas)
                {
                    farmingActionData.action.EBehaviorCooldown = EBehaviorCooldown.Ready;
                }
            }
        }
        public void StartExecuteBehavior()
        {
            if(C_ExecuteBehavior != null)
            {
                StopCoroutine(C_ExecuteBehavior);
            }
            C_ExecuteBehavior = StartCoroutine(ExecuteBehavior());
        }
        IEnumerator ExecuteBehavior()
        {
            while (ownerAI.energy > 0)
            {
                yield return new WaitWhile(() => currentBehaviorState == EBehaviorState.Executing);
                EvaluateAndExecuteBehavior();
                yield return new WaitWhile(() => currentBehaviorState == EBehaviorState.Executing);
            }
        }
        public void EvaluateAndExecuteBehavior()
        {
            if(Time.time > timeExecuted + timeDelay) { 
                timeExecuted = Time.time;
            List<SNormalActionData> suffleNormalActions = new List<SNormalActionData>();
            List<SFarmingActionData> suffleFarmingActions = new List<SFarmingActionData>();
            List<SCombatActionData> suffleCombatActions = new List<SCombatActionData>();

            //Check and Get All Actions - Skill Actions - Skirmush Actions has valid conditions and 0s Cooldown
            if (normalActionDatas.Count > 0)
            {
                foreach (var normalActionData in normalActionDatas)
                {
                    if (EvaluateCondition(normalActionData.condition))
                    {
                        if (normalActionData.action.EBehaviorCooldown == EBehaviorCooldown.Ready)
                        {
                            suffleNormalActions.Add(normalActionData);
                        }
                    }
                }
            }
            if (farmingActionDatas.Count > 0)
            {
                foreach (var farmingActionData in farmingActionDatas)
                {
                    if (EvaluateCondition(farmingActionData.condition))
                    {
                        if (farmingActionData.action.EBehaviorCooldown == EBehaviorCooldown.Ready)
                        {
                            suffleFarmingActions.Add(farmingActionData);
                        }
                    }
                }
            }
            if (combatActionDatas.Count > 0)
            {
                foreach (var combatActionData in combatActionDatas)
                {
                    if (EvaluateCondition(combatActionData.condition))
                    {
                        if (combatActionData.action.EBehaviorCooldown == EBehaviorCooldown.Ready)
                        {
                            suffleCombatActions.Add(combatActionData);
                        }
                    }
                }
            }


            //If Suffle Action - Skirmish Action - Skill Action have value then we EXECUTE the 
            //action has HIGHEST priorty value
            if (combatActionDatas.Count > 0)
            {
                //Get Action has highest priority
                SCombatActionData actionOutput = new SCombatActionData();
                int maxPriority = 0;
                foreach (var combatActionData in suffleCombatActions)
                {
                    if (combatActionData.priority >= maxPriority)
                    {
                        maxPriority = combatActionData.priority;
                    }
                }
                foreach (var combatActionData in suffleCombatActions)
                {
                    if (combatActionData.priority == maxPriority)
                    {
                        actionOutput = combatActionData;
                        break;
                    }
                }

                currentCombatActionData = actionOutput;

                currentAction = currentCombatActionData.action;

                //StartCoroutine(StartCooldown(currentCombatActionData.action));

                currentCombatActionData.action.Execute(ownerAI);
                return;
            }

            if (suffleFarmingActions.Count > 0)
            {
                //Get Action has highest priority
                SFarmingActionData actionOutput = new SFarmingActionData();
                int maxPriority = 0;
                foreach (var farmingAction in suffleFarmingActions)
                {
                    if (farmingAction.priority >= maxPriority)
                    {
                        maxPriority = farmingAction.priority;
                    }
                }
                foreach (var farmingAction in suffleFarmingActions)
                {
                    if (farmingAction.priority == maxPriority)
                    {
                        actionOutput = farmingAction;
                        break;
                    }
                }

                currentFarmingActionData = actionOutput;

                currentAction = currentCombatActionData.action;

                //StartCoroutine(StartCooldown(currentFarmingActionData.action));

                currentFarmingActionData.action.Execute(ownerAI);
                return;
            }
                if (suffleNormalActions.Count > 0)
                {
                    //Get Action has highest priority
                    SNormalActionData actionOutput = new SNormalActionData();
                    int maxPriority = 0;
                    foreach (var normalAction in suffleNormalActions)
                    {
                        if (normalAction.priority >= maxPriority)
                        {
                            maxPriority = normalAction.priority;
                        }
                    }
                    foreach (var normalAction in suffleNormalActions)
                    {
                        if (normalAction.priority == maxPriority)
                        {
                            actionOutput = normalAction;
                            break;
                        }
                    }

                    currentNormalActionData = actionOutput;

                    currentAction = currentCombatActionData.action;

                    //StartCoroutine(StartCooldown(currentNormalActionData.action));

                    currentNormalActionData.action.Execute(ownerAI);
                    return;
                }
            }
        }
        #region ActionCooldown
        IEnumerator StartCooldown(SO_Behavior action)
        {
            action.EBehaviorCooldown = EBehaviorCooldown.WaitForCooldown;
            float elapsedTime = 0.0f;
            while (elapsedTime < action.cooldown)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            action.EBehaviorCooldown = EBehaviorCooldown.Ready;

        }
        //IEnumerator StartCooldown(SkirmishAction skirmishAction)
        //{
        //    skirmishAction.EBehaviorCooldown = EBehaviorCooldown.WaitForCooldown;
        //    float elapsedTime = 0.0f;
        //    while(elapsedTime < skirmishAction.Cooldown)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        yield return null;
        //    }
        //    skirmishAction.EBehaviorCooldown = EBehaviorCooldown.Ready;

        //}
        //IEnumerator StartCooldown(SkillAction skillAction)
        //{
        //    skillAction.EBehaviorCooldown = EBehaviorCooldown.WaitForCooldown;
        //    float elapsedTime = 0.0f;
        //    while (elapsedTime < skillAction.Cooldown)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        yield return null;
        //    }
        //    skillAction.EBehaviorCooldown = EBehaviorCooldown.Ready;

        //}
        #endregion

        #region CheckValue
        private bool EvaluateCheckBool(BoolValue boolValue)
        {
            bool variableValue = GetBoolValue(boolValue.variableType);
            switch (boolValue.compareType)
            {
                case ECompareType.Equal:
                    return variableValue == boolValue.value;

                default:
                    return false;
            }
        }
        private bool EvaluateCheckInt(IntValue intValue)
        {
            int variableValue = GetIntValue(intValue.variableType);
            switch (intValue.compareType)
            {
                case ECompareType.Equal:
                    return variableValue == intValue.value;

                case ECompareType.GreaterThanOrEqual:
                    return variableValue > intValue.value
                        || variableValue == intValue.value;

                case ECompareType.GreaterThan:
                    return variableValue > intValue.value;

                case ECompareType.LessThanOrEqual:
                    return variableValue < intValue.value
                        || variableValue == intValue.value;

                case ECompareType.LessThan:
                    return variableValue < intValue.value;

                default:
                    return false;
            }
        }
        private bool EvaluateCheckFloat(FloatValue floatValue)
        {
            float variableValue = GetFloatValue(floatValue.variableType);
            switch (floatValue.compareType)
            {
                case ECompareType.Equal:
                    return Mathf.Approximately(variableValue, floatValue.value);

                case ECompareType.GreaterThan:
                    return variableValue > floatValue.value;

                case ECompareType.GreaterThanOrEqual:
                    return variableValue > floatValue.value
                        || Mathf.Approximately(variableValue, floatValue.value);

                case ECompareType.LessThan:
                    return variableValue < floatValue.value;

                case ECompareType.LessThanOrEqual:
                    return variableValue < floatValue.value
                        || Mathf.Approximately(variableValue, floatValue.value);

                default:
                    return false;
            }
        }
        private bool EvaluateCondition(Condition condition)
        {
            foreach (var checkBool in condition.boolValue)
            {
                if (!EvaluateCheckBool(checkBool)) return false;
            }

            foreach (var checkInt in condition.intValue)
            {
                if (!EvaluateCheckInt(checkInt)) return false;
            }

            foreach (var checkFloat in condition.floatValue)
            {
                if (!EvaluateCheckFloat(checkFloat)) return false;
            }

            return true;
        }
        private bool GetBoolValue(EBoolType type)
        {
            switch (type)
            {
                case EBoolType.Step1_HasTarget:
                    {
                        return ownerAI.targetingComponent.target;
                    }
                case EBoolType.Step2_TargetIsSoil:
                    {
                        if (ownerAI.targetingComponent.target.GetComponent<Soil>() != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EBoolType.Step2_TargetIsStorage:
                    {
                        if (ownerAI.targetingComponent.target.GetComponent<Storage>() != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EBoolType.Step3_SoilNoPlant:
                    {
                        if (ownerAI.targetingComponent.target.GetComponent<Soil>() != null)
                        {
                            Soil soil = ownerAI.targetingComponent.target.GetComponent<Soil>();
                            if((soil.soilState & ESoilState.HasPlant) == 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EBoolType.Step3_SoilHasPlant:
                    {
                        if (ownerAI.targetingComponent.target.GetComponent<Soil>() != null)
                        {
                            Soil soil = ownerAI.targetingComponent.target.GetComponent<Soil>();
                            if ((soil.soilState & ESoilState.HasPlant) != 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EBoolType.Step4_PlantCanHarvest:
                    {
                        if (ownerAI.targetingComponent.target.GetComponent<Soil>() != null)
                        {
                            Soil soil = ownerAI.targetingComponent.target.GetComponent<Soil>();
                            if ((soil.soilState & ESoilState.HasPlant) != 0)
                            {
                                if (soil.currentTree.treeCurrentStage.isFinalStage) return true;
                                else return false;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case EBoolType.Ground:
                    {
                        return ownerAI.isGround;
                    }
                case EBoolType.Carrying:
                    {
                        return ownerAI.isCarrying;
                    }
                case EBoolType.Jumpable:
                    {
                        return ownerAI.isGround;
                    }


                default:
                    return false;
            }
        }
        private int GetIntValue(EIntType type)
        {
            switch (type)
            {
                default:
                    return 0;
            }
        }
        private float GetFloatValue(EFloatType type)
        {
            switch (type)
            {
                case EFloatType.Distance:
                    {
                        return Vector2.Distance(ownerAI.targetingComponent.target.transform.position, gameObject.transform.position);
                    }
                //case EFloatType.HP:
                //    {
                //        CharacterEnemy enemy = owner as CharacterEnemy;
                //        if (enemy != null)
                //        {
                //            float raito = enemy.attributeManager.GetAttributeData.health.currentValue * 100.0f / enemy.attributeManager.GetAttributeData.health.maxValue;
                //            return raito;
                //        }
                //        return 100.0f;
                //    }

                //case EFloatType.AngleToTarget:
                //    {
                //        CharacterEnemy enemy = owner as CharacterEnemy;
                //        if (enemy != null)
                //        {
                //            Vector3 direction = Util.GetDirection(enemy.transform.position, enemy.navMeshAgent.steeringTarget);
                //            float angle = Vector3.Angle(enemy.transform.forward, direction);
                //            return angle;
                //        }
                //        return 0.0f;
                //    }

                default:
                    return 0.0f;
            }
        }
        #endregion
    }
}
