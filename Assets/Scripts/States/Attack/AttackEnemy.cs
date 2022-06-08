using System.Collections;
using UnityEngine;

public class AttackEnemy : State<FSMBase>
{
    private Transform _target;
    [SerializeField] private float timerToAttack = 0.5f;
    [SerializeField] private Bullet bulletPrefab;

    [SerializeField] private float velocityRotationToTarget = 6f;
    [SerializeField] private float angleToAttack = 2f;
    [SerializeField] private float radiusToDetectAllies = 10f;
    private bool _cooldownAttack ;
    
    //When enter in attack 
    public override void Enter()
    {
        // //Advice my team if not in attack state to attack.
        // var team = Physics.OverlapSphere(transform.position, radiusToDetectAllies, 1 << gameObject.layer);
        //
        // foreach (var item in team)
        // {
        //     var iaTeam = item.gameObject.GetComponent<FSMBase>();
        //     if (!iaTeam || iaTeam == _owner) continue;
        //     
        //     if (iaTeam.GetComponent<IALeader>() != null)
        //     {
        //         if (iaTeam.isInState("PathMovement")) iaTeam.SetAttackState();
        //     }
        //     else if (iaTeam.GetComponent<IAMinion>() != null)
        //     {
        //         if (iaTeam.isInState("Flocking")) iaTeam.SetAttackState();
        //     }
        // }
    }

    public override void Execute() => FAttackEnemy();

    private void FAttackEnemy()
    {
        _target = _owner.lineOfSight.Target;
        _owner.lineOfSight.FLineOfSight(); //Line of sight is responsible to get new target

        if (_target)
        {
            var forwardToTarget = (_target.transform.position - transform.position);
            transform.forward = Vector3.Lerp(transform.forward, forwardToTarget, Time.deltaTime * velocityRotationToTarget);
            
            if (!(Vector3.Angle(transform.forward.normalized, forwardToTarget) < angleToAttack)) return;
            
            if (_cooldownAttack) return;
            _cooldownAttack = true;
            StartCoroutine(InstanceBullet());
        }
        else
        {
            Debug.Log("No target in : " + gameObject.name);
        }
    }
    
    private IEnumerator InstanceBullet()
    {
        var trans = transform;
        var bullet = Instantiate(bulletPrefab, trans.position, Quaternion.identity);
        bullet.SetInitialParams(trans.forward, _owner.LayerEnemy, transform);
        yield return new WaitForSeconds(timerToAttack);
        _cooldownAttack = false;
    }
}
