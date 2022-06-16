using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Node : MonoBehaviour
{
    public List<Node> nearNodes;
    public List<float> distanceToNode;
    public float currentWeight = Mathf.Infinity;
    public Node father;
    public float range;
    public LayerMask layerNodeAndObstacles;
   
    private RaycastHit[] _raycastHits;

    public void FindNearNodes()
    {
        nearNodes = new List<Node>();
        distanceToNode = new List<float>();
        Node[] ns = FindObjectsOfType<Node>();
        for (int i = ns.Length - 1; i >= 0; i--)
        {
            float dist = Vector3.Distance(transform.position, ns[i].transform.position);
            if (!(dist < range) || ns[i] == this) continue;
            
            nearNodes.Add(ns[i]);
            distanceToNode.Add(dist);
        }
    }

    public bool NodeInSight(Node n, Vector3 positionAcutalNode)
    {
        Vector3 _directionToNode = n.transform.position - positionAcutalNode;
        float _distanceRaycast = Vector3.Distance(positionAcutalNode, n.transform.position);
        _raycastHits = Physics.RaycastAll(positionAcutalNode, _directionToNode, _distanceRaycast, layerNodeAndObstacles);
        RaycastHit[] myArrayTemp = new RaycastHit[_raycastHits.Count()];
        IEnumerable<RaycastHit> raycastHitsOrderer= _raycastHits.OrderBy(rayHit => Vector3.Distance(rayHit.transform.position, positionAcutalNode));
        foreach (var item in raycastHitsOrderer)
        {
            Node _nodeToCheck = item.transform.gameObject.GetComponent<Node>();
            if (!_nodeToCheck)
            {
                return false; //if not a node it´s an obstacle or wall.
            }
            if (_nodeToCheck == n) return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (father != null)
            Gizmos.DrawLine(father.transform.position, transform.position);
    }
}
