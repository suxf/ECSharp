## 本次版本更新
### **1.13.6**
一些微小细节的优化

---
#### **1.13.5**
针对Sqlserver下工具类优化
- 数据库SqlServer助手类名大小写更正： SQL 变为 Sql, DB 变为 Db
- NoDBStorage类更名为NoSqlStorage
- 数据库SqlServer助手类中增加直接创建CreateDataEntityRows、CreateConfigLoader、CreateNoSqlStorage、CreateSqlBuilder的函数
- DataAgentRows，DataAgentRow，DataAgentObject更名为DataEntiyRows，DataEntityRow，DataEntity

---
#### **1.13.4**
针对上个版本增加的内容稍作修改
- 将 CopyAgentValue 特性更名为 KeepAgentValue
- 特性 NotCreateAgent 因为识别需要反射来判断，为了直接避免反射使用的手动创建，这里给AgentData增加了一个带有一个参数选择构造函数，来选择是否自动创建

---
#### **1.13.3**
本次版本主要强化了Hotfix模块：
没有任何接口改动，可以从上一个版本直接升级。
进一步强化热更模块功能，之前需要从GetAgent函数才能创建和获取代理，现在已经通过反射完成自动化创建，且每次重载也会完成更新
在代理数据构造调用时，就会主动创建与之对应的代理，当然因为反射影响性能，也增加了一个手动创建特性 NotCreateAgent
通过给 AgentData 类增加这个特性即可取消自动创建代理，之后可以自己手动创建。
另外，现在也支持如果在热更层新建了变量，想重载后保存这些字段和属性，可以在代理数据的继承类增加 CopyAgentValue 特性即可完成！

---
#### **1.13.2**
本次主要修改Http方面的一些内容:

- 重构了HttpService类，以前用的是HttpListener来构建的，但因为此类封装太高级，导致SSL无法直接封装使用，为了坚持框架易用的初心，现在用TcpListener重构此类，实现可以通过证书直接可以支持HTTPS模式。
- 增加了除GET/POST以外的其他几种请求方式，现在也支持通过HttpService来写符合RESTful规范的请求了。
- 支持用0.0.0.0的地址写法来全局监听
- 修改了一些描述错误注释
- 支持https的ssl访问

另外，继续重新正规一下以前写框架未注意的变量命名细节，并且命名规则遵循C#指导写法。主要是属性的命名未首字母大写的问题。

---
#### **1.13.1**
为了以后更好更新维护和开发新功能，本次小版本把以往整个主工程类、接口命名不符合命名规则的重新命名

注：可能会造成某些已有代码出现丢失问题，需要重新修改引用一下，但还是需要整改一下，毕竟长痛不如短痛。。。

下面是改动的内容：

- 去除了LogConfig.cs类，将配置的相关变量移入Log.cs中，这样就不用找配置在哪了，每次使用Log的时候都可以看到这些配置
- RedisEventListener.cs => IRedisEvent.cs
- ConfigLoaderItem.cs => ConfigItem.cs
- IHttpVisitorException.cs => IHttpVisitor.cs
- HttpInvoke.cs => IHttpInvoke.cs
- IHyperSocketClientListener.cs => IHyperSocketClient.cs
- IHyperSocketServerListener.cs => IHyperSocketServer.cs
- 移除 IRemoteSocketVisitorException.cs
- SocketStatusListener.cs => IServerSocket.cs
- 移除 ISocketVisitorException.cs
- IWebsocketInvoke.cs => IWebsocket.cs
- IHttpInvoke.cs => IHttp.cs
- IRemoteSocketInvoke.cs => IRemoteSocket.cs
- ISocketInvoke.cs => ISocket.cs

以上相关接口命名变动，且接口函数也会有细微变化。

另外，移除了Socket模块的访问器，很早之前就已经弃用了，这个版本顺便一起移除。

---
#### **1.13.0**
本版本增加了一个重要功能更新：热更补丁功能
通过Assembly隔离模式，进行逻辑的增加与更新，实现服务器不重启即可更新代码的功能
因为程序集特性，无法在热更层声明新变量，热更层主要是服务于新的逻辑增加以及修补逻辑BUG
另外可惜的是支持这个功能需要.net core3.0及以上或.net5.0及以上，加上.net core3.0发布与.net core3.1发布间隔短，且微软不再对3.0维护
所以此版本将原有的.net core3.0及以下以及.net framework全系列支持取消。
所以不需要热更版本的可以继续使用1.12.x版本，后续1.12.x版本存在问题也会进行更新维护，但不会再1.12.x版本中增加新功能了。

---
#### **1.12.0**
重新设计了TimeFlow的使用方式
现在TimeFlow不再需要被继承才可以使用，转而使用ITimeUpdate接口来完成
这样可以给原来类型可以有更多的拓展
另外对TimeFlow的大部分函数进行了缩减写法，升级到此版本需要注意和TimeFlow相关的内容需要变更一下

---
#### **1.11.7**
bug fixes

---
#### **1.11.6**
bug fixes

---
#### **1.11.5**
bug fixes

---
#### **1.11.4**
bug fixes

---
#### **1.11.2**
bug fixes

---
#### **1.11.1**
优化了一些内容

---
#### **1.11.0**
- 因为HyperSocket认证加入的UTC协同时间认证导致时间不一致的设备存在连接认证失败问题，现在移除这个认证规则，所以更新此框架的版本如果使用了这个类就需要全部都更新到这个类。
- 版本号精简，去除末尾Build号
版本1.10.8.535后续如果发现问题也将在后续进行LTS。

---
#### **1.10.8.535**
- 支持donet5.0框架
- 优化以及修复一些问题

---
#### **1.10.7.533**
bug fixes

---
#### **1.10.7.532**
增加了一个SqlServer获取数据库时间的对象

---
#### **1.10.6.530**
bug fixes

---
#### **1.10.6.527**
优化了时间流内部功能。

---
#### **1.10.5.526**
- 在全新的网络通信框架Hypersocket中增加了SSL安全传输模式，现在可以更加安全的接受发送信息而不需要担心被截获的风险了。
- 优化了一些架构。
- 思考了一下，开源协议决定从GPL-3.0迁入MIT协议，拥抱MIT！

---
#### **1.10.4.514**
bug fixes

---
#### **1.10.3.509**
bug fixes

---
#### **1.10.2.504**
bug fixes

---
#### **1.10.2.503**
- 优化大量细节
- 增加全新双协议模式网络功能HyperSocket！可以用简单的代码建立TCP与UDP同时连接的对象，其中UDP采用KCP算法保证传输的可靠性。

---
#### **1.10.0.460**
- 优化Socket中TCP协议封装包体类，现在包壳本身大小只有8~10Byte，原来是16Byte，在数据长度小于65535的情况下，包壳大小比原先小一倍。
- 网络套接字模块UDP部分重构另外分离C/S重合部分。
- 新增UDPSocket，原套接字模块UDP模式可以继续使用，新增部分是基于原有更高级的封装，完成可靠的UDP模式。