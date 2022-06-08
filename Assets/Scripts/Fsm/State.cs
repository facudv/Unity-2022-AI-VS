using UnityEngine;

public abstract class State<T> : MonoBehaviour
{
    public virtual void Enter(){}
    public virtual void Execute(){}
    public virtual void Exit(){}

    protected T _owner;
    protected FSM<T> _fsm;
    
    public void MyState(T owner, FSM<T> fsm)
    {
        _owner = owner;
        _fsm = fsm;
    }
}
