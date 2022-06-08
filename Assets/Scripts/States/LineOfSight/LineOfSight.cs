using UnityEngine;
using System.Collections;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float viewDistance = 10f;
    
    private Vector3 _dirToTarget;
    private float _angleToTarget;
    private float _distanceToTarget;
    private bool _targetInSight;
    private bool _settingTarget;

    private FSMBase _myOwner;
    private AreaDetectEnemy _areaDetectEnemy;
    
    public Transform Target { get; private set; }

    private void Awake() => _areaDetectEnemy = GetComponentInChildren<AreaDetectEnemy>();
    private void Start() => _areaDetectEnemy.SetSphereRadiusToDetectEnemy(viewDistance);
    
    private IEnumerator SetTargetTimer()
    {
        _settingTarget = true;
        yield return new WaitForSeconds(0.5f);
        SetTarget();
        _settingTarget = false;
    }

    private void SetTarget() => Target = _areaDetectEnemy.GetNearestEnemyTarget() != null ? _areaDetectEnemy.GetNearestEnemyTarget() : _myOwner.SomeTeammateHasTargetToAttack();
    public void SetLayerEnemy(LayerMask layerEnemy) => _areaDetectEnemy.SetLayerToDetectEnemy(layerEnemy);
    public void SetOwner(FSMBase owner) => _myOwner = owner;
    public void FLineOfSight()
    {
        if (Target)
        {
            var trans  =   transform;
            var posTarget= Target.transform.position;
            
            _dirToTarget      = posTarget - trans.position; //direction : final pos  - initial pos;
            _angleToTarget    = Vector3.Angle(trans.forward, _dirToTarget); //Vector3.Angle : angle between two directions.
            _distanceToTarget = Vector3.Distance(transform.position,posTarget);  

            if (_angleToTarget <= viewAngle && _distanceToTarget <= viewDistance) // meet the requirements /Is inside of field of view ?
            {
                //raycast to detect an obstacle between the target and this
                RaycastHit rch;
                bool obstaclesBetween = false;
                if (Physics.Raycast(transform.position, _dirToTarget, out rch, _distanceToTarget, 1 << Layers.obstacle))
                    if (rch.collider != null)
                        obstaclesBetween = true; //if collides with obstacle layer

                /*
                If any obstacles obstructs can attack target
                */
                if (!obstaclesBetween)
                {
                    _targetInSight = true;
                    if (!_myOwner.isInState("AttackEnemy"))
                    {
                        _myOwner.SetAttackState();
                    }
                }
                else
                    _targetInSight = false;
            }
            else //If scape the target
            {
                _targetInSight = false;
                _myOwner.SetSeekState(Target);
            }
        }
        else
        {
            if (!_settingTarget) StartCoroutine(SetTargetTimer());
        }
    }
    private void OnDrawGizmos()
    {
        /*
        Draw line to Target. If can see show in green else in red.
        */
        if (Target)
        {
            Gizmos.color = _targetInSight ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, Target.transform.position);
        }

        /*
        Draw the limits of field of view.
        */
        var trans = transform;
        var position = trans.position;
        var forward = trans.forward;
        var up = trans.up;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(position, viewDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(position, position + (forward * viewDistance));

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngle, up) * forward;
        Gizmos.DrawLine(position, position + (rightLimit * viewDistance));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngle, up) * forward;
        Gizmos.DrawLine(position, position + (leftLimit * viewDistance));
    }
}