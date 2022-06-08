using UnityEngine;

public class AreaStun : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10) return; //if not red IA
        
        var iaRedFsm = other.gameObject.GetComponent<FSMBase>();
        var iaRedMinion = other.gameObject.GetComponent<IAMinion>();
        
        if(iaRedFsm != null && iaRedMinion != null) //if minion and fsm
        {
            iaRedFsm.SetState("Stunned");
        }
    }
}
