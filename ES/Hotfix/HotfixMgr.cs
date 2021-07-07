using ES.Common.Time;
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
    /// ESHotfix热更模块管理器
    /// <para>热更程序的核心调用类，一个热更服务器可以具备多个不同Assembly</para>
    /// <para>在使用此类需要在数据层操作，建议不要在热更层操作</para>
    /// <para>如果需要主动调用热更层入口类函数，可以通过本类 agent 变量来调用</para>
    /// </summary>
    public sealed class HotfixMgr : ITimeUpdate
    {
        private static HotfixMgr _instance;
        /// <summary>
        /// 单例模式，启动要调用Load函数
        /// </summary>
        public static HotfixMgr Instance { get { if (_instance == null) _instance = new HotfixMgr(); return _instance; } }

        // internal readonly static ManualResetEventSlim ResetEvent = new(false);
        /// <summary>
        /// 代理
        /// <para>上层需要使用热更层</para>
        /// </summary>
        public dynamic Agent { get { /*ResetEvent.Wait();*/ return _agent; } }
        internal dynamic _agent;
        /// <summary>
        /// 程序集装载器对象
        /// </summary>
        private AssemblyLoader assemblyLoader;
        /// <summary>
        /// 代理引用包
        /// </summary>
        private readonly List<WeakReference<AgentRef>> agentRefs = new List<WeakReference<AgentRef>>();
        /// <summary>
        /// 是否读取
        /// </summary>
        private bool isLoading = false;
        /// <summary>
        /// 时间流
        /// </summary>
        private readonly BaseTimeFlow tf;

        internal readonly ConcurrentDictionary<Type, Type> agentTypeMap;

        /// <summary>
        /// 创建一个热更管理器
        /// </summary>
        private HotfixMgr()
        {
            agentTypeMap = new ConcurrentDictionary<Type, Type>();
            tf = BaseTimeFlow.CreateTimeFlow(this, 0); 
            tf.StartTimeFlowES(); 
        }

        /// <summary>
        /// 载入逻辑模块
        /// <para>读取不能保证所有情况都是正常的，建议用try-catch捕获异常，以确保未正确替换，还能保持旧环境继续运行</para>
        /// </summary>
        /// <param name="assemblyFileName">程序集文件名(后缀小写或不写且程序集需要在运行根目录下)</param>
        /// <param name="classFullName">程序集下热更模块主入口类全称(即命名空间和类名)</param>
        /// <returns>加载是否成功</returns>
        public bool Load(string assemblyFileName, string classFullName)
        {
            if (isLoading) return false;
            isLoading = true;
            // LmBinder.ResetEvent.Reset();

            // 简单的处理了一下可能携带的后缀格式
            assemblyFileName = assemblyFileName.Replace(".dll", "");
            var dllName = assemblyFileName + ".dll";
            var pdbName = assemblyFileName + ".pdb";
            // 处理开始
            FileStream fsRead = null;
            FileStream pdbRead = null;
            try
            {
                if (File.Exists(dllName))
                {
                    fsRead = new FileStream(dllName, FileMode.Open);
                    if (File.Exists(pdbName)) pdbRead = new FileStream(pdbName, FileMode.Open);
                    int fsLen = (int)fsRead.Length;
                    if (fsLen > 0)
                    {
                        // 取得临时数据
                        var tempDllAssemblyLoader = new AssemblyLoader(fsRead, pdbRead);
                        if (tempDllAssemblyLoader.IsAlive)
                        {
                            // 最终绑定对象
                            var assembly = tempDllAssemblyLoader.GetAssembly();
                            var typeInfo = assembly.GetType(classFullName);
                            if (typeInfo == null) throw new NullReferenceException("Class is not found!");
                            // 处理代理类字典
                            agentTypeMap.Clear();
                            var allTypes = assembly.GetTypes();
                            var baseAgentType = typeof(BaseAgent);
                            for (int i = 0, len = allTypes.Length; i < len; i++)
                            {
                                var type = allTypes[i];
                                if (type.IsSubclassOf(baseAgentType) && type.BaseType.IsGenericType)
                                {
                                    var agentDataType = type.BaseType.GetGenericArguments()[0];
                                    agentTypeMap.TryAdd(agentDataType, type);
                                }
                            }
                            // 创建入口实例
                            var typeInstance = tempDllAssemblyLoader.GetAssembly().CreateInstance(classFullName);
                            // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                            // var lmBinderType = dllStaticAssembly.GetType("Module.LmBinder");
                            // if (lmBinderType == null) throw new NullReferenceException("LmBinder.cs module is not found!");
                            // lmBinderType.GetField(typeInfo.Name).SetValue(lmBinderType, typeInstance);
                            // 原子锁
                            Interlocked.Exchange(ref _agent, Convert.ChangeType(typeInstance, typeInfo));
                            // 处理代理索引
                            lock (agentRefs) { 
                                for (int i = agentRefs.Count - 1; i >= 0; i--) { 
                                    if (agentRefs[i].TryGetTarget(out var agentRef)) 
                                    {
                                        agentRef.isCreated = false;
                                        if (agentRef.type != null) agentRef.CreateAgent();
                                        else agentRef._agent = null;
                                    } 
                                    else agentRefs.RemoveAt(i);
                                }
                            }
                            // 程序域转换
                            if (assemblyLoader != null && assemblyLoader.IsAlive) assemblyLoader.Unload();
                            assemblyLoader = tempDllAssemblyLoader;
                            if (fsRead != null) fsRead.Close();
                            if (pdbRead != null) pdbRead.Close();
                            // LmBinder.ResetEvent.Set();
                            isLoading = false;
                            return true;
                        }
                    }
                }
            }
            catch
            {
                if (fsRead != null) fsRead.Close();
                if (pdbRead != null) pdbRead.Close();
                // LmBinder.ResetEvent.Set();
                isLoading = false;
                throw;
            }
            return false;
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        public Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        public Version GetAssemblyVersion()
        {
            return assemblyLoader.GetAssembly().GetName().Version;
        }

        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <returns></returns>
        public Assembly GetAssembly()
        {
            return assemblyLoader.GetAssembly();
        }

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
                    if (ratio == 0) periodTime          = 1000;
                    // 缩减10倍 周期60s
                    else if (ratio >= 10) periodTime    = 60000;
                    // 缩减5倍 周期30s
                    else if (ratio >= 5) periodTime     = 30000;
                    // 缩减2倍 周期15s
                    else if (ratio >= 2) periodTime     = 15000;
                    // 前后不变 周期12s
                    else if (ratio >= 1) periodTime     = 12000;
                    // 增速1.25倍 周期10s
                    else if (ratio >= 0.8) periodTime   = 10000;
                    // 增速2倍 周期8s
                    else if (ratio >= 0.5) periodTime   = 8000;
                    // 增速5倍 周期4s
                    else if (ratio >= 0.2) periodTime   = 4000;
                    // 增速10倍 周期1s
                    else if (ratio >= 0.1) periodTime   = 1000;
                    // 其他情况也是1s
                    else periodTime = 1000;
                    // 记录最后一次数值
                    lastAgentRefCount = currentAgentRefCount;
                }
            }
            // 减去流逝的时间
            periodTime -= TimeFlow.period;
        }

        /// <summary>
        /// 更新结束
        /// </summary>
        public void UpdateEnd()
        {
        }

        internal void AddAgentRef(AgentRef agentRef)
        {
            lock (agentRefs) agentRefs.Add(new WeakReference<AgentRef>(agentRef));
        }

        /// <summary>
        /// 程序集装载器
        /// </summary>
        private class AssemblyLoader 
        {
            private AssemblyProtectContext context;

            public bool IsAlive { get; private set; } = true;

            public AssemblyLoader(Stream stream, Stream pdbStream)
            {
                context = new AssemblyProtectContext(stream, pdbStream);
            }

            public Assembly GetAssembly()
            {
                if (context != null && IsAlive) return context.assembly;
                return null;
            }

            public void Unload()
            {
                if (IsAlive)
                {
                    IsAlive = false;
                    context.UnloadAssembly();
                    context = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            /// <summary>
            /// 程序集上下文
            /// </summary>
            private class AssemblyProtectContext : AssemblyLoadContext
            {
                public Assembly assembly = null;

                public AssemblyProtectContext(Stream stream, Stream pdbStream) : base(true)
                {
                    if (pdbStream == null) assembly = LoadFromStream(stream);
                    else assembly = LoadFromStream(stream, pdbStream);
                }
                public void UnloadAssembly() { Unload(); assembly = null; }
            }
        }
    }
}
