using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb => gameObject.GetComponent<Rigidbody2D>();
    public float speed;
    private float inputX;
    private float inputY;

    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }
    void Update()
    {
        playerMoveDir();
        SwitchAnimation();
    }

    void FixedUpdate()
    {
        playerMove();
    }

    /// <summary>
    /// 玩家移动方向
    /// </summary>
    public void playerMoveDir()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if (inputX != 0 && inputY != 0) //限制斜方向的移动速度
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);

        isMoving = movementInput != Vector2.zero;
    }
    /// <summary>
    /// 玩家移动
    /// </summary>
    private void playerMove()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        foreach(var anim in animators)
        {
            anim.SetBool("isMoving",isMoving);
            if(isMoving)
            {
                anim.SetFloat("inputX",inputX);
                anim.SetFloat("inputY",inputY);
            }
        }
    }


}
