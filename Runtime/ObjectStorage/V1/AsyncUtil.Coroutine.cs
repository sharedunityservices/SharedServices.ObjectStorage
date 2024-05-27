using System;

namespace SharedServices.ObjectStorage.V1
{
    public static partial class AsyncUtil
    {
        public static WaitUntilCallback CallbackToIEnumerator(Action<Action> action) => new(action);
        public static WaitUntilCallback<T0> CallbackToIEnumerator<T0>(Action<Action<T0>> action) => new(action);
        public static WaitUntilCallback<T0, T1> CallbackToIEnumerator<T0, T1>(Action<Action<T0, T1>> action) => new(action);
        public static WaitUntilCallback<T0, T1, T2> CallbackToIEnumerator<T0, T1, T2>(Action<Action<T0, T1, T2>> action) => new(action);
        public static WaitUntilCallback<T0, T1, T2, T3> CallbackToIEnumerator<T0, T1, T2, T3>(Action<Action<T0, T1, T2, T3>> action) => new(action);
        
        public static WaitUntilCallback<TArg0, T0> CallbackToIEnumerator<TArg0, T0>(TArg0 arg0, Action<TArg0, Action<T0>> action) => new(arg0, action);
        public static WaitUntilCallback<TArg0, T0, T1> CallbackToIEnumerator<TArg0, T0, T1>(TArg0 arg0, Action<TArg0, Action<T0, T1>> action) => new(arg0, action);
        public static WaitUntilCallback<TArg0, T0, T1, T2> CallbackToIEnumerator<TArg0, T0, T1, T2>(TArg0 arg0, Action<TArg0, Action<T0, T1, T2>> action) => new(arg0, action);
        public static WaitUntilCallback<TArg0, TArg1, T0> CallbackToIEnumerator<TArg0, TArg1, T0>(TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1, Action<T0>> action) => new(arg0, arg1, action);
        public static WaitUntilCallback<TArg0, TArg1, T0, T1> CallbackToIEnumerator<TArg0, TArg1, T0, T1>(TArg0 arg0, TArg1 arg1, Action<TArg0, TArg1, Action<T0, T1>> action) => new(arg0, arg1, action);
        public static WaitUntilCallback<TArg0, TArg1, TArg2, T0> CallbackToIEnumerator<TArg0, TArg1, TArg2, T0>(TArg0 arg0, TArg1 arg1, TArg2 arg2, Action<TArg0, TArg1, TArg2, Action<T0>> action) => new(arg0, arg1, arg2, action);
    }
}