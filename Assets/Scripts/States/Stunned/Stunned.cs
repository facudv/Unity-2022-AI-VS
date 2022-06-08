using System.Collections;
using UnityEngine;

public class Stunned : State<FSMBase>
{
    private IEnumerator TimerOutStun()
    {
        yield return new WaitForSeconds(4f);
        _fsm.SetState("AttackEnemy");
    }
    public override void Enter() => StartCoroutine(TimerOutStun());
}
