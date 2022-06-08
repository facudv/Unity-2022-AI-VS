using System.Collections;
using UnityEngine;

public class StunEnemies : State<FSMBase>
{
    [SerializeField]private GameObject areaStunEnemys;

    private IEnumerator ReturnAttack()
    {
        yield return new WaitForSeconds(4f);
        _fsm.SetState("AttackEnemy");
    }
    
    public override void Enter()
    {
        areaStunEnemys.SetActive(true);
        StartCoroutine(ReturnAttack());
    }

    public override void Exit() => areaStunEnemys.SetActive(false);

}
