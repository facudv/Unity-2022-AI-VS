using System.Collections.Generic;

public class IARedLeader : IALeader
{
    private ActiveShield _activeShieldState;

    protected override void Start()
    {
        base.Start();

        _activeShieldState = GetComponent<ActiveShield>();
        fsm.AddState("ActiveShield", _activeShieldState);
        _activeShieldState.MyState(this,fsm);

        lifeManager.SetActionModifyHp(RedLeaderProtectCondition,Modify.Add);
    }

    public override List<FSMBase> GetMinions() => GameManager.RedMinions;

    private void RedLeaderProtectCondition(float hp)
    {
        if (!(hp < 35)) return;
        lifeManager.SetActionModifyHp(RedLeaderProtectCondition,Modify.Remove);
        fsm.SetState("ActiveShield");
    }
    
}
