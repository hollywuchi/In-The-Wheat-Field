using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;

    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;

    private bool inputDisable;

    void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        // 使用lambda表达式会产生巨大的性能开销，在awake中赋值则不会
        rb = GetComponent<Rigidbody2D>();

        // 刚体自带的插帧效果，真的有效改善了主角残影问题
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
    void Update()
    {
        if (!inputDisable)
            playerMoveDir();
        else
            isMoving = false;
        SwitchAnimation();
    }

    void FixedUpdate()
    {
        // 玩家在传送的过程中不能移动
        if (!inputDisable)
            playerMove();
    }

    private void OnMoveToPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void OnAfterSceneLoadEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }
    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        EventHandler.CallExcuteActionAfterAnimation(pos,itemDetails);
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

        if (Input.GetKey(KeyCode.LeftShift))
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
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("inputX", inputX);
                anim.SetFloat("inputY", inputY);
            }
        }
    }


}
