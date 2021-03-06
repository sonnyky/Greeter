﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : StateManager<Avatar> {

    private Avatar targetObject;

    public Idle(Avatar myObject) : base(myObject)
    {
        targetObject = myObject;
    }

    public override void Tick()
    {
       
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
