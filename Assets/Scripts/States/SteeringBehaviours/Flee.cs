using UnityEngine;

public class Flee : State<FSMBase>
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.obstacle && _owner.isInState("Flee")) _fsm.SetState("Idle");
    }

    public override void Enter()
    {
        var mesh = GetComponent<MeshRenderer>();
        mesh.material.color = Color.white;
    }

    public override void Execute()
    {
        if (target)
        {
            transform.position += transform.forward * (speed/2 * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, (transform.position - target.position).normalized, Time.deltaTime * rotationSpeed * 3);
        }
        else _fsm.SetState("Idle");
    }
    
    public void SetTarget(Transform transTarget) => target = transTarget;
    
    private void OnDrawGizmos()
    {
        if (!target) return;
        var position = transform.position;
            
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, (position - target.position).normalized * speed + position);
    }
}
