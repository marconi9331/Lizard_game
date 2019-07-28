using UnityEngine;

public class LizardStateGrounded : LizardState
{
    private Vector3 moveDirection;
    private Vector3 frontDetectorStartPos;
    private Vector3 frontDetectorEndPos;
    private Vector3 backDetectorStartPos;
    private Vector3 backDetectorEndPos;
    float originalLinearDrag;

    public LizardStateGrounded(PlayerController _Lizard) : base(_Lizard)
    {
        lizardTrans = Lizard.playerTR;
        originalLinearDrag = Lizard.playerRB.drag;
    }

    public override void OnStateEnter()
    {
        frontDetectorStartPos = Lizard.FrontDetectorStart.transform.position;
        frontDetectorEndPos = Lizard.FrontDetectorEnd.transform.position;
        backDetectorStartPos = Lizard.BackDetectorStart.transform.position;
        backDetectorEndPos = Lizard.BackDetectorEnd.transform.position;

        Lizard.frontLine = Physics2D.Linecast(frontDetectorStartPos, frontDetectorEndPos, Lizard.groundLayer).point;
        Lizard.backLine = Physics2D.Linecast(backDetectorStartPos, backDetectorEndPos, Lizard.groundLayer).point;

        if (Lizard.CollisionCount <= 0 || (Lizard.frontLine == Vector3.zero && Lizard.backLine == Vector3.zero))
        {
            Lizard.SetState(1);
        }

        Lizard.playerRB.drag = originalLinearDrag;
    }

    public override void OnStateExit()
    {
        Lizard.playerRB.drag = 0;
        Debug.Log("exited Grounded");
    }

    public override void Reorient()
    {
        frontDetectorStartPos = Lizard.FrontDetectorStart.transform.position;
        frontDetectorEndPos = Lizard.FrontDetectorEnd.transform.position;
        backDetectorStartPos = Lizard.BackDetectorStart.transform.position;
        backDetectorEndPos = Lizard.BackDetectorEnd.transform.position;

        Lizard.frontLine = Physics2D.Linecast(frontDetectorStartPos, frontDetectorEndPos, Lizard.groundLayer).point;
        Lizard.backLine = Physics2D.Linecast(backDetectorStartPos, backDetectorEndPos, Lizard.groundLayer).point;


        if (Lizard.frontLine != Vector3.zero && Lizard.backLine != Vector3.zero)
        {
            moveDirection = (Lizard.frontLine - Lizard.backLine).normalized;
//            if (Mathf.Abs(lizardTrans.rotation.x) == 1f )
//            {
//                moveDirection *= -1f;
//            }

            lizardTrans.Rotate(Quaternion.FromToRotation(lizardTrans.right, moveDirection).eulerAngles);
            //zRotation = Mathf.Atan2(moveDirection.x * -lizardTrans.localScale.x, moveDirection.y * lizardTrans.localScale.x) * Mathf.Rad2Deg;
            //lizardTrans.rotation = Quaternion.Euler(lizardTrans.rotation.eulerAngles.x, lizardTrans.rotation.eulerAngles.y, zRotation - 90f);

            #region gimbal lock dirty fix

            //if (Lizard.playerTR.localRotation.y != 0f)
            //{
            //Lizard.playerTR.localRotation = Quaternion.Euler(new Vector3(lizardTrans.rotation.x, 0f, 181f));
            //Lizard.playerTR.right = (moveDirection * -lizardTrans.localScale.x);
            //}
            //Lizard.playerTR.rotation = Quaternion.Euler(moveDirection);

            #endregion
        }
        else
        {
            if (Lizard.CollisionCount <= 0)
                Lizard.SetState(1);
        }
    }

    public override void ApplyDownForce()
    {
        lizardNormal = Lizard.WallDetectorEnd.position - Lizard.WallDetectorStart.position;
        if (Lizard.WallDetectorStart.position == Vector3.zero || Lizard.WallDetectorEnd.position == Vector3.zero)
            return;
        Lizard.currentState.Reorient();
        Lizard.playerRB.AddForce((((-lizardNormal.normalized * -Physics2D.gravity.y) * 60) * Time.deltaTime));
    }

    public override void Jump()
    {
        //if (Input.GetAxisRaw("Jump") > 0)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(wallDetector);
            wallDetectedPoint = Physics2D.Linecast(Lizard.WallDetectorStart.position, Lizard.WallDetectorEnd.position, Lizard.groundLayer).point;
            if (wallDetectedPoint != Vector3.zero)
            {
                wallDetectedPoint = Vector3.zero;
                Lizard.SetState(2);
                Lizard.currentState.Jump();
            }
            else
            {
                Lizard.playerRB.AddForce((lizardNormal.normalized * Lizard.JumpIntensity) * Time.deltaTime, ForceMode2D.Impulse);
                Lizard.SetState(1);
            }
            Lizard.LizAnim.SetTrigger("jump");
        }
    }

    public override void Move()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Lizard.playerRB.velocity == Vector2.zero)
        {
//            if (Mathf.Abs(lizardTrans.rotation.eulerAngles.z) > 135f)
            if (Mathf.Abs(lizardTrans.rotation.eulerAngles.z) > 135f)
            {
                Lizard.reverseInput = -1;
                Debug.Log("revert" + Lizard.reverseInput);
            }
            else
            {
                Lizard.reverseInput = 1;
            }
        }

        Lizard.playerRB.AddForce(Lizard.playerTR.right * (Input.GetAxis("Horizontal") * Time.deltaTime * Lizard.SpeedMultiplier) * Lizard.reverseInput, ForceMode2D.Force);
        Lizard.playerRB.velocity = Vector2.ClampMagnitude(Lizard.playerRB.velocity, 5f);
        
        if (Lizard.playerRB.velocity != Vector2.zero)
        {
            Lizard.LizAnim.SetBool("walking", true);
        }
        else
        {
            Lizard.LizAnim.SetBool("walking", false);
        }
    }
}