﻿using System;
using UnityEngine;

namespace Utility.Async
{
    public class WaitUntilCallback : CustomYieldInstruction
    {
        private bool _isDone;
        public override bool keepWaiting => !_isDone;

        public WaitUntilCallback(Action<Action> action)
        {
            action(() => _isDone = true);
        }
    }
    
    public class WaitUntilCallback<T0> : CustomYieldInstruction
    {
        private bool _isDone;
        
        public T0 Result { get; private set; }
        public override bool keepWaiting => !_isDone;

        public WaitUntilCallback(Action<Action<T0>> action)
        {
            action(callbackResult =>
            {
                Result = callbackResult;
                _isDone = true;
            });
        }
    }
    
    public class WaitUntilCallback<T0, T1> : CustomYieldInstruction
    {
        private bool _isDone;
        
        public (T0, T1) Result { get; private set; }
        public override bool keepWaiting => !_isDone;

        public WaitUntilCallback(Action<Action<T0, T1>> action)
        {
            action((callbackResult0, callbackResult1) =>
            {
                Result = (callbackResult0, callbackResult1);
                _isDone = true;
            });
        }
        
        public WaitUntilCallback(T0 arg0, Action<T0, Action<T1>> action)
        {
            action(arg0, callbackResult1 =>
            {
                Result = (arg0, callbackResult1);
                _isDone = true;
            });
        }
    }
    
    public class WaitUntilCallback<T0, T1, T2> : CustomYieldInstruction
    {
        private bool _isDone;
        
        public (T0, T1, T2) Result { get; private set; }
        public override bool keepWaiting => !_isDone;

        public WaitUntilCallback(Action<Action<T0, T1, T2>> action)
        {
            action((callbackResult0, callbackResult1, callbackResult2) =>
            {
                Result = (callbackResult0, callbackResult1, callbackResult2);
                _isDone = true;
            });
        }
        
        public WaitUntilCallback(T0 arg0, Action<T0, Action<T1, T2>> action)
        {
            action(arg0, (callbackResult1, callbackResult2) =>
            {
                Result = (arg0, callbackResult1, callbackResult2);
                _isDone = true;
            });
        }
        
        public WaitUntilCallback(T0 arg0, T1 arg1, Action<T0, T1, Action<T2>> action)
        {
            action(arg0, arg1, callbackResult2 =>
            {
                Result = (arg0, arg1, callbackResult2);
                _isDone = true;
            });
        }
    }
    
    public class WaitUntilCallback<T0, T1, T2, T3> : CustomYieldInstruction
    {
        private bool _isDone;
        
        public (T0, T1, T2, T3) Result { get; private set; }
        public override bool keepWaiting => !_isDone;

        public WaitUntilCallback(Action<Action<T0, T1, T2, T3>> action)
        {
            action((callbackResult0, callbackResult1, callbackResult2, callbackResult3) =>
            {
                Result = (callbackResult0, callbackResult1, callbackResult2, callbackResult3);
                _isDone = true;
            });
        }
        
        public WaitUntilCallback(T0 arg0, Action<T0, Action<T1, T2, T3>> action)
        {
            action(arg0, (callbackResult1, callbackResult2, callbackResult3) =>
            {
                Result = (arg0, callbackResult1, callbackResult2, callbackResult3);
                _isDone = true;
            });
        }
        
        public WaitUntilCallback(T0 arg0, T1 arg1, Action<T0, T1, Action<T2, T3>> action)
        {
            action(arg0, arg1, (callbackResult2, callbackResult3) =>
            {
                Result = (arg0, arg1, callbackResult2, callbackResult3);
                _isDone = true;
            });
        }
        
        public WaitUntilCallback(T0 arg0, T1 arg1, T2 arg2, Action<T0, T1, T2, Action<T3>> action)
        {
            action(arg0, arg1, arg2, callbackResult3 =>
            {
                Result = (arg0, arg1, arg2, callbackResult3);
                _isDone = true;
            });
        }
    }
}