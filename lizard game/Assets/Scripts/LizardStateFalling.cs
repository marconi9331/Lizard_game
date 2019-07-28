using UnityEngine;

public class LizardStateFalling : LizardState
{
    public LizardStateFalling(PlayerController _Lizard) : base(_Lizard)
    {
    }

    public override void OnStateEnter()
    {
        Lizard.playerRB.drag = 0f;
        Lizard.reverseInput = 1;
//        Debug.Log("entered falling");
    }

    public override void Reorient()
    {
        if (Lizard.CollisionCount > 0 && Lizard.playerRB.velocity == Vector2.zero)
        {
            Lizard.SetState(0);
        }
        lizardTrans.Rotate(Quaternion.FromToRotation(lizardTrans.up, Vector3.up).eulerAngles);
//        Lizard.transform.right = Vector3.right;
    }

    public override void ApplyDownForce()
    {
        Lizard.currentState.Reorient();
        //Lizard.playerRB.AddForce(Physics2D.gravity * Time.deltaTime* 30f);
        Lizard.playerRB.gravityScale = 3f;
    }
}