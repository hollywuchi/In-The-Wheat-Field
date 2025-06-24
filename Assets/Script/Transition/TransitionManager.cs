using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        public string startSceneName;

        void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }
        void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void OnTransitionEvent(string sceneName, Vector3 position)
        {
            StartCoroutine(Transition(sceneName,position));
        }

        void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
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

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallMoveToPosition(position);

            EventHandler.CallAfterSceneLoadEvent();
        }

        /// <summary>
        /// 加载场景并激活
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            // 异步加载场景，加载模式为添加场景
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // 获取当前加载的场景，为场景列表的最后一个
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }


    }
}