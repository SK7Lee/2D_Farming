using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    public Character owner;
    public Transform target;

    public void Awake()
    {
        owner = GetComponent<Character>();
    }

}
