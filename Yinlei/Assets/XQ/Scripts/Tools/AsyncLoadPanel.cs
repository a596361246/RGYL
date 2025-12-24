using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SceneManager = UnityEngine.SceneManagement.SceneManager;
namespace XQ
{

    public class AsyncLoadPanel : MonoBehaviour
    {
        public Image progressImg;
        //public Transform circleImgTrf;
        // float speed = 20f;   
        //public TMP_Text numText;
        private AsyncOperation operation;
        private float showNum = 0;
        private float nowNum = 0;
        public static string sceneName = "MainScene";

        void Start()
        {
            //Screen.fullScreen = true;
            //Time.timeScale = 20;
            StartCoroutine(AsyncLoad(sceneName));
            //StartCoroutine(LoadSceneAsync());
        }

        public void LoadScene(string scene)
        {
            sceneName = scene;
            StartCoroutine(AsyncLoad(sceneName));
        }

        //异步加载需要用协程实现，其一unity不支持多线程，异步加载是指边运行当前场景
        //边加载下一个场景的资源，当资源加载完毕，再切换场景
        public IEnumerator AsyncLoad(string scene)
        {
            yield return new WaitForSeconds(1f);
            //unity异步加载的API
            operation = SceneManager.LoadSceneAsync(scene);
            //不允许切场景
            operation.allowSceneActivation = false;
            yield return operation;
        }

        [SerializeField]
        private Slider sliderProcess;
        [Space]
        [SerializeField]
        private Text sliderText;
        /// <summary>
        /// 进度条进度
        /// </summary>
        private float Progress = 0f;

        IEnumerator LoadSceneAsync()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                if (Progress < 200)
                    Progress++;
                else
                    async.allowSceneActivation = true;
                sliderProcess.value = Progress / 200f;
                sliderText.text = (100 * sliderProcess.value).ToString("f0") + "%";
                yield return null;
            }
            yield return new WaitUntil(() => async.allowSceneActivation);
            yield return new WaitForSeconds(1);
        }

        void Update()
        {
            //转圈圈
            //circleImgTrf.Rotate(Vector3.forward, speed, Space.Self);
            //不为空则开始检测进度
            if (operation != null)
            {
                //Debug.Log("当前进度：" + (operation.progress / 0.9f));
                //加载完毕
                //这里注意：如果你需要实现进度条，那么一定要知道
                //当进度到达90%时其实就不会再动了
                showNum = operation.progress * 100 / 0.9f;
                if (showNum > nowNum)
                {
                    nowNum = Mathf.Clamp(nowNum, showNum, 1 * Time.deltaTime);
                    progressImg.fillAmount = nowNum / 100.0f;
                    //numText.text = nowNum.ToString("f2") + "%";
                }

                if (operation.progress >= 0.9f && nowNum > 90)
                {
                    //可以允许自动切场景了
                    operation.allowSceneActivation = true;

                }
            }
        }
    }
}