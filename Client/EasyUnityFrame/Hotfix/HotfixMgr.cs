#if !UNITY_2020_1_OR_NEWER
using ES.Time;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace ES.Hotfix
{
    /// <summary>
    /// 热更新模块管理器
    /// <para>通过主函数Load来加载Dll模块</para>
    /// </summary>
    public static class HotfixMgr
    {
        private static bool isLoading = false;
        /// <summary>
        /// 程序集装载器对象
        /// </summary>
        private static AssemblyLoader? assemblyLoader;
        /// <summary>
        /// 代理引用包
        /// </summary>
        private static readonly List<WeakReference<AgentRef>> agentRefs = new List<WeakReference<AgentRef>>();
        /// <summary>
        /// 时间流
        /// </summary>
        private static readonly BaseTimeFlow tf;
        /// <summary>
        /// 代理类型字典
        /// </summary>
        internal static ConcurrentDictionary<Type, Type> agentTypeMap = new ConcurrentDictionary<Type, Type>();
        /// <summary>
        /// 是否为第一次加载
        /// <para>只有在第一次加载才为true，之后都是false</para>
        /// <para>第二次调用Load后变化</para>
        /// </summary>
        public static bool IsFirstLoad { get; private set; } = true;
        /// <summary>
        /// 加载次数
        /// </summary>
        private static int loadCount = 0;
        /// <summary>
        /// 更新检测
        /// </summary>
        private readonly static UpdateCheck updateCheck;

        /// <summary>
        /// 创建一个热更管理器
        /// </summary>
        static HotfixMgr()
        {
            updateCheck = new UpdateCheck();
            tf = BaseTimeFlow.CreateTimeFlow(updateCheck, 0);
            tf.StartTimeFlowES();
        }

        /// <summary>
        /// 载入热更新模块
        /// <para>读取不能保证所有情况都是正常的，建议用try-catch捕获异常，以确保未正确替换，还能保持旧环境继续运行</para>
        /// <para>主入口包含一个静态Main函数 public static void Main(string[] args){}</para>
        /// </summary>
        /// <param name="assemblyFileName">程序集文件名(后缀小写或不写且程序集需要在运行根目录下)</param>
        /// <param name="classFullName">含有 public static void Main(string[] args){} 程序集下热更模块主入口类全称(即命名空间和类名)</param>
        /// <param name="args">传入热更层字符串数组</param>
        /// <param name="entryMethodName">入口函数名称，默认不指定为 Main</param>
        /// <returns>本次加载是否执行，进入执行且完成为true，未执行为false</returns>
        public static bool Load(string assemblyFileName, string classFullName, /*bool keepMainValue = false,*/ string[]? args = null, string entryMethodName = "Main")
        {
            if (isLoading /*|| loadTime > DateTime.Now.AddSeconds(-3)*/) return false;
            isLoading = true;
            if (++loadCount >= 2) IsFirstLoad = false;
            // LmBinder.ResetEvent.Reset();
            if (args == null) args = new string[0];
            // 简单的处理了一下可能携带的后缀格式
            assemblyFileName = assemblyFileName.Replace(".dll", "");
            var dllName = assemblyFileName + ".dll";
            var pdbName = assemblyFileName + ".pdb";
            // 处理开始
            FileStream? fsRead = null;
            FileStream? pdbRead = null;
            try
            {
                if (!File.Exists(dllName)) throw new NullReferenceException($"[{assemblyFileName}] file is not found!");
                fsRead = new FileStream(dllName, FileMode.Open);
                if (File.Exists(pdbName)) pdbRead = new FileStream(pdbName, FileMode.Open);
                int fsLen = (int)fsRead.Length;
                if (fsLen <= 0) throw new NullReferenceException($"Assembly file is empty!");
                // 取得临时数据
                var tempDllAssemblyLoader = new AssemblyLoader(fsRead, pdbRead);
                if (!tempDllAssemblyLoader.IsAlive) throw new NullReferenceException($"Assembly is not alive!");
                // 最终绑定对象
                var assembly = tempDllAssemblyLoader.GetAssembly();
                var typeInfo = assembly.GetType(classFullName);
                if (typeInfo == null) throw new NullReferenceException($"[{classFullName}] class is not found!");
                var methodInfo = typeInfo.GetMethod(entryMethodName, BindingFlags.Public | BindingFlags.Static);
                if (methodInfo == null) throw new NullReferenceException($"[{entryMethodName}] public static method is not found!");
                if (methodInfo.GetParameters().Length != 1) throw new NullReferenceException($"[{entryMethodName}] method has not one args!");
                // 处理代理类字典
                var agentTypeMapTemp = new ConcurrentDictionary<Type, Type>();
                var allTypes = assembly.GetTypes();
                var abstractAgentType = typeof(AbstractAgent);
                var iAgentType = typeof(IAgent<>);
                for (int i = 0, len = allTypes.Length; i < len; i++)
                {
                    var type = allTypes[i];
                    var interfaceTypes = type.GetInterfaces();
                    Type? interfaceType = null;
                    for (int j = interfaceTypes.Length - 1; j >= 0; j--)
                    {
                        var temp = interfaceTypes[j];
                        if (temp.IsAssignableFrom(type) && temp.IsGenericType)
                        {
                            interfaceType = temp;
                            break;
                        }
                    }
                    if (interfaceType != null && abstractAgentType.IsAssignableFrom(type))
                    {
                        agentTypeMapTemp.TryAdd(interfaceType.GetGenericArguments()[0], type);
                    }
                }
                Interlocked.Exchange(ref agentTypeMap, agentTypeMapTemp);
                // 处理代理索引
                lock (agentRefs)
                {
                    // 先暂停时间流
                    foreach (var item in agentRefs)
                    {
                        if (item.TryGetTarget(out var agentRef) && agentRef._agent != null && agentRef._agent is ITimeUpdate)
                        {
                            // 处理时间流代理停止
                            TimeFlowManager.Instance.CloseByObj((ITimeUpdate)agentRef._agent);
                        }
                    }
                    // 后重置所有代理
                    for (int i = agentRefs.Count - 1; i >= 0; i--)
                    {
                        if (agentRefs[i].TryGetTarget(out var agentRef))
                        {
                            agentRef.isCreated = false;
                            if (agentRef.isAutoCreate)
                            {
                                agentRef.CreateAsyncAgent();
                            }
                            else agentRef._agent = null;
                        }
                        else agentRefs.RemoveAt(i);
                    }
                }
                // 拷贝入口函数所在类的所有静态成员变量和属性
                // if (keepMainValue && assemblyLoader != null)
                // {
                //     var oldTypeInfo = assemblyLoader.GetAssembly().GetType(classFullName);
                //     var fields = typeInfo.GetFields(BindingFlags.Public | BindingFlags.Static);
                //     var properties = typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Static);
                //     for (int i = 0, len = fields.Length; i < len; i++)
                //     {
                //         var newField = fields[i];
                //         var oldField = oldTypeInfo.GetField(newField.Name);
                //         if (newField.GetType() == oldField.GetType() && !newField.IsInitOnly)
                //             newField.SetValue(typeInfo, oldField.GetValue(oldTypeInfo));
                //     }
                //     for (int i = 0, len = properties.Length; i < len; i++)
                //     {
                //         var newProperty = properties[i];
                //         var oldProperty = oldTypeInfo.GetProperty(newProperty.Name);
                //         if (newProperty.GetType() == oldProperty.GetType() && newProperty.CanWrite)
                //             newProperty.SetValue(typeInfo, oldProperty.GetValue(oldTypeInfo));
                //     }
                // }
                var waitDestroyLoader = assemblyLoader;
                // 调用入口函数
                methodInfo.Invoke(Interlocked.Exchange(ref assemblyLoader, tempDllAssemblyLoader), new object[] { args });
                // 程序域转换
                if (waitDestroyLoader != null && waitDestroyLoader.IsAlive) waitDestroyLoader.Unload();
                if (fsRead != null) fsRead.Close();
                if (pdbRead != null) pdbRead.Close();
            }
            catch
            {
                if (fsRead != null) fsRead.Close();
                if (pdbRead != null) pdbRead.Close();
                throw;
            }
            finally
            {
                isLoading = false;
                // 没有执行成功需要关闭写入锁
            }
            return true;
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        public static Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version!;
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        public static Version? GetAssemblyVersion()
        {
            return assemblyLoader?.GetAssembly()?.GetName().Version;
        }

        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly? GetAssembly()
        {
            return assemblyLoader?.GetAssembly();
        }

        /// <summary>
        /// 增加代理引用
        /// </summary>
        /// <param name="agentRef"></param>
        internal static void AddAgentRef(AgentRef agentRef)
        {
            lock (agentRefs) agentRefs.Add(new WeakReference<AgentRef>(agentRef));
        }

        private class UpdateCheck : ITimeUpdate
        {
            private int periodTime = 0;
            private int lastAgentRefCount = 0;
            /// <summary>
            /// 更新
            /// </summary>
            /// <param name="deltaTime"></param>
            public void Update(int deltaTime)
            {
                // 时间到直接处理弱引用不存在的
                if (periodTime <= 0)
                {
                    periodTime = int.MaxValue;
                    lock (agentRefs)
                        for (int i = agentRefs.Count - 1; i >= 0; i--)
                            if (!agentRefs[i].TryGetTarget(out _)) agentRefs.RemoveAt(i);
                    return;
                }
                if (periodTime == int.MaxValue)
                {
                    lock (agentRefs)
                    {
                        var currentAgentRefCount = agentRefs.Count;
                        // 特殊情况是当前没有引用按照正常10s一次的周期检测
                        if (currentAgentRefCount <= 0)
                        {
                            lastAgentRefCount = currentAgentRefCount;
                            periodTime = 10000;
                            return;
                        }
                        var ratio = lastAgentRefCount * 1.0f / currentAgentRefCount;
                        // 比例增加第一次 周期1s
                        if (ratio == 0) periodTime = 1000;
                        // 缩减10倍 周期60s
                        else if (ratio >= 10) periodTime = 60000;
                        // 缩减5倍 周期30s
                        else if (ratio >= 5) periodTime = 30000;
                        // 缩减2倍 周期15s
                        else if (ratio >= 2) periodTime = 15000;
                        // 前后不变 周期12s
                        else if (ratio >= 1) periodTime = 12000;
                        // 增速1.25倍 周期10s
                        else if (ratio >= 0.8) periodTime = 10000;
                        // 增速2倍 周期8s
                        else if (ratio >= 0.5) periodTime = 8000;
                        // 增速5倍 周期4s
                        else if (ratio >= 0.2) periodTime = 4000;
                        // 增速10倍 周期1s
                        else if (ratio >= 0.1) periodTime = 1000;
                        // 其他情况也是1s
                        else periodTime = 1000;
                        // 记录最后一次数值
                        lastAgentRefCount = currentAgentRefCount;
                    }
                }
                // 减去流逝的时间
                periodTime -= deltaTime;
            }

            /// <summary>
            /// 更新结束
            /// </summary>
            public void UpdateEnd()
            {
            }

        }

        /// <summary>
        /// 程序集装载器
        /// </summary>
        private class AssemblyLoader
        {
            private AssemblyProtectContext context;

            public bool IsAlive { get; private set; } = true;

            public AssemblyLoader(Stream stream, Stream? pdbStream)
            {
                context = new AssemblyProtectContext(stream, pdbStream);
            }

            public Assembly GetAssembly()
            {
                return context.assembly;
            }

            public void Unload()
            {
                if (IsAlive)
                {
                    IsAlive = false;
                    context.UnloadAssembly();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            /// <summary>
            /// 程序集上下文
            /// </summary>
            private class AssemblyProtectContext : AssemblyLoadContext
            {
                public Assembly assembly;

                public AssemblyProtectContext(Stream stream, Stream? pdbStream) : base(true)
                {
                    if (pdbStream == null) assembly = LoadFromStream(stream);
                    else assembly = LoadFromStream(stream, pdbStream);
                }

                public void UnloadAssembly()
                {
                    Unload();
                    // assembly = null; 
                }
            }
        }
    }
}
#endif