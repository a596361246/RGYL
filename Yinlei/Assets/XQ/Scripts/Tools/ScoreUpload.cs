using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

using UnityEngine;

using WisdomTree.Common.Function;

namespace XQ
{
    public class ScoreUpload : MonoBehaviour
    {
        //public static int score = 0;

        public struct ExperimentalData
        {
            /// <summary>
            /// 题目内容
            /// </summary>
            public string questionName;
            /// <summary>
            /// 答题开始时间
            /// </summary>
            public DateTime startTime;
            /// <summary>
            /// 
            /// </summary>
            public DateTime endTime;
            /// <summary>
            /// 
            /// </summary>
            public int useTime;
            /// <summary>
            /// 获得分数
            /// </summary>
            public int score;
            /// <summary>
            /// 题目分值
            /// </summary>
            public int maxScore;
            /// <summary>
            /// 正确的答案
            /// </summary>
            public string correctAnswer;
            /// <summary>
            /// 回答的内容
            /// </summary>
            public string result;
        }

        private static Dictionary<string, ExperimentalData> choiceDic = new Dictionary<string, ExperimentalData>(20);
        private static Dictionary<string, ExperimentalData> eassayDic = new Dictionary<string, ExperimentalData>(20);

        /// <summary>
        /// 选择题
        /// </summary>
        /// <param name="question"></param>
        /// <param name="data"></param>
        public static void AddChoiceQues(string question, ExperimentalData data)
        {
            if (!choiceDic.ContainsKey(question))
            {
                choiceDic.Add(question, data);
            }
        }

        /// <summary>
        /// 问答题
        /// </summary>
        /// <param name="question"></param>
        /// <param name="data"></param>
        public static void AddEassayQues(string question, ExperimentalData data)
        {
            if (!eassayDic.ContainsKey(question))
            {
                eassayDic.Add(question, data);
            }
        }
        /// <summary>
        /// 上传实验报告
        /// </summary>
        public static void Upload()
        {
            //解答题
            Content[] eassayContent = new Content[eassayDic.Count];
            int eassayNum = 0;
            foreach (var item in eassayDic)
            {
                string contentName = $"{eassayNum + 1}.{item.Value.questionName}";
                string contentScript = $"{item.Value.result}\n答题时间：{item.Value.startTime}";
                Content content = new Content(contentName, contentScript);
                eassayContent[eassayNum] = content;
                eassayNum++;
            }

            //选择题
            Content[] choiceContent = new Content[choiceDic.Count];
            int choiceNum = 0;
            int score = 0;
            foreach (var item in choiceDic)
            {
                int useTime = item.Value.useTime;
                //DateTime sTime = item.Value.startTime;
                //DateTime eTime = item.Value.endTime;
                //DateTime eTime = item.Value.endTime;
                score += item.Value.score;
                string contentName = $"{choiceNum + 1}.{item.Value.questionName}    题目分值：{item.Value.maxScore}    得分：{item.Value.score}";
                string contentScript = $"{item.Value.correctAnswer}\n学生选择：{item.Value.result}\n答题时间：{item.Value.startTime}";
                Content content = new Content(contentName, contentScript);
                choiceContent[choiceNum] = content;
                choiceNum++;
            }

            Debug.Log(JsonConvert.SerializeObject(choiceContent));
            Debug.Log(JsonConvert.SerializeObject(eassayContent));
            Communication.UploadReport(score, null, url => Communication.OpenWebReport(url),
                new Model($"选择题汇总，得分：{score}", choiceContent),
                new Model("思考题汇总", eassayContent)
            );
        }


        public static void Clear()
        {
            Debug.Log("清除");
            //choiceDic.Clear();
            //eassayDic.Clear();
        }

    }
}


