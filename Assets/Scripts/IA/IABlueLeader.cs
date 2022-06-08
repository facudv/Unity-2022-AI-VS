using System.Collections.Generic;

public class IABlueLeader : IALeader
{
    private StunEnemies _stunEnemysState;

    private void BlueLeaderStunCondition(float hp)
    {
        if (!(hp < 35)) return;
        lifeManager.SetActionModifyHp(BlueLeaderStunCondition,Modify.Remove);
        fsm.SetState("StunEnemies");
    }
    
    protected override void Start()
    {
        base.Start();

        _stunEnemysState = GetComponentInChildren<StunEnemies>();
        fsm.AddState("StunEnemies", _stunEnemysState);
        _stunEnemysState.MyState(this, fsm);

        lifeManager.SetActionModifyHp(BlueLeaderStunCondition,Modify.Add);
    }

    public override List<FSMBase> GetMinions() => GameManager.BlueMinions;
    
}
