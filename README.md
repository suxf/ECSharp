# EasySharpFrame
[![Nuget](https://img.shields.io/nuget/v/EasySharpFrame?style=float)](https://www.nuget.org/packages/EasySharpFrame)
[![Nuget](https://img.shields.io/nuget/dt/EasySharpFrame?style=float)](https://www.nuget.org/stats/packages/EasySharpFrame?groupby=Version)
[![Platform](https://img.shields.io/badge/framework-.net-blueviolet?style=float)](https://dotnet.microsoft.com/download/dotnet)
[![Platform](https://img.shields.io/badge/platform-win|linux|osx-lightgrey?style=float)](https://dotnet.microsoft.com/download/dotnet)
[![GitHub](https://img.shields.io/github/license/suxf/EasySharpFrame?style=float)](https://github.com/suxf/EasySharpFrame/blob/master/LICENSE)

Easy .NET Develop Frame.

这是一个基于.Net语言框架而设计的开发框架。集成了许多常规用到的开发功能，主要目的是利于基于此框架的服务器快捷开发！

# 快速使用
可以直接从NuGet库中搜索安装[最新版本](https://www.nuget.org/packages/EasySharpFrame)。

# 版本支持
| 版本 | .Net6.0 | .Net5.0 | .NetCore3.1 | .NetFramework4.6.2 | .NetStandard2.0 |
| :-: | :-----: | :-----: |:----------: | :----------------: | :------------: |
| 1.17.0+ | √ | √ | √ | √ | √ |
| 1.13.0 - 1.16.8 | √ | √ | √ | × | × |
| 1.11.x - 1.12.0 | √ | √ | √ | √ | × |
| 1.9.x - 1.10.x | × | × | √ | √ | × |
| 1.7.x - 1.8.x  | × | × | √ | × | × |

### 更新历史 [查看](https://github.com/suxf/EasySharpFrame/blob/master/UPDATE.md)

# 目录介绍
| 目录 | 备注 |
| ------------ | ------------ |
| UnityESF | Unity框架支持 |
| docs | [在线API文档](https://suxf.github.io/EasySharpFrame) |
| ES | 框架主工程 |
| Sample | 框架测试样本工程 |
| SampleDll | 框架热更模块测试样本工程 |

# 功能说明
### 1.HTTP服务
对TcpListener进行封装，支持ssl模式，可以完成高并发任务。以后可能会更新静态文件的访问，即网页访问。
```csharp
/* 使用方法一 */
public void Http1()
{
    // 先继承http异常接口，这里把测试的访问函数也写在一个类中，实际不需要
    HttpHandle1 handle = new HttpHandle1();
    // 建立http访问器，并载入异常接口类
    HttpVisitor visitor = new HttpVisitor(handle);
    // 建立http服务，填写地址并且赋予访问器
    // X509Certificate2 certificate = new X509Certificate2("https.pfx", "8888");
    // HttpService service = new HttpService("127.0.0.1", 8080, visitor, certificate);
    HttpService service = new HttpService("127.0.0.1", 8080, visitor);
    // 给访问器增加函数
    visitor.Add("", handle.Index);
    visitor.Add("Hello", handle.Hello);
    // 启动服务
    service.StartServer();
    // 然后就可以通过浏览器或其他请求工具来访问了
    // 访问地址： http://127.0.0.1:8080/Hello?text=World

    Console.ReadLine();
}

class HttpHandle1 : IHttpVisitor
{
    public void Index(HttpRequest request, HttpResponse response)
    {
        // 首页根访问
        if (!request.GetParams.TryGetValue("text", out var text)) text = "text no content";
        response.Write("Index:" + text);
    }
    
    public void Hello(HttpRequest request, HttpResponse response)
    {
        response.Write("Hello World:" + request.PostValue);
    }

    public void HttpVisitorException(HttpRequest request, Exception ex)
    {
        // http异常处理
    }
}
```
```csharp
/* 使用方法二 */
public void Http2()
{
    // 先继承http异常接口，这里把测试的访问函数也写在一个类中，实际不需要
    HttpHandle2 handle = new HttpHandle2();
    // 建立http服务，填写地址
    HttpService service = new HttpService("127.0.0.1", 8080, handle);
    // 启动服务
    service.StartServer();
    // 然后就可以通过浏览器或其他请求工具来访问了
    // 访问地址： http://127.0.0.1:8080/Hello?text=World

    Console.ReadLine();
}

class HttpHandle2 : IHttp
{
    public void HttpException(HttpRequest request, Exception exception)
    {
        // http异常处理
    }

    public void OnRequest(HttpRequest request, HttpResponse response)
    {
        // 这里是全部消息回调接口
        // 所以如果需要高度自定义可以使用此方法
    }
}
```
### 2.Websocket服务
使用Fleck库中的Websocket进行二次封装，支持ssl模式。
```csharp
WebsocketService service = new WebsocketService("ws://127.0.0.1:8081", new WebsocketHandle());

class WebsocketHandle : IWebsocket
{
    public void OnBinary(RemoteConnection conn)
    {
        // 数据使用byte流传输调用
    }

    public void OnClose(RemoteConnection conn)
    {
        // 关闭时调用
    }

    public void OnError(RemoteConnection conn, Exception exception)
    {
        // 出现异常调用
    }

    public void OnMessage(RemoteConnection conn)
    {
        // 数据使用文本时调用
    }

    public void OnOpen(RemoteConnection conn)
    {
        // 新连接产生调用
    }
}

```
### 3.HyperSocket<自定义Socket服务>
该模块已经深度封装了原生Socket模块，实现了快捷连接，加密连接等比较便捷实用的功能，通过IOCP接口可以实现高并发收发。需要配合配套的客户端才能使用。
```csharp
// 创建服务器
var config = new HyperSocketConfig() { UseSSL = true };
new HyperSocketServer("127.0.0.1", 8888, 500, new ServerListener(), config).StartServer();

// 服务器监听接口
class ServerListener : IHyperSocketServer
{
    public void OnClose(RemoteHyperSocket socket)
    {
        // 客户端关闭
    }

    public void SocketError(RemoteHyperSocket socket, Exception ex)
    {
        // 连接异常
    }

    public void OnOpen(RemoteHyperSocket socket)
    {
        // 客户端新连接
    }

    public void OnTcpReceive(byte[] data, RemoteHyperSocket socket)
    {
        // Tcp模式接收
    }

    public void OnUdpReceive(byte[] data, RemoteHyperSocket socket)
    {
        // Udp模式接收
    }
}
```
```csharp
// 创建客户端
new HyperSocket("127.0.0.1", 8888, new ClientListener()).Connect();

// 客户端监听接口
class ClientListener : IHyperSocketClient
{
    public void SocketError(HyperSocket socket, Exception ex)
    {
        // 客户端异常
    }

    public void OnOpen(HyperSocket socket)
    {
        // 客户端连接成功
    }

    public void OnTcpReceive(byte[] data, HyperSocket socket)
    {
        // Tcp模式接收
    }

    public void OnUdpReceive(byte[] data, HyperSocket socket)
    {
        // Udp模式接收
    }
}
```
### 4.TimeFlow<时间流>
该模块深度封装了原生Thread模块，可以快捷给每个类增加一个时间更新，类似Unity中组件的Update功能，模块以固定周期的速度进行刷新，并且经过多个项目及测试，在周期时间内最终循环时间很精准。另外 TimeCaller 是支持快速定制一个Scheduler定时器的功能类;TimeClock是一种可自定义现实时间的闹钟定时器功能。
```csharp
class TimeDemo : ITimeUpdate
{
    TimeFlow tf;
    public TimeDemo(){
        // 时间流
        tf = TimeFlow.Create(this);
        tf.Start();

        // 时间闹钟
        TimeClock.Create(delegate(DateTime time) {
            Log.Info($"Time Now Alarm Clock:{time}"); 
        }, "00:00:00").Start(true);

        // 时间执行器
        TimeCaller.Create(static delegate { 
            Log.Info("Hello TimeCaller"); 
        }, 2000, 1000, 3).Start();
    }
    
    // 可以从 TimeFlow.period 直接获取周期时间
    // dt为消耗时间的差值 因为程序不可能每次都精准执行
    // 所以update会不断调整时间执行时间 dt就是这个时间的差值
    // 一般情况下不需要管理，因为在总时间循环中 几乎可以忽略 因为我们有自动修正
    public void Update(int dt)
    {
    }
    
    // 停止更新
    public void UpdateEnd()
    {
    }
}
```
### 5.Sqlserver数据库助手
Sqlserver相关操作比较多，更多可直接查看Sample中书写的样例：[查看链接](https://github.com/suxf/EasySharpFrame/blob/master/Sample/Test_DBSqlServer.cs)

助手目前有以下几种功能：
- 数据库连接：简化连接操作步骤
- 数据库执行SQL和存储过程：书写SQL直接执行得到结果
- SQL语句构建器：函数化某些SQL关键字，通过函数连调实现执行SQL
- SQLServer基础配置加载器：可以通过映射关系加载数据库表中的配置，高并发读取
- Sqlserver数据缓存助理数据组：可以通过映射关系加载数据库表中的数据，高并发实现增删改查
- 非关系型存储：通过映射原理建立表中Key-Value模型的对象，实现高并发读写

```csharp
// 数据库连接使用此函数即可简单创建 数据库的创建还提供更多重载方案，可以点入查看
SqlServerDbHelper dbHelper = new SqlServerDbHelper("127.0.0.1", "sa", "123456", "db_test");
// 增加异常监听器 需要一个继承 ISQLServerDBHelper 接口
dbHelper.SetExceptionListener(this);
// 检测数据库连接是否成功调用 成功返回true
if (dbHelper.CheckConnected())
{
    Console.WriteLine("数据库已连接");
}
//获取数据库时间 如果获取不到默认获取程序本地时间
Log.Info("数据库时间:" + dbHelper.Now);
// 普通查询调用
var result = dbHelper.CommandSQL("SELECT * FROM tb_test");
// 查询条数判断
if (result.EffectNum > 0)
{
    // 取出表一的相关数据
    // 如果查询有多个select 可以通过result.dataSet取得
    int id = (int)result.Collection[0]["id"];
    Console.WriteLine($"id:{id}");
}
```
### 6.Mysql数据助手
Mysql数据助手和Sqlserver数据库助手使用操作差不多，可参考第5项。
```csharp
// 数据库连接使用此函数即可简单创建 数据库的创建还提供更多重载方案，可以点入查看
MySqlDbHelper dbHelper = new MySqlDbHelper("127.0.0.1", "root", "123456");
// 增加异常监听器
dbHelper.SetExceptionListener(this);
// 检测数据库连接是否成功调用 成功返回true
if (dbHelper.CheckConnected())
{
    Log.Info("数据库已连接");
}
//获取数据库时间 如果获取不到默认获取程序本地时间
Log.Info("数据库时间:" + dbHelper.Now);
```

### 7.Redis数据库助手
简化Redis连接复杂度，快速连接Redis并且对数据进行高并发读写操作，对订阅功能进行简化操作，使订阅更加易用。
```csharp
// 继承 IRedisEvent 接口来用于监听回调
RedisHelper helper = new RedisHelper("127.0.0.1:6379");
// 增加事件监听用于检测连接状态
helper.AddEventListener(this);
// 设置一个值
helper.Set("test", 1);
// 取出值
var test = helper.Get<int>("test");
```
### 8.Log功能
日志功能就是解决服务器对各种记录数据的记录和输出，日志即可输出在控制窗口，也可以写入本地文件持久化储存，供后续查看。Log类中提供日志前置配置参数，可以对日志进行自定义配置，详见 LogConfig 类。
```csharp
// 以下四个函数均为普通分级日志输出函数
Log.Debug("debug is this");
Log.Info("info is this");
Log.Warn("warn is this");
Log.Error("error is this");
// 此函数可以写在try catch中 用于打印异常问题
Log.Exception(new System.Exception(), "exception is this");
```
### 9.热更新功能
支持服务器运行中可以进行逻辑更新的功能。当然，热更的实现，在各个语言上都是通过运行时反射实现的，所以一旦利用反射原理的功能都会逊色于原生直接调用。经过多次测试在千万次的简单循环下，初次加载可能会存在总量30~100毫秒的延迟；随之以后的调用则影响很小，循环总量和直接调用总量为2:1，也就是说在正常情况下，直接调用耗时1ms的操作，移植到热更新层也仅仅花费2ms左右，所以非密集型计算，耗时偏差基本可以忽略不计。

```csharp
/** 主工程项目 **/
class Test_Hotfix
{
    public Test_Hotfix()
    {
        while (true)
        {
            // 普通测试
            TestHotfix();
            // 耗时测试
            // ConsumeTime();

            Console.WriteLine($"Is First Load:{HotfixMgr.IsFirstLoad}");

            // 回车重载测试
            Console.ReadLine();
            Console.Clear();
        }
    }

    // 测试只需要放入构造函数
    // 热更测试
    public void TestHotfix()
    {
        HotfixMgr.Load("SampleDll", "SampleDll.Main", new string[] { "Hello World" }, "Main_Test");
    }

    // 测试只需要放入构造函数
    // 耗时测试
    public void ConsumeTime()
    {
        HotfixMgr.Instance.Load("SampleDll", "SampleDll.Main", null, "Main_Test1");
        Player player = new Player();
        Stopwatch watch = new Stopwatch();
        /* 性能测试 */
        // 第一次直接调用
        Console.WriteLine("第一次直接调用开始~");
        watch.Reset();
        watch.Start();
        player.Test();
        watch.Stop();
        Console.WriteLine($"第一次直接调用耗时1:{watch.Elapsed.TotalMilliseconds}ms");
        // 第一次实测热更调用
        Console.WriteLine("\n\n热更调用开始~");
        watch.Reset();
        watch.Start();
        player.GetDynamicAgent().Test();
        watch.Stop();
        Console.WriteLine($"第一次热更层耗时1:{watch.Elapsed.TotalMilliseconds}ms");
        // 第二次直接调用
        Console.WriteLine("\n\n第二次直接调用开始~");
        watch.Reset();
        watch.Start();
        player.Test();
        watch.Stop();
        Console.WriteLine($"第二次直接调用耗时2:{watch.Elapsed.TotalMilliseconds}ms");
        // 第二次实测热更调用
        Console.WriteLine("\n\n热更调用开始~");
        watch.Reset();
        watch.Start();
        player.GetDynamicAgent().Test();
        watch.Stop();
        Console.WriteLine($"第二次热更层耗时2:{watch.Elapsed.TotalMilliseconds}ms");
    }
}

// 手动创建对应的代理
// 如果每次热更重载后不主动创建 则代理不会运作
// 也可以通过带参数构造函数来设定手动
// [NotCreateAgent]
public class Player : AgentData
{
    public int count;
   
    // 用于测试 实际上一般数据层不写逻辑
    public void Test()
    {
        for (int i = 0; i < 10000000; i++) count++;
        Console.WriteLine("直接调用计数:" + count);
    }
}

// 自动创建代理
// 并且添加 KeepAgentValue 特性实现代理内变量保存
// 如果去除 KeepAgentValue 特性则变量不会在重载后保存
[KeepAgentValue]
public class Player1 : AgentData
{
    public int count;

    public string test;

    public Player1()
    {
        test = "Hello World";
        // 手动创建代理
        CreateAgent();
    }
}
```
```csharp
/** 热更工程项目 **/
// 热更测试DLL入口
public class Main
{
    static readonly Player player = AgentDataPivot.AddOrGetObject<Player>("player");
    static Player1 player1;

    static StructValue<int> test_1 = AgentDataPivot.AddOrGetStruct("test_1", 0);

    static B b;
    static C c;
    public static void Main_Test(string[] args)
    {
        Console.WriteLine($"Input args:{args[0]}, test_1:{test_1.Value++}");

        player1 = AgentDataPivot.AddOrGetObject<Player1>("player1");
        b = new B();
        c = new C();
        Test2(b, c);
    }

    public static void Main_Test1(string[] args)
    {
        Stopwatch watch = new Stopwatch();
        // 可以利用拓展特性来实现不每次都书写泛型实现代理
        // player.GetAgent<PlayerAgent>().Test();
        // player.GetAgent().Test();

        watch.Reset();
        watch.Start();
        player.GetAgent().Test();
        watch.Stop();
        Console.WriteLine($"内部第一次热更层耗时3:{watch.Elapsed.TotalMilliseconds}ms\n");
        watch.Reset();
        watch.Start();
        player.GetAgent().Test();
        watch.Stop();
        Console.WriteLine($"内部第二次热更层耗时3:{watch.Elapsed.TotalMilliseconds}ms\n\n");
    }

    public static void Test2(A obj1, A obj2)
    {
        obj1.GetAbstractAgent<A_Agent>().WriteHelloA();
        obj2.GetAbstractAgent<A_Agent>().WriteHelloA();
        obj1.GetAbstractAgent<A_Agent>().Hello();
        obj2.GetAbstractAgent<A_Agent>().Hello();
        obj1.GetAgent<B_Agent>().Hello();
        obj2.GetAgent<C_Agent>().Hello();
    }
}

// 如果觉得每次调用都需要使用GetAgent的泛型来处理
// 那么可以针对需要大量调用的代理，在热更层写一个静态拓展来实现不用再写代理泛型的重复工作
public static class AgentRegister 
{
    // PlayerAgent代理
    // 这样只需要在这里写一次，以后就可以直接借助GetAgent()函数直接使用了
    public static PlayerAgent GetAgent(this Player self) => self.GetAgent<PlayerAgent>();
}

// 如果需要时间流需要在热更层继承和使用
// 测试案例一
public class PlayerAgent : Agent<Player>, ITimeUpdate
{
public TimeFlow tf;
protected override void Initialize()
{
    // tf = TimeFlow.Create(this);
    // tf.Start();
}
public void Test()
{
    // Console.WriteLine(self.name);
    Stopwatch watch = new Stopwatch();
    /* 性能测试 */
    // 第一次直接调用
    watch.Start();
    for (int i = 0; i < 10000000; i++) { self.count++; }
    watch.Stop();
    Console.WriteLine($"热更层循环耗时:{watch.Elapsed.TotalMilliseconds}ms");
    // for (int i = 0; i < 1000000; i++) self.count++;
    Console.WriteLine("热更层计数:" + self.count);
}
int count = 0;
public void Update(int deltaTime)
{
    if (count % 1000 == 0) Console.WriteLine($"player count:{self.count++},copyCount:{count}");
    count += deltaTime;
}
public void UpdateEnd()
{
}

// 测试案例二 主动创建 且保留值
public class Player1Agent : Agent<Player1>, ITimeUpdate
{
public int copyCount = 0;
private readonly int seed = new Random().Next(9999);
protected override void Initialize()
{
    // 两种相同作用
    Console.WriteLine("IsFirstCreateAgent:" + self.IsFirstCreateAgent);
    Console.WriteLine("IsFirstCreate:" + IsFirstCreate);
    // 先处理代理数据构造函数，在处理代理构造
    Console.WriteLine(self.test);
    TimeFlow.Create(this).Start();
}
public void Update(int deltaTime)
{
    if (copyCount % 1000 == 0) Console.WriteLine($"player1 count:{self.count++},copyCount:{copyCount},seed:{seed}");
    copyCount += deltaTime;
}
public void UpdateEnd()
{
}

// 测试案例三 继承测试
// A抽象代理实现
public abstract class A_Agent : AbstractAgent, IAgent<A>
{
    public A self => _self as A;
    public void WriteHelloA()
    {
        self.test1 += 100;
        self.test2 += " world A";
    }
    public abstract void Hello();
    protected override void Initialize()
    {
        self.test1 += 1000;
    }
}

// B抽象代理
public class B_Agent : A_Agent, IAgent<B>
{
    public new B self => _self as B;

    public override void Hello()
    {
        self.test1 += 20;
        self.test3 += " world B";
        Console.WriteLine(self.test1 + "," + self.test2 + "," + self.test3);
    }
    protected override void Initialize()
    {
        base.Initialize();
        self.test1 += 20;
    }
}

// C抽象代理
public class C_Agent : A_Agent, IAgent<C>
{
    public new C self => _self as C;
    public override void Hello()
    {
        self.test1 += 50;
        self.test4 += " world C";
        Console.WriteLine(self.test1 + "," + self.test2 + "," + self.test4);
    }
    protected override void Initialize()
    {
        base.Initialize();
        self.test1 += 10;
    }
}
```
### 10.可变变量
以空间换更方便的数值传递操作，可变变量可以满足所有基础类型的变量存储和读取，并且配备列表、字典容器来提供批量存储，同时可以很方便的获取存储后的原始字节数组或序列化的数据，当然也可以重新反序列化成新的对象。
```csharp
Var a1 = (byte)1;
Var a2 = (sbyte)-2;
Var a3 = (ushort)3;
Var a4 = (short)-4;
Var a5 = 5U;
Var a6 = -6;
Var a7 = 7UL;
Var a8 = -8L;
Var a9 = 9.123456789F;
Var a10 = 9.123456789987654321D;
Var a11 = true;
Var a12 = "hello world";
Var a13 = TestEnum.B;
Var a14 = new Var(new object());
VarList list = new VarList();
list.Add(a1);
list.Add(a2);
list.Add(a3);
VarMap map = new VarMap();
map.Add("a1", a1);
map.Add("a2", a2);
map.Add("a3", a3);
map.Add("list", list);
Var a15 = list;
Var a16 = map;
Var b1 = a1 > a2;
Var b2 = a3 == a4;
Var b3 = a6 % a3;
Var b4 = a8 * a7;
Var b5 = a9.ToString();
Var b6 = a10.GetBytes();
```
### 11.事件与命令
本模块主要负责特定事件的函数触发，使用泛型自定义多种参数的事件或者命令，来在特定的场合和逻辑中触发相关函数调用。其中，命令模式支持同步等待功能，可以支持异步事件转同步执行。
```csharp
// 新建事件
Event<int> event1 = new Event<int>();
// 添加事件
event1.Add(1, static () => { /* 执行事件回调 */ });
// 触发事件
event1.Call(1);

// 新建多级事件
MultiEvent<string, int> event2 = new MultiEvent<string, int>();
// 添加事件
event2.Add("test", 1, static () => { /* 执行事件回调 */ });
// 触发事件
event2.Call("test", 1);

// 新建命令
Command<string, string> command = new Command<string, string>();
// 添加命令
command.Add("test1", static (object obj) => { return ""; });
command.Add("test2", static (object obj) => { return "Hello World"; });
// 触发事件
command.Call("test1");
// 创建等待id
int waitId = command.AutoWaitID;
// 异步触发事件
command.Call("test2", waitId);
// 添加等待命令 此处执行被阻塞 可以在其他线程中被触发后继续执行
string str = command.WaitCall("test2", waitId);
```
### 12.其他
其他小功能不再过多介绍，可以在使用的过程中慢慢查询API来获取使用细节。

```csharp
// 读取配置 默认和读取程序名一样配置文件 比如此程序生成为Sample.exe那么读取对应的是Sample.exe.config
// 一般来说使用vs2019开发 只需要在新建一个和程序集名称一模一样的.config配置文件即可
// 注意此函数不支持读取其他文件 此demo已经创建了配置文件详见项目 Sample.config
// 本类设计初只能读取两层 具体结构可以参照样例
// 此处读取第一层配置数据
string test = AppConfig.Read("test");
int test1 = AppConfig.Read<int>("test1");
bool test2 = AppConfig.Read<bool>("test2");
// 此处读取第二层配置数据
string test2 = AppConfig.Read("testgroup", "test2");
float tests3 = AppConfig.Read<float>("testgroup", "test3");

// 通过ini文件读取配置
Ini.LoadParser("config.ini", true);
Log.Info($"config filename name:{Ini.Current.GetValue("filename")}");

// 获取有效字节
// 此判定依据是在某索引位为0开始 往后4位皆为0 则认为后续数据无效实现
// 所以这里的设定还是要看具体情况来 不一定适用所有情况
// ByteHelper.GetValidLength 则是直接获取长度大小 而非返回数据
byte[] bytes = ByteHelper.GetValidByte(new byte[] { 1, 2, 3, 4, 0, 0, 0 });

// 随机生成指定位数的字符串
// 字符串将有数字与大小写字母组成
string code = RandomCode.Generate(32);

// md5的封装
string md5Str = MD5.Encrypt("helloworld");

// 获取此框架的版本信息
string versionStr = ES.Utils.SystemInfo.FrameVersion;
// 逻辑线程数
int processorCount = ES.Utils.SystemInfo.ProcessorCount;
```

# 引用声明
本框架所有引用第三方外部工具均为MIT协议且均采用NuGet库自动安装

1. [statianzo/Fleck](https://github.com/statianzo/Fleck) 支持WebSocket连接库
1. [KumoKyaku/KCP](https://github.com/KumoKyaku/KCP) UDP高速传输算法协议
1. [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) 通用强大的Json格式化工具
1. [StackExchange/StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) Redis数据库支持工具
1. [dotnet/corefx](https://github.com/dotnet/corefx) 微软官方支持的SqlServer支持库
1. [mysql-net/MySqlConnector](https://github.com/mysql-net/MySqlConnector) 高性能的MySql数据库连接支持库

# 关于框架
框架的雏形是工作和业余开发过程中遇到的一些问题，慢慢汇聚出来零零散散的工具类形成的，在经过一段时间整理重构和后续维护，才有了现在这个版本，在此之前使用此框架的项目也一直在线上跑着，总的来说是为了开发更加快速便捷才构建了这个框架。框架迭代的技术都是以新技术为主，优先使用官方推荐的解决方案。

如果对这个项目或者项目里用的一些技术比较感兴趣的朋友，希望能够给颗⭐支持一下~😊

最后，谢谢所有能够看到这里的和所有打⭐的朋友。💕💕💕