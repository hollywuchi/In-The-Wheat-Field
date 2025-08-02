using System;
using System.Collections.Generic;
using Farm.AStar;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;
    [SerializeField] private string currentScene;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;

    public string StartScene { set => currentScene = value; }
    [Header("移动参数")]
    public float normalSpeed = 2;
    private float minSpeed = 1;
    private float maxSpeed = 3;

    private Vector2 dir;
    private bool isMoving;

    // componment

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;

    private Grid grid;
    private Stack<MovementStep> movementSteps;
    private bool isInitialised;

    private TimeSpan gameTime => TimeManager.Instance.GameTime;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent()
    {
        grid = GetComponent<Grid>();
        CheckVisiable();

        if (isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }
    }

    private void CheckVisiable()
    {
        if (currentScene == SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }

    private void InitNPC()
    {
        targetScene = currentScene;
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);

        targetGridPosition = currentGridPosition;
    }

    public void BuildPath(ScheduleDetails schedule)
    {
        movementSteps.Clear();
        currentSchedule = schedule;

        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }

        if (movementSteps.Count > 1)
        {
            // 更新每一步的时间戳
            UpdateTimeOnPath();
        }
    }

    /// <summary>
    /// 循环判断每一步的时间戳
    /// </summary>
    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;
        TimeSpan currentGameTime = gameTime;

        foreach (MovementStep step in movementSteps)
        {
            if (previousStep == null)
                previousStep = step;

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;

            if (MoveInDiagonal(step, previousStep))
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThresHold));
            else
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThresHold));
            // 累加获得下一步的时间戳
            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            // 循环下一步
            previousStep = step;
        }
    }


    /// <summary>
    /// 判断是否走斜方向
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previousStep"></param>
    /// <returns></returns>
    private bool MoveInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    #region 设置NPC显示情况
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        // TODO:将影子关闭
        // transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;

        // transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}
