using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaDetectEnemy : MonoBehaviour
{
    private LayerMask _layerEnemy;
    private SphereCollider _mySphereCollider;
    [SerializeField]private List<Transform> _enemysGo = new();
    
    private void Awake() => _mySphereCollider = gameObject.GetComponent<SphereCollider>();
    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer != _layerEnemy.value) return;
        if(!_enemysGo.Contains(other.transform)) _enemysGo.Add(other.transform);
    }

    public void SetLayerToDetectEnemy(LayerMask layerMask) => _layerEnemy = layerMask;
    public void SetSphereRadiusToDetectEnemy(float radius) => _mySphereCollider.radius = radius * 2f;
    public Transform GetNearestEnemyTarget() => _enemysGo.Count > 0 ? _enemysGo.Where(x => x != null).OrderBy(x => (x.transform.position - transform.position).magnitude).FirstOrDefault() : null;
    
}
