using System;
public enum IaDie{Leader,Minion}

public class Die : State<FSMBase>
{
    private Action _onIaDie;
    
    private void LeaderDie() => _owner.GameManager.EndGame(gameObject.layer == 11 ? "RED" : "BLUE", _owner);
    private void MinionDie() => _owner.GameManager.RemoveMinion(_owner);
    public override void Enter() => _onIaDie();
    public void SetActionDieIa(IaDie iaDieType)
    {
        switch (iaDieType)
        {
            case IaDie.Leader : _onIaDie += LeaderDie; break;
            case IaDie.Minion : _onIaDie += MinionDie; break;
        }
    }
}
