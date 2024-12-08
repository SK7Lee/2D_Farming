using System.Collections;
using UnityEngine;

namespace FarmSystem
{
    
    public class Tree : MonoBehaviour
    {
        [Header("Components")]
        public SpriteRenderer spriteRenderer;

        [Header("Tree Datas")]
        public SO_TreeData treeDataOrigin;
        public SO_TreeData treeDataTemporary;

        [Header("Stage Manager")]
        [Header("Tree Current Stage")]
        public int currentStageIndex = 0;
        public StageData treeCurrentStage;

        Coroutine C_Growing;
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        private void Start()
        {
            treeDataTemporary = Instantiate(treeDataOrigin);
            treeCurrentStage = treeDataTemporary.data.stageDatas[currentStageIndex];
            StartGrowing();
        }
        public void TransitionToNextStage()
        {
            if (!treeCurrentStage.isFinalStage)
            {
                currentStageIndex++;
                treeCurrentStage = treeDataTemporary.data.stageDatas[currentStageIndex];
                spriteRenderer.sprite = treeCurrentStage.stageImage;
            }
        }

        public void StartGrowing()
        {
            if(C_Growing != null)
            {
                StopCoroutine(C_Growing);
            }
            StartCoroutine(Growing());
        }

        IEnumerator Growing()
        {
            while(currentStageIndex < treeDataTemporary.data.stageDatas.Count)
            {
                yield return new WaitForSeconds(treeCurrentStage.transitionTime);
                TransitionToNextStage();
            }
        }

    }
}