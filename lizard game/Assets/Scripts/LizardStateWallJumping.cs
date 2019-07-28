using UnityEngine;

public class LizardStateWallJumping : LizardState
{
    public LizardStateWallJumping(PlayerController _Lizard) : base(_Lizard)
    {
    }

   

    public override void Jump()
    {
        lizardNormal = new Vector2(Lizard.WallDetectorEnd.position.x - Lizard.WallDetectorStart.position.x, Lizard.WallDetectorEnd.position.y - Lizard.WallDetectorStart.position.y);
        Lizard.playerRB.AddForce((lizardTrans.up * Lizard.JumpIntensity * 2) * Time.deltaTime, ForceMode2D.Impulse);
        lizardTrans.Rotate(Vector3.right, 180f, Space.Self);

        //lizardTrans.rotation = Quaternion.Euler(lizardTrans.rotation.eulerAngles.x + 180f, lizardTrans.rotation.eulerAngles.y, lizardTrans.rotation.eulerAngles.z);
        //        Debug.Break();

        //lizardTrans.localScale = new Vector3(lizardTrans.localScale.x, -lizardTrans.localScale.y, lizardTrans.localScale.z);
        //        Lizard.playerRB.AddForce((lizardNormal * Lizard.JumpIntensity) * Time.deltaTime, ForceMode2D.Impulse);
        Lizard.SetState(3);
        Lizard.currentState.Reorient();
    }
}