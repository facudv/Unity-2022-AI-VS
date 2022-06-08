using System.Collections;
using UnityEngine;

public class ActiveShield : State<FSMBase>
{
    [SerializeField]private GameObject shield;
    
    private IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(3f);
        _fsm.SetState("AttackEnemy");
    }
    
    public override void Enter()
    {
        shield.SetActive(true);
        StartCoroutine(ShieldDuration());
    }
    
    public override void Exit() => shield.SetActive(false);
    
}
