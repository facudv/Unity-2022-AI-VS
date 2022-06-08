using System.Collections.Generic;

public class GetPath : State<FSMBase>
{
    private Dijkstra _tethaStar;
    private GetNearNode _getNearNode;
    public Node nearNode;
    public Node destinyNode;
    private PathMovement _pathMovementState;

    private void Awake()
    {
        _getNearNode = GetComponent<GetNearNode>();
        _tethaStar = FindObjectOfType<Dijkstra>();
        _pathMovementState = GetComponent<PathMovement>();
    }
    
    public override void Enter()
    {
        nearNode = _getNearNode.FGetNearNode();
        _tethaStar.RegistrerIARequestOfPath(this);
    }
    public void SendPathToRequestIa(List<Node> _tethaStarPath)
    {
        _pathMovementState.Path(_tethaStarPath);
        _fsm.SetState("PathMovement");
    }
}
