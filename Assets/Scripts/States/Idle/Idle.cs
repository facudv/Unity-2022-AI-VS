using UnityEngine;

public class Idle : State<FSMBase>
{
    public override void Execute() => Debug.Log("OnIdle" + gameObject.name);
}
