using NavMeshPlus.Extensions;
using UnityEngine;

namespace FarmSystem
{
    public class Storage : MonoBehaviour
    {
        public void StorageResource(Character storager)
        {
            foreach (Transform resourceChild in storager.gameObject.transform)
            {
                 Tree treeComponent = resourceChild.gameObject.GetComponent<Tree>();
                    if (treeComponent != null)
                    {
                        Destroy(resourceChild.gameObject);
                        Debug.Log($"Tree '{resourceChild.gameObject.name}' has been removed.");
                        continue;
                    }
            }
            storager.isCarrying = false;
            storager.animator.SetBool(AnimationParams.IsCarrying_Param, storager.isCarrying);

            //AI Stuffs
            //Change Target
            if (storager as CharacterAI)
            {
                storager.targetingComponent.isUpdateTarget = true;
            }
        }
    }
}
