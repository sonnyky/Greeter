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
        targetObject.MoveForward(0.01f);
        targetObject.ReachedTarget();
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        targetObject.SetAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_WALK);
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}
