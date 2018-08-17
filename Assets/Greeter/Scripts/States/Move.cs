using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : StateManager<Avatar> {

    private Avatar targetObject;

    public Move(Avatar myObject) : base(myObject)
    {
        targetObject = myObject;
    }

    public override void Tick()
    {
        targetObject.FaceTargetPosition();
        targetObject.MoveForward(0.1f);
        targetObject.ReachedTarget();
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        targetObject.SetAnimation(2);
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}
