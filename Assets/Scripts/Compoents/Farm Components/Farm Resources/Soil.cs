using FarmSystem;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

namespace FarmSystem
{
    [Flags]
    public enum ESoilState
    {
        None = 0,
        HasPlant = 1 << 0,
        IsDry = 1 << 1,
        CanHarvest = 1 << 2,
        InProcess = 1 << 3
    }
    public class Soil : MonoBehaviour
    {
        [Header("Components")]
        public SpriteRenderer spriteRenderer;

        [Header("Land Data")]
        public SO_SoilData soilDataOrigin;
        public SO_SoilData soilDataTemporary;

        [Header("Current Soil Stage")]
        public ESoilState soilState;
        public SoilStageData currentLandStageData;

        [Header("Current Tree")]
        public Tree currentTree;

        [Header("Loaded Trees")]
        public List<SO_TreeData> loadedTreeDatas;

        //Test
        public float timeUpdated = 0.0f;
        public float timeDelay = 2.0f;
        //int count = 0;
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            soilDataTemporary = Instantiate(soilDataOrigin);
        }
        private void Start()
        {
        }
        private void Update()
        {

        }
        #region SoilStageManager
        public void SetState(ESoilState state)
        {
            soilState |= state;
        }
        public void RemoveState(ESoilState state)
        {
            soilState &= ~state;
        }
        public bool HasState(ESoilState state)
        {
            return (soilState & state) != 0;
        }
        #endregion
        #region SoilStuffs
        public void PlantTree(Character planter, string treeName)
        {
            if ((soilState & ESoilState.HasPlant) == 0)
            {
                //Instantiate
                SO_TreeData treeDataLoad = Resources.Load<SO_TreeData>($"Tree Data/{treeName}");
                GameObject treeGO = Instantiate(treeDataLoad.data.objectReference);

                //Assign Transform
                treeGO.transform.SetParent(transform);
                treeGO.transform.localPosition = treeDataLoad.data.offsets.plantingPositionOffset;
                treeGO.transform.transform.localScale = treeDataLoad.data.offsets.plantingScaleOffset;

                currentTree = treeGO.GetComponent<Tree>();
                SetState(ESoilState.HasPlant);

                //Change Soil's Sprite
                foreach(var stageData in soilDataTemporary.data.stageDatas)
                {
                    if (stageData.name == "HasTree") {
                        spriteRenderer.sprite = stageData.stageImage;
                    }
                }

                //AI Stuffs
                //Change Target
                if (planter as CharacterAI)
                {
                    planter.targetingComponent.isUpdateTarget = true;
                    //bool hasTarget = planter.targetingComponent.ChangeTargetAI(ETargetObjectType.Soil, ESoilState.HasPlant, false);
                    //if (hasTarget)
                    //{
                    //    return;
                    //}
                    //hasTarget = planter.targetingComponent.ChangeTargetAI(ETargetObjectType.Soil, ESoilState.CanHarvest, true);
                    //if (hasTarget)
                    //{
                    //    return;
                    //}
                }
            }
        }
        public void Watering()
        {
            if ((soilState & ESoilState.HasPlant) == 0)
            {
                //Change Soil's Sprite
                foreach (var stageData in soilDataTemporary.data.stageDatas)
                {
                    if (stageData.name == "Wet")
                    {
                        spriteRenderer.sprite = stageData.stageImage;
                    }
                }
            }
        }
        public void Harvest(Character harvester)
        {
            if (((soilState & ESoilState.HasPlant) != 0)
                && currentTree.treeCurrentStage.isFinalStage)
            {
                //Switch Anim
                harvester.isCarrying = true;
                harvester.animator.SetBool(AnimationParams.IsCarrying_Param, harvester.isCarrying);

                //Spawn Tree is Chile of Harvester
                GameObject treeGO = currentTree.gameObject;
                treeGO.transform.SetParent(harvester.gameObject.transform);

                //Assign Tree Transform
                treeGO.transform.localPosition = currentTree.treeDataTemporary.data.offsets.havestingPositionOffset;
                treeGO.transform.localScale = currentTree.treeDataTemporary.data.offsets.havestingScaleOffset;

                //Remove State
                RemoveState(ESoilState.HasPlant);
                RemoveState(ESoilState.CanHarvest);

                //AI Stuffs
                //Change Target
                if (harvester as CharacterAI)
                {
                    bool hasTarget = harvester.targetingComponent.ChangeTargetAI(ETargetObjectType.Storage);
                    if (hasTarget)
                    {
                        return;
                    }
                }
            }
        }
        public void DestroyCurrentTree()
        {
            if ((soilState & ESoilState.HasPlant) != 0)
            {
                GameObject treeGO = currentTree.gameObject;
                Destroy(treeGO);
                currentTree = null;
                RemoveState(ESoilState.HasPlant);
            }
        }
        #endregion


    }
}
