using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName;

        private CanvasGroup canvasGroup;
        private bool isFade;
        private AsyncOperation operation;
        void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }
        void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        // Start 竟然也可以改成协程形式
        IEnumerator Start()
        {
            canvasGroup = FindObjectOfType<CanvasGroup>();
            yield return LoadSceneSetActive(startSceneName);
            // 目的是，在游戏初始化之后也能执行场景加载后的事件
            EventHandler.CallAfterSceneLoadEvent();
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
        private IEnumerator Transition(string sceneName, Vector3 position)
        {

            EventHandler.CallBeforeSceneUnloadEvent();

            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);


            EventHandler.CallMoveToPosition(position);

            EventHandler.CallAfterSceneLoadEvent();

            // 如果没有新场景没有加载完，那么就不能淡出
            if(operation.isDone)
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

    }
}