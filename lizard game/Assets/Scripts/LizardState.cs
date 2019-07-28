using UnityEngine;

public abstract class LizardState
{
    public PlayerController Lizard;
    protected Transform lizardTrans;
    protected Vector2 lizardNormal;
    protected Vector3 wallDetectedPoint;


    public LizardState(PlayerController _Lizard)
    {
        this.Lizard = _Lizard;
        this.lizardTrans = Lizard.transform;
    }

    public virtual void OnStateEnter()
    {
    }

    public virtual void OnStateExit()
    {
    }

    public virtual void Reorient()
    {
    }

    public virtual void ApplyDownForce()
    {
    }

    public virtual void Jump()
    {
    }

    public virtual void Move()
    {
    }


}