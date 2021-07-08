# EasySharpFrame
[![Nuget](https://img.shields.io/nuget/v/EasySharpFrame?style=float)](https://www.nuget.org/packages/EasySharpFrame)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/suxf/EasySharpFrame?color=9cf&style=float)](https://github.com/suxf/EasySharpFrame/releases)
[![Nuget](https://img.shields.io/nuget/dt/EasySharpFrame?style=float)](https://www.nuget.org/stats/packages/EasySharpFrame?groupby=Version)
[![Platform](https://img.shields.io/badge/.netcore-3.1-blueviolet?style=float)](https://dotnet.microsoft.com/download/dotnet)
[![Platform](https://img.shields.io/badge/.net->=5.0-blueviolet?style=float)](https://dotnet.microsoft.com/download/dotnet)
[![GitHub](https://img.shields.io/github/license/suxf/EasySharpFrame?style=float)](https://github.com/suxf/EasySharpFrame/blob/master/LICENSE)

Easy .NET Develop Frame.

这是一个基于.Net语言框架而设计的开发框架。集成了许多常规用到的开发功能，主要目的是利于基于此框架的服务器快捷开发！

# 快速使用
可以直接从NuGet库中搜索安装[最新版本](https://www.nuget.org/packages/EasySharpFrame)。

# 框架版本
| 版本 | 支持 |
| ------------ | ------------ |
| .Net 6.0 | √  |
| .Net 5.0 | √  |
| .Net Core 3.1 | √ |
| .Net Framework 4.8 | × |

注：从1.13.x版本后仅支持框架版本情况如上，主要是因为热更功能所需的框架功能在老版本中没有支持，在1.12.x及之前的版本基本上全框架版本都可以使用，不需要热更功能可以使用1.12.x版本。

### 更新历史 [查看](https://github.com/suxf/EasySharpFrame/blob/master/UPDATE.md)

# 目录介绍
| 目录 | 备注 |
| ------------ | ------------ |
| Client | 客户端框架支持(目前只更新了Unity版本)  |
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
HyperSocket.CreateServer("127.0.0.1", 8888, 500, new ServerListener(), config);

// 服务器监听接口
class ServerListener : IHyperSocketServer
{
    public void OnClose(RemoteHyperSocket socket)
    {
        // 客户端关闭
    }

    public void SocketError(Exception ex)
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
HyperSocket.CreateClient("127.0.0.1", 8888, new ClientListener());

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
该模块深度封装了原生Thread模块，可以快捷给每个类增加一个时间更新，类似Unity中组件的Update功能，模块以固定10ms的速度进行刷新，并且经过多个项目及测试，在周期时间内最终循环时间很精准。另外 TimeCaller 是支持快速定制一个Scheduler定时器的功能类。
```csharp
class TimeDemo : ITimeUpdate
{
    TimeFlow tf;
    public TimeDemo(){
        tf = TimeFlow.Create(this);
        tf.Start();
    }
    
    // 此函数会默认10毫秒调用一次
    // 可以从 TimeFlow.period 直接获取周期时间
    // dt为消耗时间的差值 因为程序不可能每次都精准10毫秒执行
    // 所以update会不断调整时间执行时间 dt就是这个时间的差值
    // 一般情况下不需要管理，因为在总时间循环中 几乎可以忽略 因    为我们有自动修正
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
dbHelper = new SQLServerDBHelper("127.0.0.1", "sa", "123456", "db_test");
// 增加异常监听器 需要一个继承 ISQLServerDBHelper 接口
dbHelper.SetExceptionListener(this);
// 检测数据库连接是否成功调用 成功返回true
if (dbHelper.CheckConnected())
{
    Console.WriteLine("数据库已连接");
}
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
以后如果用的人多了，再补充吧~

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
日志功能就是解决服务器对各种记录数据的记录和输出，日志即可输出在控制窗口，也可以写入本地文件持久化储存，供后续查看。Log类中提供日志前置配置参数，可以对日志进行自定义配置。
```csharp
// 以下四个函数均为普通分级日志输出函数
Log.Debug("debug is this");
Log.Info("info is this");
Log.Warn("warn is this");
Log.Error("error is this");
// 此函数可以写在try catch中 用于打印异常问题
Log.Exception(new System.Exception(), "exception is this");
```
```csharp
/** 可配置变量 **/
/** 配置修改建议在第一次调用Log前修改完成，避免出现奇怪的问题 **/
// 日志控制台输出开关 默认开启
Log.LOG_CONSOLE_OUTPUT = true;
// 日志写入周期 单位 ms
Log.LOG_PERIOD = 1000;
// 日志写入文件后缀
Log.LOG_FILE_SUFFIX = ".log";
// 日志单个文件最多大小
// 单位 byte 默认 50MB大小
Log.LOG_UNIT_FILE_MAX_SIZE = 52428800;
// 日志根路径
Log.LOG_PATH = "./log/";
```
### 9.热更功能
是的，没错！这个功能就是支持服务器运行中可以进行逻辑更新的功能。当然，众所周知这种非解释性脚本的热更实现，在各个语言上都是通过运行时反射实现的，所以一旦和反射搭上边的功能都会逊色于原生直接调用。但是！！！经过多次测试，在百万次的简单循环下，初次调用可能会存在总量100毫秒的延迟；随之以后的调用则影响很小，循环总量在30毫秒左右，偏差基本可以忽略不计。

```csharp
/** 主工程项目 **/
class Test_Hotfix
{
    // 实际创建都需要先完成热更模块读取完成后执行
    Player player = new Player();
    Player1 player1;
    public Test_Hotfix()
    {
        TestHotfix();
    }

    // 测试只需要放入构造函数
    // 热更测试
    public void TestHotfix()
    {
        while (true)
        {
            HotfixMgr.Instance.Load("SampleDll", "SampleDll.Main");
            HotfixMgr.Instance.Agent.Test();
            if(player1 == null) player1 = new Player1();
            Console.ReadLine();
            Console.Clear();
        }
    }
}

// 手动创建对应的代理
// 如果每次热更重载后不主动创建 则代理不会运作
// 也可以通过带参数构造函数来设定手动
[NotCreateAgent]
public class Player : AgentData
{
    public int count;

    // 通过base(false)设置手动创建
    // 这样就不用通过 NotCreateAgent 特性来判断 二者选其一即可
    public Player() : base(false) { }
}

// 自动创建代理
// 并且添加 KeepAgentValue 特性实现代理内变量保存
// 如果去除 KeepAgentValue 特性则变量不会在重载后保存
[KeepAgentValue]
public class Player1 : AgentData
{
    public int count;
}
```
```csharp
/** 热更工程项目 **/
// 热更测试DLL入口
public class Main
{
    readonly Player player = AgentDataPivot.AddOrGetObject<Player>("player");
    public void Test()
    {
        player.GetAgent<PlayerAgent>().Test();
    }
}

// 如果需要时间流需要在热更层继承和使用
// 测试案例一 非主动创建
public class PlayerAgent : Agent<Player>, ITimeUpdate
{
    public TimeFlow tf;

    public PlayerAgent()
    {
        tf = TimeFlow.Create(this);
        tf.Start();
    }

    public void Test()
    {
        Console.WriteLine("Hello:" + self.count);
    }

    int count = 0;
    public void Update(int deltaTime)
    {
        if (count % 1000 == 0) Console.WriteLine($"player count:{self.count++},copyCount:{count}");
        count += TimeFlow.period;
    }

    public void UpdateEnd()
    {
    }
}

// 测试案例二 主动创建 且保留值
public class Player1Agent : Agent<Player1>, ITimeUpdate
{
    public int copyCount = 0;

    public Player1Agent()
    {
        TimeFlow.Create(this).Start();
    }

    public void Update(int deltaTime)
    {
        if (copyCount % 1000 == 0) Console.WriteLine($"player1 count:{self.count++},copyCount:{copyCount}");
        copyCount += TimeFlow.period;
    }

    public void UpdateEnd()
    {
    }
}
```
### 10.其他
其他小功能不再过多介绍，可以在使用的过程中慢慢查询API来获取使用细节。

```csharp
// 读取配置 默认和读取程序名一样配置文件 比如此程序生成为Sample.exe那么读取对应的是Sample.exe.config
// 一般来说使用vs2019开发 只需要在新建一个和程序集名称一模一样的.config配置文件即可
// 注意此函数不支持读取其他文件 此demo已经创建了配置文件详见项目 Sample.config
// 本类设计初只能读取两层 具体结构可以参照样例
// 此处读取第一层配置数据
string test = AppConfig.Read("test");
// 此处读取第二层配置数据
string test2 = AppConfig.Read("testgroup", "test2");

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
string versionStr = ES.Common.Utils.Version.ToString();
```

# 引用声明
本框架所有引用第三方外部工具均为MIT协议且均采用NuGet库自动安装

1. [statianzo/Fleck](https://github.com/statianzo/Fleck) 支持WebSocket连接库
1. [KumoKyaku/KCP](https://github.com/KumoKyaku/KCP) UDP高速传输算法协议
1. [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) 通用强大的Json格式化工具
1. [StackExchange/StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) Redis数据库支持工具
1. [dotnet/corefx](https://github.com/dotnet/corefx) 微软官方支持的SqlServer支持库

# 关于框架
最初是由工作和业余开发过程中遇到的问题慢慢汇聚出来的一些零零散散的工具类，经过一段时间整理重构和后续维护，才有了现在这个版本，在此之前使用此框架的项目也一直在线上跑着，总的来说是为了开发更加快速便捷才构建了这个框架。

如果对这个项目比较感兴趣的朋友，希望能够给颗⭐支持一下~