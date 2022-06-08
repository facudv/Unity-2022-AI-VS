using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IALeader : FSMBase
{
    private Collider _myCollider;
    protected override void Start()
    {
        base.Start();
        dieState.SetActionDieIa(IaDie.Leader);
        _myCollider = GetComponent<Collider>();
    }

    public override void OnNotify(NotifyActionObserver action)
    {
        switch (action)
        {
            case NotifyActionObserver.StartGame : StartCoroutine(TimerToMove());          break;
            case NotifyActionObserver.EndGame   : enabled = false; _myCollider.enabled = false; break;
        }
    }

    private IEnumerator TimerToMove()
    {
        yield return new WaitForSeconds(4f);
        fsm.SetState("GetPath");
    }

    public abstract List<FSMBase> GetMinions();
    
    public override Transform SomeTeammateHasTargetToAttack()
    {
        //other minions has target ?
        List<FSMBase> minions = GetMinions();
        foreach (var minion in minions)
        {
            if (minion == null) continue;
            var targetAttackMinion = minion.lineOfSight.Target;
            if (targetAttackMinion) return targetAttackMinion;
        }
        //if anyone has target return null
        return null;
    }
}
