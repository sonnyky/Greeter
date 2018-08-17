using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : StateManager<Avatar> {

    private Avatar targetObject;

    public Initialize(Avatar myObject) : base(myObject)
    {
        targetObject = myObject;
    }

    public override void Tick()
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}
