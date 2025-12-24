using System;
using System.Collections;
using System.Collections.Generic;

// ========================================================
// 描述："记录一些全局变量和属性"
// ========================================================
namespace XQ
{
    /// <summary>
    /// 考核和学习
    /// </summary>
    public enum GameModel
    {
        LearnModel,

        CheckModel,
    }

    /// <summary>
    /// 步骤
    /// </summary>
    public enum StepModel
    {
        Step1,
        Step2,
        Step3,
    }

    public static class UtilityScript
    {
        /// <summary>
        /// 鼠标操作
        /// </summary>
        public static bool IsCanMouseEvent { get; set; } = false;


        public static GameModel gameModel { get; set; } = GameModel.LearnModel;

        /// <summary>
        /// 当前步骤
        /// </summary>
        public static StepModel stepModel { get; set; } = StepModel.Step1;

        public static bool isCanMove { get; set; } = true;

        //重置属性
        public static void ResetProp()
        {

        }
    }
}

