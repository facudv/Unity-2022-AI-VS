using System.Collections.Generic;
using UnityEngine;


public class FSM<T>
{
    private readonly T _owner;
    private readonly Dictionary<string, State<T>> _states;
    public  State<T> currentState { get; private set; }

    public FSM(T owner)
    {
        _owner = owner;
        _states = new Dictionary<string, State<T>>();
    }

    public bool ActualState(string stateName)
    {
        var stateToCheck = _states[stateName];
        return stateToCheck == currentState;
    }

    public void AddState(string stateName, State<T> state) => _states.Add(stateName, state);

    public void SetState(string stateName)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = _states[stateName];
        currentState.Enter();
    }
    

    public void Update()
    {
        if (currentState)
        {
            currentState.Execute();
            //Debug.Log(currentState);
        }
        else Debug.LogWarning("NO HAY ESTADO EN ESTA FSM : " + _owner);
    }

    public void ExitState()
    {
        if (currentState != null)
            currentState.Exit();
    }
}
