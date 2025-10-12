using System.Collections;
using Farm.Save;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : Singleton<TransitionManager>, ISaveable
    {
        [SceneName]
        public string startSceneName;

        private CanvasGroup canvasGroup;
        public bool isFade;
        private AsyncOperation operation;

        public string GUID => GetComponent<DataGUID>().guid;

        protected override void Awake()
        {
            base.Awake();
            // 在开始游戏之前添加UI场景
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }
        void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;
        }
        void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;
        }



        private void OnStartNewGameEvent(int obj)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }

        // Start 竟然也可以改成协程形式
        void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            canvasGroup = FindObjectOfType<CanvasGroup>();
        }

        private void OnTransitionEvent(string sceneName, Vector3 position)
        {
            if (!isFade)
                StartCoroutine(Transition(sceneName, position));
        }
        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneName">目标场景名称</param>
        /// <param name="position">玩家落地位置</param>
        /// <returns></returns>
        public IEnumerator Transition(string sceneName, Vector3 position)
        {

            EventHandler.CallBeforeSceneUnloadEvent();

            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);


            EventHandler.CallMoveToPosition(position);

            EventHandler.CallAfterSceneLoadEvent();

            // 如果没有新场景没有加载完，那么就不能淡出
            if (operation.isDone)
                yield return Fade(0);
        }

        /// <summary>
        /// 加载场景并激活
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            // 异步加载场景，加载模式为添加场景
            yield return operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // 获取当前加载的场景，为场景列表的最后一个
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }

        /// <summary>
        /// 切换场景时的淡入淡出
        /// </summary>
        /// <param name="targetAlpha">1为黑，0为透明</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;
            canvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(canvasGroup.alpha - targetAlpha) / Settings.FadeDuration;

            while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            isFade = false;
            canvasGroup.blocksRaycasts = false;
        }

        private IEnumerator LoadSaveDataScene(string sceneName)
        {
            yield return Fade(1);

            if (SceneManager.GetActiveScene().name != "PersistentScene")
            {
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }

            yield return LoadSceneSetActive(sceneName);
            EventHandler.CallAfterSceneLoadEvent();

            yield return Fade(0);
        }

        public IEnumerator UnloadScene()
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            yield return Fade(0);
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName = SceneManager.GetActiveScene().name;

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}