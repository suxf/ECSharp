#if UNITY_2020_1_OR_NEWER
#nullable enable
#endif
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECSharp
{
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult>();
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult, in TValue1>(TValue1? value1);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult, in TValue1, in TValue2>(TValue1? value1, TValue2? value2);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC<out TResult, in TValue1, in TValue2, in TValue3>(TValue1? value1, TValue2? value2, TValue3? value3);

    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC_WITH_PARAMETER<out TResult>(object? target);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC_WITH_PARAMETER<out TResult, in TValue1>(object? target, TValue1? value1);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC_WITH_PARAMETER<out TResult, in TValue1, in TValue2>(object? target, TValue1? value1, TValue2? value2);
    /// <summary>
    /// 委托函数
    /// </summary>
    public delegate TResult? COMMAND_FUNC_WITH_PARAMETER<out TResult, in TValue1, in TValue2, in TValue3>(object? target, TValue1? value1, TValue2? value2, TValue3? value3);

    internal class WaitData { public ManualResetEventSlim waitHandle = new ManualResetEventSlim(false); public object? parameter; }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey, FuncData> funcMap = new Dictionary<TKey, FuncData>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult?> func, int repeat = -1)
        {
            if (repeat == 0)
                return;

            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0)
                return;

            funcMap.Add(key, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }
            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v1.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return Task.FromResult<TResult?>(default);
            }
            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v1.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        public TResult? Call(TKey key)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.repeat > 0) --v1.repeat;

            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func();
            if (v1.func2 != null) v1.result = v1.func2(v1.parameter);
            return v1.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        public void Call(TKey key, int waitId)
        {
            if (!funcMap.TryGetValue(key, out var v1))
                return;

            if (v1.waitMap == null || !v1.waitMap.TryGetValue(waitId, out var waitData))
            {
                return;
            }

            v1.waitMap.Remove(waitId);

            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func();
            if (v1.func2 != null) v1.result = v1.func2(waitData.parameter ?? v1.parameter);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC<TResult?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func2 == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func2 == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Clear(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult, TValue1> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?, TValue1?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey, FuncData> funcMap = new Dictionary<TKey, FuncData>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult?, TValue1?> func, int repeat = -1)
        {
            if (repeat == 0) return;
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0) return;
            funcMap.Add(key, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v1.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v1.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="value1">传入值</param>
        public TResult? Call(TKey key, TValue1? value1 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func(value1);
            if (v1.func2 != null) v1.result = v1.func2(v1.parameter, value1);
            return v1.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="value1">传入值</param>
        public void Call(TKey key, int waitId, TValue1? value1 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
                return;

            if (v1.waitMap == null || !v1.waitMap.TryGetValue(waitId, out var waitData))
            {
                return;
            }

            v1.waitMap.Remove(waitId);
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func(value1);
            if (v1.func2 != null) v1.result = v1.func2(waitData.parameter ?? v1.parameter, value1);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC<TResult?, TValue1?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?, TValue1?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func2 == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func2 == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Clear(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult, TValue1, TValue2> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?, TValue1?, TValue2?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey, FuncData> funcMap = new Dictionary<TKey, FuncData>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult?, TValue1?, TValue2?> func, int repeat = -1)
        {
            if (repeat == 0) return;
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0) return;
            funcMap.Add(key, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v1.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v1.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public TResult? Call(TKey key, TValue1? value1 = default, TValue2? value2 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func(value1, value2);
            if (v1.func2 != null) v1.result = v1.func2(v1.parameter, value1, value2);
            return v1.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public void Call(TKey key, int waitId, TValue1? value1 = default, TValue2? value2 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
                return;
            if (v1.waitMap == null || !v1.waitMap.TryGetValue(waitId, out var waitData))
            {
                return;
            }

            v1.waitMap.Remove(waitId);
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func(value1, value2);
            if (v1.func2 != null) v1.result = v1.func2(waitData.parameter ?? v1.parameter, value1, value2);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC<TResult?, TValue1?, TValue2?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?, TValue1?, TValue2?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func2 == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func2 == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Clear(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class Command<TKey, TResult, TValue1, TValue2, TValue3> where TKey : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey, FuncData> funcMap = new Dictionary<TKey, FuncData>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?> func, int repeat = -1)
        {
            if (repeat == 0) return;
            funcMap.Add(key, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0) return;
            funcMap.Add(key, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v1.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey key, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v1.waitMap == null)
            {
                v1.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v1.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v1.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public TResult? Call(TKey key, TValue1? value1 = default, TValue2? value2 = default, TValue3? value3 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return default;
            }

            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func(value1, value2, value3);
            if (v1.func2 != null) v1.result = v1.func2(v1.parameter, value1, value2, value3);
            return v1.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key">指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public void Call(TKey key, int waitId, TValue1? value1 = default, TValue2? value2 = default, TValue3? value3 = default)
        {
            if (!funcMap.TryGetValue(key, out var v1))
                return;

            if (v1.waitMap == null || !v1.waitMap.TryGetValue(waitId, out var waitData))
            {
                return;
            }

            v1.waitMap.Remove(waitId);
            if (v1.repeat > 0) --v1.repeat;
            if (v1.repeat == 0)
            {
                funcMap.Remove(key);
            }

            if (v1.func != null) v1.result = v1.func(value1, value2, value3);
            if (v1.func2 != null) v1.result = v1.func2(waitData.parameter ?? v1.parameter, value1, value2, value3);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey key, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key, out var v1))
            {
                return false;
            }

            if (v1.func2 == func)
            {
                funcMap.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap)
            {
                if (v1.Value.func2 == func)
                {
                    funcMap.Remove(v1.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key">指令名</param>
        public bool Clear(TKey key)
        {
            return funcMap.Remove(key);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, FuncData>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, FuncData>>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?> func, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }
            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0)
                return;
            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v2.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v2.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public TResult? Call(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func();
            if (v2.func2 != null) v2.result = v2.func2(v2.parameter);
            return v2.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        public void Call(TKey1 key1, TKey2 key2, int waitId)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
                return;

            if (v2.waitMap == null || !v2.waitMap.TryGetValue(waitId, out var waitData))
                return;

            v2.waitMap.Remove(waitId);
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func();
            if (v2.func2 != null) v2.result = v2.func2(waitData.parameter ?? v2.parameter);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func2 == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func2 == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;

            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult, TValue1> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?, TValue1?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, FuncData>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, FuncData>>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?, TValue1?> func, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v2.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v2.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="value1">传入值</param>
        public TResult? Call(TKey1 key1, TKey2 key2, TValue1? value1 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func(value1);
            if (v2.func2 != null) v2.result = v2.func2(v2.parameter, value1);
            return v2.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="value1">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, int waitId, TValue1? value1 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
                return;

            if (v2.waitMap == null || !v2.waitMap.TryGetValue(waitId, out var waitData))
                return;

            v2.waitMap.Remove(waitId);
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func(value1);
            if (v2.func2 != null) v2.result = v2.func2(waitData.parameter ?? v2.parameter, value1);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?, TValue1?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?, TValue1?> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func2 == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func2 == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;

            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult, TValue1, TValue2> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?, TValue1?, TValue2?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, FuncData>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, FuncData>>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?, TValue1?, TValue2?> func, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v2.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v2.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public TResult? Call(TKey1 key1, TKey2 key2, TValue1? value1 = default, TValue2? value2 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func(value1, value2);
            if (v2.func2 != null) v2.result = v2.func2(v2.parameter, value1, value2);
            return v2.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, int waitId, TValue1? value1 = default, TValue2? value2 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
                return;

            if (v2.waitMap == null || !v2.waitMap.TryGetValue(waitId, out var waitData))
                return;

            v2.waitMap.Remove(waitId);
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func(value1, value2);
            if (v2.func2 != null) v2.result = v2.func2(waitData.parameter ?? v2.parameter, value1, value2);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?, TValue1, TValue2> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?, TValue1, TValue2> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1, TValue2> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func2 == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1, TValue2> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func2 == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;

            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }

    /// <summary>
    /// 命令控制
    /// </summary>
    public sealed class MultiCommand<TKey1, TKey2, TResult, TValue1, TValue2, TValue3> where TKey1 : notnull where TKey2 : notnull
    {
        private class FuncData { public TResult? result; public COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?>? func; public COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?>? func2; public int repeat; public object? parameter; public Dictionary<int, WaitData>? waitMap; }
        private readonly Dictionary<TKey1, Dictionary<TKey2, FuncData>> funcMap = new Dictionary<TKey1, Dictionary<TKey2, FuncData>>();

        private int waitIndex = 0;
        /// <summary>
        /// 自增等待ID
        /// </summary>
        public int AutoWaitID => waitIndex++;

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?> func, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func = func });
        }

        /// <summary>
        /// 增加指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="func">委托函数</param>
        /// <param name="parameter">传入参数</param>
        /// <param name="repeat">重复次数 默认 -1 无限重复</param>
        public void Add(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?> func, object? parameter, int repeat = -1)
        {
            if (repeat == 0)
                return;

            if (!funcMap.TryGetValue(key1, out var v1))
            {
                v1 = new Dictionary<TKey2, FuncData>();
                funcMap.Add(key1, v1);
            }

            v1.Add(key2, new FuncData() { repeat = repeat, func2 = func, parameter = parameter });
        }

        /// <summary>
        /// 等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public TResult? WaitCall(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return v2.result;
        }

        /// <summary>
        /// 异步等待执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="parameter">传入参数，此处不为空会覆盖添加处的参数值</param>
        /// <param name="waitTimeout">超时时间 默认 -1 永不超时</param>
        public Task<TResult?> WaitCallAsync(TKey1 key1, TKey2 key2, int waitId, object? parameter = null, int waitTimeout = -1)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return Task.FromResult<TResult?>(default);
            }

            if (v2.waitMap == null)
            {
                v2.waitMap = new Dictionary<int, WaitData>();
            }

            WaitData waitData = new WaitData() { parameter = parameter };
            v2.waitMap.Add(waitId, waitData);
            waitData.waitHandle.Wait(waitTimeout);
            return Task.FromResult(v2.result);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public TResult? Call(TKey1 key1, TKey2 key2, TValue1? value1 = default, TValue2? value2 = default, TValue3? value3 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return default;
            }

            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func(value1, value2, value3);
            if (v2.func2 != null) v2.result = v2.func2(v2.parameter, value1, value2, value3);
            return v2.result;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        /// <param name="waitId">等待ID</param>
        /// <param name="value1">传入值</param>
        /// <param name="value2">传入值</param>
        /// <param name="value3">传入值</param>
        public void Call(TKey1 key1, TKey2 key2, int waitId, TValue1? value1 = default, TValue2? value2 = default, TValue3? value3 = default)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
                return;

            if (v2.waitMap == null || !v2.waitMap.TryGetValue(waitId, out var waitData))
                return;

            v2.waitMap.Remove(waitId);
            if (v2.repeat > 0) --v2.repeat;
            if (v2.repeat == 0)
            {
                v1.Remove(key2);
            }

            if (v2.func != null) v2.result = v2.func(value1, value2, value3);
            if (v2.func2 != null) v2.result = v2.func2(waitData.parameter ?? v2.parameter, value1, value2, value3);
            waitData.waitHandle.Set();
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定指令
        /// </summary>
        public bool Remove(TKey1 key1, TKey2 key2, COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            if (!funcMap.TryGetValue(key1, out var v1) || !v1.TryGetValue(key2, out var v2))
            {
                return false;
            }

            if (v2.func2 == func)
            {
                v1.Remove(key2);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指令
        /// </summary>
        public void Remove(COMMAND_FUNC_WITH_PARAMETER<TResult?, TValue1?, TValue2?, TValue3?> func)
        {
            foreach (var v1 in funcMap)
            {
                foreach (var v2 in v1.Value)
                {
                    if (v2.Value.func2 == func)
                    {
                        v1.Value.Remove(v2.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定指令
        /// </summary>
        /// <param name="key1">一级指令名</param>
        /// <param name="key2">二级指令名</param>
        public bool Clear(TKey1 key1, TKey2 key2)
        {
            if (!funcMap.TryGetValue(key1, out var v1))
                return false;

            return v1.Remove(key2);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            funcMap.Clear();
        }
    }
}
