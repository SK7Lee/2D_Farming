using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FarmSystem
{
    public enum EActionState
    {
        None = 0,
        Executing,
        Finish
    }
    public class ActionComponent : MonoBehaviour
    {
        [Header("Character Onwer")]
        public Character owner;

        [Header("Loaded Actions")]
        public List<SO_ActionData> actions;

        [Header("Current Action")]
        public SO_ActionData currentAction;
        public EActionState currentActionState = EActionState.None;

        public int actionExecuteTimes = 0;
        //Coroutine
        Coroutine C_ExecuteAction;
        private void Awake()
        {
            owner = GetComponent<Character>();
            LoadAction();
        }
        public void LoadAction()
        {
            var actionDatas = Resources.LoadAll<SO_ActionData>($"Action Data");
            foreach (var actionData in actionDatas)
            {
                actions.Add(actionData);
            }
        }
        public SO_ActionData FilterAction()
        {
            foreach(var action in actions)
            {
                if(action.data.toolRequired == owner.currentToolType 
                    && action.data.targetObjectTypeRequired == owner.targetingComponent.currentTargetObjectType)
                {
                    return action;
                }
            }
            return null;
        }
        public void StartExecuteAction()
        {
            if (C_ExecuteAction != null)
            {
                StopCoroutine(C_ExecuteAction);
            }
            C_ExecuteAction = StartCoroutine(ExecuteAction());
        }
        public void PlayActionAI(SO_ActionData action)
        {
            currentAction = action;
            owner.currentToolType = action.data.toolRequired;
            StartExecuteAction();
        }
        public void ExecuteAnimationByTool(EToolType toolType)
        {
            if(owner as CharacterAI)
            {
                (owner as CharacterAI).Flip(EFlipType.ActionExecute);
            }
            switch (toolType)
            {
                case EToolType.Pickaxe:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Mining_State, .1f);
                    break;
                case EToolType.Axe:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Axe_State, .1f);
                    break;
                case EToolType.WaterCan:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Watering_State, .1f);
                    break;
                case EToolType.Shovel:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Dig_State, .1f);
                    break;
                case EToolType.ByHand:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Doing_State, .1f);
                    break;
                case EToolType.Hammer:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Hammering_State, .1f);
                    break;
                case EToolType.Rod:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Casting_State, .1f);
                    break;
                default:
                    owner.animator.CrossFadeInFixedTime(AnimationParams.Doing_State, .1f);
                    break;
            }
        }
        IEnumerator ExecuteAction()
        {
            yield return new WaitWhile(() => currentActionState == EActionState.Executing);

            if (owner as CharacterAI)
            {
                Soil soil = (owner as CharacterAI).targetingComponent.target.GetComponent<Soil>();
                if (soil != null)
                {
                    if (!soil.HasState(ESoilState.InProcess))
                    {
                        soil.SetState(ESoilState.InProcess);
                    }
                    else
                    {
                        (owner as CharacterAI).targetingComponent.isUpdateTarget = true;
                        yield break;
                    }
                }
            }
            while (actionExecuteTimes < currentAction.data.playAnimTimes)
            {
                yield return new WaitWhile(() => currentActionState == EActionState.Executing);
                ExecuteAnimationByTool(owner.currentToolType);
                actionExecuteTimes++;
                yield return new WaitWhile(() => currentActionState == EActionState.Executing);
            }
            if (owner as CharacterAI)
            {
                Soil soil = (owner as CharacterAI).targetingComponent.target.GetComponent<Soil>();
                if (soil != null)
                    (owner as CharacterAI).targetingComponent.target.GetComponent<Soil>().RemoveState(ESoilState.InProcess);
            }
            yield return new WaitWhile(() => currentActionState == EActionState.Executing);

            ResetAction();
        }
        public void ResetAction()
        {
            actionExecuteTimes = 0;
           if(owner as CharacterAI)
            {
                CharacterAI ownerAI = owner as CharacterAI;
                ownerAI.behaviorDecesion.currentBehaviorState = EBehaviorState.Finish;
                    (ownerAI.behaviorDecesion.currentFarmingActionData.action as Farming).Benefit(ownerAI);

            }
        }

    }
}