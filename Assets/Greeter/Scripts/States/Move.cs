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

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        targetObject.SetAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_IDLE);
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}
