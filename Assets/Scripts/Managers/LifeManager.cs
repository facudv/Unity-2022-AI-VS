using System;
using UnityEngine;

public enum Modify{Add,Remove}

public class LifeManager : MonoBehaviour
{
    private float _actualHp;
    private Action<float> OnModifyHp = delegate {};
    
    [SerializeField] private float maxHp;
    private FSMBase _owner;

    private void Start() => _actualHp = maxHp;
    public void SetOwner(FSMBase owner) => _owner = owner;
    public void SetActionModifyHp(Action<float> modifyHp,Modify actionType) => OnModifyHp = actionType == Modify.Add ? OnModifyHp += modifyHp : OnModifyHp -= modifyHp;

    /// <summary>
    /// "add" : to add life.
    /// "rest" : to rest life.
    /// </summary>
    /// <param name="action"></param>
    public void ModifyHp(Modify action)
    {
        switch (action)
        {
            case Modify.Add                       : if (_actualHp < maxHp) _actualHp++; break;
            case Modify.Remove when _actualHp > 0 : _actualHp--;                        break;
            case Modify.Remove                    : _owner.SetState("Die");             break;
        }
        OnModifyHp(_actualHp);
    }


}
