using UnityEditor;
using UnityEngine;

public abstract class FSMBase : MonoBehaviour , IObserver
{
    protected FSM<FSMBase> fsm;
    private IObservable _observableGm;
    [SerializeField] private LayerMask layerEnemy;
    public LayerMask LayerEnemy
    {
        get => layerEnemy;
        private set => layerEnemy = value;
    }
    
    //Managers
    public GameManager GameManager { get; private set; }
    protected LifeManager lifeManager;
    
    public LineOfSight lineOfSight { get; private set; }

    //States
    protected Die dieState;
    private Seek _seekState;
    
    protected virtual void Start()
    {
        fsm = new FSM<FSMBase>(this);
        
        fsm.AddState("PathMovement", GetComponent<PathMovement>());
        GetComponent<PathMovement>().MyState(this, fsm);
        
        fsm.AddState("GetPath", GetComponent<GetPath>());
        GetComponent<GetPath>().MyState(this, fsm);

        var attackEnemyState = GetComponent<AttackEnemy>();
        fsm.AddState("AttackEnemy", attackEnemyState);
        attackEnemyState.MyState(this, fsm);

        dieState = GetComponent<Die>();
        fsm.AddState("Die", dieState);
        dieState.MyState(this, fsm);

        _seekState = GetComponent<Seek>();
        fsm.AddState("Seek", _seekState);
        _seekState.MyState(this, fsm);

        var idleState = GetComponent<Idle>();
        fsm.AddState("Idle", idleState);
        idleState.MyState(this, fsm);

        lineOfSight = GetComponent<LineOfSight>();
        lineOfSight.SetOwner(this);
        lineOfSight.SetLayerEnemy(layerEnemy);
        
        lifeManager = GetComponent<LifeManager>();
        lifeManager.SetOwner(this);

        GameManager = FindObjectOfType<GameManager>();
        
        _observableGm = GameManager;
        _observableGm.Subscribe(this);
    }
    
    public virtual void Update() => fsm.Update();
    public bool isInState(string actualState) => fsm.ActualState(actualState);
    public abstract void OnNotify(NotifyActionObserver action);
    public abstract Transform SomeTeammateHasTargetToAttack();
    public void SetState(string stateName)  => fsm.SetState(stateName);

    public void SetAttackState() => SetState("AttackEnemy");

    public void SetSeekState(Transform target)
    {
        _seekState.SetTarget(target.transform);
        SetState("Seek");
    }

    private void OnDrawGizmos()
    {
        if (fsm == null) return;
        if(fsm.currentState != null)
            Handles.Label(transform.position + Vector3.up * 1.5f, "s = "+ fsm.currentState);
    }
}
