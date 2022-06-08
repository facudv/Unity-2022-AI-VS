using UnityEngine;

public class Seek : State<FSMBase>
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    
    public override void Execute()
    {
        _owner.lineOfSight.FLineOfSight();
        if (!target) return;

        var trans = transform;
        var forward = trans.forward;
        var position = trans.position;
        position += forward * (Time.deltaTime * speed);
        trans.position = position;
        forward = Vector3.Lerp(forward, (target.position - position).normalized,rotationSpeed * Time.deltaTime);
        transform.forward = forward;
    }
    
    public void SetTarget(Transform targetTrans) => target = targetTrans;

    private void OnDrawGizmos()
    {
        if (!target) return;
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawLine(position, (target.position - position).normalized * speed * 2 + position);
    }
}