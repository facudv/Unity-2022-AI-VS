using System.Collections.Generic;
using UnityEngine;

public abstract class IAMinion : FSMBase
{
    private Flee _fleeState;
    public Transform LastToDamage{get; set;}//ref to the last target who damaged this
    
    private void FleeStateCondition(float hp)
    {
        if (!(hp < 10)) return;

        _fleeState.SetTarget(LastToDamage);
        fsm.SetState("Flee");
    }
    
    protected override void Start()
    {
        base.Start();

        fsm.AddState("Flocking", GetComponent<Boid>());
        GetComponent<Boid>().MyState(this, fsm);
        
        dieState.SetActionDieIa(IaDie.Minion);

        _fleeState = GetComponent<Flee>();
        fsm.AddState("Flee", _fleeState);
        _fleeState.MyState(this, fsm);

        lifeManager.SetActionModifyHp(FleeStateCondition,Modify.Add);
    }

    protected abstract IALeader GetLeader();

    public override void OnNotify(NotifyActionObserver action)
    {
        switch (action)
        {
            case NotifyActionObserver.StartGame: fsm.SetState("Flocking"); break;
            case NotifyActionObserver.EndGame  : enabled = false;          break;
        }
    }
    
    public override Transform SomeTeammateHasTargetToAttack()
    {
        //leader has target ?
        IALeader leader = GetLeader();
        Transform targetAttackLeader = leader.GetComponent<FSMBase>().lineOfSight.Target;
        if (targetAttackLeader) return targetAttackLeader;
        //other minions has target ?
        List<FSMBase> minions = leader.GetMinions();
        var index = 0;
        for (; index < minions.Count; index++)
        {
            var minion = minions[index];
            if (minion == null) continue;
            var targetAttackMinion = minion.lineOfSight.Target;
            if (targetAttackMinion != null) return targetAttackMinion;
        }
        //if anyone has target return null
        return null;
    }
}
