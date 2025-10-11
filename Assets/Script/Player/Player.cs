using System.Collections;
using System.Collections.Generic;
using Farm.Save;
using UnityEditor.Rendering;
using UnityEngine;

public class Player : MonoBehaviour, ISaveable
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;

    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;

    private bool inputDisable;

    // 鼠标方向向量
    private float mouseX;
    private float mouseY;
    // private bool useTool;

    public string GUID => GetComponent<DataGUID>().guid;

    void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }



    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        // 使用lambda表达式会产生巨大的性能开销，在awake中赋值则不会
        rb = GetComponent<Rigidbody2D>();

        // 刚体自带的插帧效果，真的有效改善了主角残影问题
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        inputDisable = true;
    }

    void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
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
    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commondity && itemDetails.itemType != ItemType.Funiture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);
            // 鼠标指针在斜方向的时候，人物没办法斜方向
            // 因此要进行统一
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else
                mouseX = 0;

            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
        }
        else
        {
            EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
        }
    }

    private void OnUpdateGameStateEvent(GameState state)
    {
        switch (state)
        {
            case GameState.Pause:
                inputDisable = true;
                break;

            case GameState.GamePlay:
                inputDisable = false;
                break;
        }
    }
    private void OnStartNewGameEvent(int obj)
    {
        inputDisable = false;
        transform.position = Settings.playerStartPos;
    }

    private void OnEndGameEvent()
    {
        inputDisable = true;
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        // useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in animators)
        {
            anim.SetTrigger("useTool");
            // 人物面朝的方向
            anim.SetFloat("inputX", mouseX);
            anim.SetFloat("inputY", mouseY);
        }
        yield return new WaitForSeconds(0.5f);
        EventHandler.CallExcuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        // 等待动画结束
        // useTool = false;
        inputDisable = false;
    }

    /// <summary>
    /// 玩家移动方向
    /// </summary>
    public void playerMoveDir()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");


        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }

        // if (inputX != 0 && inputY != 0) //限制斜方向的移动速度（已废弃）
        // {
        //     inputX *= 0.7f;
        //     inputY *= 0.7f;
        // }
        // 将向量进行归一化，可以解决玩家斜方向移动速度不匹配问题（最佳答案）
        // 进行归一化之后虽然手感上要好一点，但是之后会有漂移问题
        movementInput = new Vector2(inputX, inputY).normalized;

        isMoving = movementInput != Vector2.zero;
    }
    /// <summary>
    /// 玩家移动
    /// </summary>
    private void playerMove()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.fixedDeltaTime);
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);

            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("inputX", inputX);
                anim.SetFloat("inputY", inputY);
            }
        }
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerialzableVector3>
        {
            { name, new SerialzableVector3(transform.position) }
        };
        // Question: 这里为什么可以简化Add
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[name].ToVector3();

        transform.position = targetPosition;
    }
}
