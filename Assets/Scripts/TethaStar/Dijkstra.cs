using UnityEngine;
using System.Collections.Generic;

public class Dijkstra : MonoBehaviour
{
    private readonly List<Node> _openNodes = new();
    private readonly List<Node> _closedNodes = new();
    [SerializeField] private List<Node> pathNodes = new();

    [SerializeField] private Node initialNode;
    [SerializeField] private Node finalNode;

    //who call this class must add to list and wait his turn to get the path nodes
    [SerializeField] private List<GetPath> iaRequests;

    //ThetaStar
    private List<int> _indexsToRemovePath = new();


    public void RegistrerIARequestOfPath(GetPath iaRequest)
    {
        iaRequests.Add(iaRequest);
        CheckTheFirstIaRequesterToGetPath(iaRequest);
    }

    private void CheckTheFirstIaRequesterToGetPath(GetPath iaRequest)
    {
        if (iaRequest != iaRequests[0]) return;
        
        initialNode = iaRequest.nearNode;
        finalNode = iaRequest.destinyNode;
        
        ExecuteDijkstra(0);
        
        iaRequests.Remove(iaRequest);
        if (iaRequests.Count >= 1) CheckTheFirstIaRequesterToGetPath(iaRequests[0]);
    }

    private void ExecuteDijkstra(int index)
    {
        foreach (var item in _openNodes)
        {
            item.currentWeight = Mathf.Infinity;
            item.father = null;
        }

        foreach (var item in _closedNodes)
        {
            item.currentWeight = Mathf.Infinity;
            item.father = null;
        }

        _openNodes.Clear();
        _closedNodes.Clear();
        pathNodes.Clear();

        _openNodes.Add(initialNode);
        initialNode.currentWeight = 0;
        
        Explore(initialNode, index);
    }


    private void Explore(Node n, int index)
    {
        n.FindNearNodes();

        for (int i = n.nearNodes.Count - 1; i >= 0; i--)
        {
            if (_closedNodes.Contains(n.nearNodes[i])) continue;
            
            if (!_openNodes.Contains(n.nearNodes[i]))
            {
                _openNodes.Add(n.nearNodes[i]);
            }
            var dist = n.currentWeight + n.distanceToNode[i];
            
            if (!(dist < n.nearNodes[i].currentWeight)) continue;
            
            n.nearNodes[i].currentWeight = dist;
            n.nearNodes[i].father = n;
        }

        _openNodes.Remove(n);
        _closedNodes.Add(n);

        if (n == finalNode)
        {
            pathNodes.Add(n);
            var aux = n.father;
            while (aux != null)
            {
                pathNodes.Add(aux);
                aux = aux.father;
            }
            pathNodes.Reverse();
            var indexNextNode = 1;
            _indexsToRemovePath.Clear();
            List<Node> thetaStarPath = new List<Node>();
            for (int i = 0; i < pathNodes.Count; i++)
            {
                while (indexNextNode < pathNodes.Count && pathNodes[i].NodeInSight(pathNodes[indexNextNode], pathNodes[i].transform.position))
                {
                    indexNextNode++;
                }

                if (indexNextNode >= pathNodes.Count) continue;
                i = indexNextNode - 2;
                _indexsToRemovePath.Add(indexNextNode - 1);
            }

            thetaStarPath.Add(initialNode);
            foreach (var item in _indexsToRemovePath)
            {
                thetaStarPath.Add(pathNodes[item]);
            }
            thetaStarPath.Add(finalNode);
            
            iaRequests[index].SendPathToRequestIa(thetaStarPath);
        }
        else
        {
            if (_openNodes.Count > 0)
            {
                var actualNode = _openNodes[0];
                var currentFitness = Mathf.Infinity;
                foreach (var item in _openNodes)
                {
                    var fitnessOfItem = Vector3.Distance(item.transform.position, finalNode.transform.position) //Euclidiana
                        + Mathf.Abs(item.transform.position.x - finalNode.transform.position.x) //Manhattan
                        + Mathf.Abs(item.transform.position.y - finalNode.transform.position.y)
                        + Mathf.Abs(item.transform.position.z - finalNode.transform.position.z);

                    if (currentFitness > fitnessOfItem) //Found better node to explore
                    {
                        currentFitness = fitnessOfItem;
                        actualNode = item;
                    }
                }

                Explore(actualNode, index);
            }
            else
            {
                Debug.Log("No se encontro camino");
            }
            
        }
    }
}
