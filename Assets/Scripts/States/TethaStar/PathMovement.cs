using System.Collections.Generic;
using UnityEngine;

public class PathMovement : State<FSMBase>
{
    [SerializeField]private Node initialNode;
    [SerializeField]private bool isPingPong;
    
    private bool _isReverse;
    private Node _targetNode;

    private List<Node> _allNodes;

    private int _currentIndex = 0;
    private int _dir = 1;

    private bool _notInDestiny;

    public void Path(List<Node> _tethaStarPath)
    {
        _allNodes = _tethaStarPath;
        _targetNode = _allNodes[0];
    }
    
    public override void Enter() => _notInDestiny = false;
    
    public override void Execute()
    {
        _owner.lineOfSight.FLineOfSight();

        if (_notInDestiny) return;
        
        Vector3 forward = _targetNode.transform.position - transform.position;
        transform.forward += Vector3.Lerp(transform.forward, forward, Time.deltaTime / 5f);
        
        var distBeforeMove = Vector3.Distance(_targetNode.transform.position, transform.position);
        transform.position += transform.forward * (2 * Time.deltaTime);

        if (!(Vector3.Distance(_targetNode.transform.position, transform.position) > distBeforeMove)) return;
        
        _currentIndex += _dir;
        if (_currentIndex < 0 || _currentIndex >= _allNodes.Count)
        {
            if (isPingPong)
            {
                _dir *= -1;
                _currentIndex += _dir;
            }
            else
            {
                _notInDestiny = true;
            }
        }
        if(!_notInDestiny)_targetNode = _allNodes[_currentIndex];
    }
}


