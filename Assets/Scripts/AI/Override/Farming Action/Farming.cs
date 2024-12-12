using FarmSystem;
using NavMeshPlus.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = "Behavior Data", menuName = "Farm System/Behavior/Farming Action")]
public class Farming : FarmingAction
{
    public SO_ActionData ActionData;
    public override void Execute(CharacterAI agent)
    {
        //Benefit(agent);
        agent.actionComponent.PlayActionAI(ActionData); 

    }
    public override bool IsFinish(CharacterAI agent)
    {
        return base.IsFinish(agent);
    }
    public void Benefit(CharacterAI agent)
    {
        switch(ActionData.data.name) {
            case "Break Stone":

                break;
            case "Dig":

                break;
            case "Plant":
                agent.targetingComponent.target.gameObject.GetComponent<Soil>().PlantTree(agent, agent.treeTargetName);
                //agent.targetingComponent.target.gameObject.GetComponent<Soil>().RemoveState(ESoilState.InProcess);
                break;
            case "Watering":

                break;
            case "Fishing":

                break;
            case "Harvesting":
                agent.targetingComponent.target.gameObject.GetComponent<Soil>().Harvest(agent);
                //agent.targetingComponent.target.gameObject.GetComponent<Soil>().RemoveState(ESoilState.InProcess);
                break;
            case "Storage Resources":
                agent.targetingComponent.target.gameObject.GetComponent<Storage>().StorageResource(agent);
                break;

        }
    }
}