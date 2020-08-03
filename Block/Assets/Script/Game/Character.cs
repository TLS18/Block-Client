using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private float MoveTime; //运动保持时间
    private float lastSendInfoTime;  //发送位置的时间
    public Vector3 recvPos; //接收到的位置
    public PlayerMgr.CtrlType ctrlType;
    public float speed;
    public bool isOnGround;
    public bool jumpPressed;
    public float jumpForce;
    public LayerMask ground;
    public GameObject groundCheck;
    public float checkRadius;
    public Canvas canvas;
    public int animInfo;  //动画状态
    
    void Awake()
    {
        ctrlType = PlayerMgr.CtrlType.net;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        lastSendInfoTime = float.MinValue;
        MoveTime = 0.3f;
        recvPos = transform.position;
    }

    void Update()
    {
        if(ctrlType == PlayerMgr.CtrlType.player)
        {
            if(Input.GetKeyDown(KeyCode.W)&&isOnGround==true)
            {
                jumpPressed = true;
            }
            if(Time.time - lastSendInfoTime > 0.2f)
            {
                PlayerMgr.instance.SendInfo();
                lastSendInfoTime = Time.time;
            }
            SetAnimInfo();
        }
        if(ctrlType == PlayerMgr.CtrlType.net)
        {
            float x = Vector3.MoveTowards(transform.position, recvPos, 4f * Time.deltaTime).x;
            float y = Vector3.MoveTowards(transform.position, recvPos, 7f * Time.deltaTime).y;
            transform.position = new Vector3(x,y);
            Debug.Log(animInfo);
        }
        
    }

    void FixedUpdate()
    {
        if (EditNameBtn.instance.isEnable && !Chat.instance.chatInput.isFocused)
        {
            isOnGround = Physics2D.OverlapCircle(groundCheck.transform.position, checkRadius, ground);
            animator.SetBool("isOnGround", isOnGround);                 
            if (ctrlType == PlayerMgr.CtrlType.player)
            {
                Movement();
                Jump();
            }
            if (ctrlType == PlayerMgr.CtrlType.net)
            {
                if (animator != null)
                {
                    animator.SetInteger("animInfo", animInfo);
                    if (transform.position.x != recvPos.x) { animator.SetBool("isMoving", true); }
                    else { animator.SetBool("isMoving", false); }
                    if (transform.position.y != recvPos.y) { animator.SetBool("isJumping", true); }
                    else { animator.SetBool("isJumping", false); }
                }
            }
        }  
    }

    void Movement()
    {
        float xVelocity = Input.GetAxisRaw("Horizontal");
        rigidbody2D.velocity = new Vector2(xVelocity * speed, rigidbody2D.velocity.y);
        if (xVelocity != 0)
        { 
            canvas.transform.localScale = new Vector3(Mathf.Sign(xVelocity) * 0.2f,canvas.transform.localScale.y);
            transform.localScale = new Vector3(Mathf.Sign(xVelocity) * 3, 3, 3);
        }
        animator.SetFloat("xspeed", Mathf.Abs(rigidbody2D.velocity.x));
        animator.SetFloat("yspeed", rigidbody2D.velocity.y);
    }

    void Jump()
    {
        if (jumpPressed && isOnGround)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
            animator.SetTrigger("jump");
            jumpPressed = false;
        }
    }

    private void OnDrawGizmosSelected()  //在scene中可以看到范围
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.transform.position, checkRadius);
    }

    public void NetCtrl(Vector3 pos, float xScale,int anim)
    {
        animInfo = anim;
        recvPos = pos;
        transform.localScale = new Vector3(xScale, 3, 3);
        Canvas canvas = transform.Find("Canvas").GetComponent<Canvas>();
        canvas.transform.localScale = new Vector3(Mathf.Sign(xScale) * 0.2f, canvas.transform.localScale.y);
        
    }




    public void SetAnimInfo()
    {
        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.IsName("Idle")) { animInfo = 0; }
        if (animatorState.IsName("Walk")) { animInfo = 1; }
        if (animatorState.IsName("Jump")) { animInfo = 2; }
        if (animatorState.IsName("Fall")) { animInfo = 3; }
    }


}
