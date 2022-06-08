public class IABlueMinion : IAMinion
{
    protected override IALeader GetLeader() => GameManager.BlueLeader;
}

