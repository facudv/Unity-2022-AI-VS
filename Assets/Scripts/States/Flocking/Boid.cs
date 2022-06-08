using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Boid : State<FSMBase>
{
	[Header("Objets In Area")]
	[SerializeField]private List<Collider> flock;
	[SerializeField]private List<Collider> obstacles;
	private Collider _nearestObstacle;
	
	[Header("Objets In Area")]
	[SerializeField]private float radiusFlock = 8.5f;
	[SerializeField]private float radiusAvoidance = 1.5f;
	[SerializeField]private float speed;
	[SerializeField]private float rotSpeed;
	[SerializeField]private Vector3 dir;

	private Vector3 _vectCohesion;
	private Vector3 _vectAlignment;
	private Vector3 _vectSeparation;
	private Vector3 _vectLeader;
	private Vector3 _vectAvoidance;

	[Header("LayersMask")]
	[SerializeField]private LayerMask layerObstacle;
	[SerializeField]private LayerMask layerMinionType;
	
	[Header("Multipliers")]
	[SerializeField]private float escCohesion;
	[SerializeField]private float escAlignment = 1f;
	[SerializeField]private float escSeparation = 1f;
	[SerializeField]private float escLeader = 1f;
	[SerializeField]private float escAvoidance;
	private float _offsetLeader;
	
	private bool _onTriggerObstacle;
	[Header("TargetTransform")]
	[SerializeField]private Transform leaderTrans;

	private bool isNearLeaderPos => (transform.position - leaderTrans.transform.position).magnitude > 0.5;

	private Vector3 CalculateCohesion()
	{
		Vector3 c = new Vector3();

		for (int i = 0; i < flock.Count; i++)
		{
			c += flock[i].transform.position - transform.position;
		}
		c /= flock.Count;
		return c;
	}

	private Vector3 CalculateAlignment()
	{
		Vector3 a = new Vector3();

		foreach (var t in flock)
		{
			a += t.transform.forward;
		}
		a /= flock.Count;

		return a;
	}

	private Vector3 CalculateSeparation()
	{
		Vector3 s = new Vector3();

		for (int i = 0; i < flock.Count; i++)
		{
			Vector3 v = transform.position - flock[i].transform.position;
			float m = radiusFlock - v.magnitude;
			v.Normalize();
			v *= m;
			s += v;
		}
		s /= flock.Count;
		return s;
	}
	private Vector3 CalculateLeader() => leaderTrans != null ? leaderTrans.position - leaderTrans.forward * _offsetLeader - transform.position : Vector3.zero;
	private Vector3 CalculateAvoidance() => !_nearestObstacle ? Vector3.zero : transform.position - _nearestObstacle.transform.position;
	private Collider GetNearestObstacle() => obstacles.Count > 0 ? obstacles.OrderBy(mCollider => (mCollider.transform.position - transform.position).magnitude).First() : null;

	private void GetObjectsInArea(List<Collider> colliders, float radius, LayerMask layer)
	{
		colliders.Clear();
		colliders.AddRange(Physics.OverlapSphere(transform.position, radius, layer));
		colliders.Remove(GetComponent<Collider>());
	}
	
	private void OnTriggerStay(Collider other)
	{
		if (!Utility.IsInLayerMask(other.gameObject,layerObstacle)) return;
		if (_owner.isInState("Idle")) return;
		_fsm.SetState("Flocking");
		_nearestObstacle = other;
		_onTriggerObstacle = true;
	}

	private void OnTriggerExit(Collider other) 
	{
		if (!Utility.IsInLayerMask(other.gameObject,layerObstacle)) return;
		_onTriggerObstacle = false;
	}
	
	public override void Execute()
	{
		var position = transform.position;
		GetObjectsInArea(obstacles, radiusAvoidance, layerObstacle);

		if (_onTriggerObstacle)
		{
			transform.forward = (transform.position - _nearestObstacle.transform.position).normalized;
			transform.position += transform.forward * (speed * Time.deltaTime);
			return;
		}
		
		_owner.lineOfSight.FLineOfSight();
		
		GetObjectsInArea(flock, radiusFlock, layerMinionType);

		_nearestObstacle = GetNearestObstacle();
		
		_vectCohesion = CalculateCohesion() * escCohesion;
		_vectAlignment = CalculateAlignment() * escAlignment;
		_vectSeparation = CalculateSeparation() * escSeparation;
		_vectLeader = CalculateLeader() * escLeader;
		_vectAvoidance = CalculateAvoidance() * escAvoidance;
		
		dir = _vectCohesion + _vectAlignment + _vectSeparation + _vectLeader + _vectAvoidance;

		
		if (!isNearLeaderPos) return;

		var trans = transform;
		var forward = trans.forward;
		
		forward = Vector3.Lerp(forward, dir, rotSpeed * Time.deltaTime);
		trans.forward = forward;
		trans.position += forward * (Time.deltaTime * speed);
	}
	
	public void SetLeaderRef(Transform trans) => leaderTrans = trans;
	
	private void OnDrawGizmos()
	{
		var position = transform.position;
		
		Gizmos.color = Color.red;
		Gizmos.DrawRay(position, _vectCohesion);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(position, _vectAlignment);

		Gizmos.color = Color.green;
		Gizmos.DrawRay(position, _vectSeparation);

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(position, _vectLeader);
	}
}
