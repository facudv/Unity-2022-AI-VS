using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]private float speed;
    private Transform _ownerTrans;
    public LayerMask TargetLayerMask { get; private set;}

    
    private void Start() =>  Destroy(gameObject, 4f);
    private void Update() => transform.position += transform.forward * (speed * Time.deltaTime);

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer != TargetLayerMask.value) return;
        if (other.gameObject.layer == Layers.obstacle) Destroy(gameObject);
        
        other.gameObject.GetComponent<LifeManager>().ModifyHp(Modify.Remove);
        var minion = other.gameObject.GetComponent<IAMinion>();
        if (minion) minion.LastToDamage = _ownerTrans;
        Destroy(gameObject);
    }
    
    public void SetInitialParams(Vector3 forward, LayerMask layerTarget,Transform ownerTrans)
    {
        var meshrend = GetComponent<MeshRenderer>();
        meshrend.material.color = layerTarget.value switch
        {
            1 << 10 => Color.blue,
            1 << 11 => Color.red,
            _ => meshrend.material.color
        };

        transform.forward = forward;
        TargetLayerMask = layerTarget;
        _ownerTrans = ownerTrans;
    }
}
