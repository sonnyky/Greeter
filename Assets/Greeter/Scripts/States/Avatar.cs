using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour {

    private StateManager<Avatar> m_CurrentState;

    internal Initialize m_InitState;
    internal Idle m_IdleState;
    internal Move m_MoveState;

    private Vector3 m_Target;

    private Vector3 m_CameraPosition;
    private float m_TurnSpeed = 2.5f;

    private QuerySDMecanimController m_MecanimController;

    // Use this for initialization
    void Start () {

        m_InitState = new Initialize(this);
        m_MoveState = new Move(this);
        m_IdleState = new Idle(this);

        m_MecanimController = gameObject.GetComponent<QuerySDMecanimController>();

        m_CameraPosition = Camera.main.transform.position;
        m_Target = m_CameraPosition;

        SetState(m_MoveState);
	}
	
	// Update is called once per frame
	void Update () {
        if (m_CurrentState != null)
        {
            m_CurrentState.Tick();
        }
	}

    public void FaceTargetPosition()
    {
        Vector3 _direction = (m_Target - transform.position).normalized;
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * m_TurnSpeed);
    }

    public void SetState(StateManager<Avatar> nextState)
    {
        if (m_CurrentState != null) m_CurrentState.OnStateExit();
        m_CurrentState = nextState;
        if (m_CurrentState != null) m_CurrentState.OnStateEnter();
    }

    public void MoveForward(float speed)
    {
        transform.position += transform.forward * speed;
    }

    public void ReachedTarget()
    {
        if((transform.position - m_Target).sqrMagnitude < 4f)
        {
            SetState(m_IdleState);
        }
    }

    public void SetAnimation(QuerySDMecanimController.QueryChanSDAnimationType anim)
    {
        m_MecanimController.ChangeAnimation(anim, true);
    }

}
