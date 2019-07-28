using UnityEngine;
using UnityEngine.EventSystems;

public class LizardStateWallTransition : LizardState
{
    public LizardStateWallTransition(PlayerController _Lizard) : base(_Lizard)
    {
    }

    public override void OnStateEnter()
    {
        if (Mathf.Abs(lizardTrans.rotation.eulerAngles.z) > 135f ||  Mathf.Abs(lizardTrans.rotation.y) > 0.7f &&  Lizard.reverseInput == -1)
        {
            Lizard.reverseInput = -1;
            Debug.Log("revert" + Lizard.reverseInput);
        }
        else
        {
            Lizard.reverseInput = 1;
        }
    }

    public override void ApplyDownForce()
    {
        Lizard.currentState.Reorient();
    }

    public override void Reorient()
    {
        wallDetectedPoint = Physics2D.Linecast(Lizard.WallDetectorStart.position, Lizard.WallDetectorEnd.position, Lizard.groundLayer).point;

//        if (wallDetectedPoint == Vector3.zero)
//        {
//            Lizard.playerRB.velocity = new Vector3(Lizard.playerRB.velocity.x, 0f);
//            Lizard.SetState(1);
//        }
    }
}