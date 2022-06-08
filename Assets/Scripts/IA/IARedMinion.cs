public class IARedMinion : IAMinion

{
    protected override void Start()
    {
        base.Start();
 
        fsm.AddState("Stunned", GetComponent<Stunned>());
        GetComponent<Stunned>().MyState(this, fsm);
    }
    
    protected override IALeader GetLeader() => GameManager.RedLeader;
}
