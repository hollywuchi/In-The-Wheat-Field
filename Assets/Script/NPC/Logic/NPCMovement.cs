using System;
using System.Collections;
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
    private float animationBreakTime;
    private bool canPlayStopAnimation;

    private Vector2 dir;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;
    public bool isMoving;
    public bool interactble;

    // componment

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;

    private Grid grid;
    private Stack<MovementStep> movementSteps;
    private bool isInitialised;
    private bool npcMove;
    private bool sceneLoaded;

    private AnimationClip stopAnimationClip;
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;

    private TimeSpan gameTime => TimeManager.Instance.GameTime;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();
        scheduleSet = new SortedSet<ScheduleDetails>();

        // 初始化动画控制器，可以通过更改animOverride来更改动画
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride;

        foreach (var schedule in scheduleData.scheduleList)
        {
            scheduleSet.Add(schedule);
        }
    }

    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
    }


    void Update()
    {
        if (sceneLoaded)
            SwitchAnimation();

        // 计时器
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTime <= 0;
    }

    void FixedUpdate()
    {
        if (sceneLoaded)
            Movement();
    }

    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }

    private void OnAfterSceneLoadEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckVisiable();

        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }
        sceneLoaded = true;
    }
    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        int time = (hour * 100) + minute;
        ScheduleDetails matchSchedule = null;

        foreach (var schedule in scheduleSet)
        {
            if (schedule.Time == time)
            {
                if (schedule.season != season)
                    continue;
                if (schedule.day != day && schedule.day != 0)
                    continue;
                matchSchedule = schedule;
            }
            else if (schedule.Time > time)
            {
                break;
            }
        }
        if (matchSchedule != null)
            BuildPath(matchSchedule);
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

    private void Movement()
    {
        if (!npcMove)
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();    // 移除列表中的第一个并获得

                currentScene = step.sceneName;

                CheckVisiable();

                nextGridPosition = (Vector3Int)step.gridCoordinate;

                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPositon(nextGridPosition, stepTime);
            }
            else if (!isMoving && canPlayStopAnimation)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridPositon(Vector3Int gridPos, TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        nextWorldPosition = GetWorldPosition(gridPos);
        npcMove = true;

        // 还有时间进行移动
        if (stepTime > gameTime)
        {
            // 用来移动的时间差
            float timeToMove = (float)(stepTime.TotalSeconds - gameTime.TotalSeconds);
            // 实际移动的距离
            float distance = Vector3.Distance(transform.position, nextWorldPosition);

            // 实际移动的速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThresHold));

            if (speed <= maxSpeed)
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;

                    Vector2 posOffect = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffect);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        // 时间到了，要立马到达这个位置
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;

        npcMove = false;
    }

    /// <summary>
    /// 根据Schedule构建路径
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {

        movementSteps.Clear();
        currentSchedule = schedule;
        targetGridPosition = (Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;
        this.interactble = schedule.interactable;

        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }
        else if (schedule.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);

            if (sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathsList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathsList[i];

                    if (path.fromGirdCell.x >= Settings.maxGridSize)
                    {
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        fromPos = path.fromGirdCell;
                    }

                    if (path.gotoGridCell.x >= Settings.maxGridSize)
                    {
                        gotoPos = schedule.targetGridPosition;
                    }
                    else
                    {
                        gotoPos = path.gotoGridCell;
                    }

                    AStar.Instance.BuildPath(path.sceneName, fromPos, gotoPos, movementSteps);
                }
            }
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

    /// <summary>
    /// 返回一个世界坐标
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }

    private void SwitchAnimation()
    {
        isMoving = transform.position != GetWorldPosition(targetGridPosition);
        anim.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        animationBreakTime = Settings.animationBreakTime;
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }
    #region 设置NPC显示情况
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        // 将影子关闭
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;

        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}
