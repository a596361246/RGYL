using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XQ
{
    /// <summary>
    /// 消息管理类
    /// </summary>
    public class MessageMgr : SingleBase<MessageMgr>
    {
        public Dictionary<string, MsgActionBase> _dicMsg = new Dictionary<string, MsgActionBase>();

        /// <summary>
        /// 注册消息事件
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="action"></param>
        public void RegistMsg(string msgName, Action action)
        {
            if (_dicMsg.ContainsKey(msgName))
            {
                (_dicMsg[msgName] as SubAction).action += action;
            }
            else
            {
                var temp = new SubAction();
                temp.action = action;
                _dicMsg.Add(msgName, temp);
            }

        }
        public void RegistMsg<T>(string msgName, Action<T> action)
        {
            if (_dicMsg.ContainsKey(msgName))
            {
                (_dicMsg[msgName] as SubActionT<T>).action += action;
            }
            else
            {
                var temp = new SubActionT<T>();
                temp.action = action;
                _dicMsg.Add(msgName, temp);
            }
        }
		public void RegistMsg<T,X>(string msgName, Action<T,X> action)
		{
			if (_dicMsg.ContainsKey(msgName))
			{
				(_dicMsg[msgName] as SubActionT<T,X>).action += action;
			}
			else
			{
				var temp = new SubActionT<T,X>();
				temp.action = action;
				_dicMsg.Add(msgName, temp);
			}
		}

		/// <summary>
		/// 注销消息
		/// </summary>
		/// <param name="msgName"></param>
		/// <param name="action"></param>
		public void UnRegistMsg(string msgName, Action action)
        {
            if (_dicMsg.ContainsKey(msgName))
            {
                (_dicMsg[msgName] as SubAction).action -= action;
                _dicMsg.Remove(msgName);
            }
        }
        public void UnRegistMsg<T>(string msgName, Action<T> action)
        {
            if (_dicMsg.ContainsKey(msgName))
            {
                (_dicMsg[msgName] as SubActionT<T>).action -= action;
                _dicMsg.Remove(msgName);
            }
        }
		public void UnRegistMsg<T,X>(string msgName, Action<T,X> action)
		{
			if (_dicMsg.ContainsKey(msgName))
			{
				(_dicMsg[msgName] as SubActionT<T,X>).action -= action;
				_dicMsg.Remove(msgName);
			}
		}


		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="msgName"></param>
		/// <param name="str"></param>
		public void SendMsg(string msgName)
        {
            if (_dicMsg.ContainsKey(msgName))
            {
                (_dicMsg[msgName] as SubAction).action.Invoke();
            }
        }
        public void SendMsg<T>(string msgName, T arg)
        {
            if (_dicMsg.ContainsKey(msgName))
            {
                (_dicMsg[msgName] as SubActionT<T>).action.Invoke(arg);
            }
        }
		public void SendMsg<T,X>(string msgName, T arg,X arg1)
		{
			if (_dicMsg.ContainsKey(msgName))
			{
				(_dicMsg[msgName] as SubActionT<T,X>).action.Invoke(arg,arg1);
			}
		}
	}

    public class MsgName
    {
        public const string UpdateStep = "UpdateStep";
    }


    public class MsgActionBase
    {

	}
	public class SubAction : MsgActionBase
	{
		public Action action;
	}
	public class SubActionT<T> : MsgActionBase
    {
        public Action<T> action;
    }
	public class SubActionT<T,X> : MsgActionBase
	{
		public Action<T,X> action;
	}

}
