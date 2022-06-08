using UnityEngine;

public class GetNearNode : MonoBehaviour
{
    [SerializeField] private LayerMask nodeLayerMask;
    private Collider _nearestCollider;

    private void Start() => FGetNearNode();
    
    public Node FGetNearNode()
    {
        Collider[] _nearNodes = Physics.OverlapSphere(transform.position, 2.5f, nodeLayerMask);
        foreach (var item in _nearNodes)
        {
            if (!_nearestCollider) _nearestCollider = item;
            else
            {
                var distanceAcutalItem = Vector3.Distance(transform.position, item.transform.position);
                var distanceNearestNode = Vector3.Distance(transform.position, _nearestCollider.transform.position);
                if (distanceAcutalItem < distanceNearestNode) _nearestCollider = item;
            }
        }
        return _nearestCollider.gameObject.GetComponent<Node>();
    }
}
