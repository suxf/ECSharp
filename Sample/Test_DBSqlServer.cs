using ES.Database.Linq;
using ES.Database.SQLServer;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Sample
{
    /// <summary>
    /// 数据库测试类【Sqlserver专用】
    /// 此类包含数据库对象中所有内容
    /// 帮助使用框架的朋友进一步了解数据库创建和使用
    /// </summary>
    class Test_DBSqlServer : ISqlServerDbHelper
    {
        // 数据库助手对象
        SqlServerDbHelper dbHelper;
        public Test_DBSqlServer()
        {
            Log.Info("数据测试开始");
            // 数据库连接使用此函数即可简单创建 数据库的创建还提供更多重载方案，可以点入查看
            dbHelper = new SqlServerDbHelper("127.0.0.1", "sa", "123456", "db_test");
            // 增加异常监听器
            dbHelper.SetExceptionListener(this);
            // 检测数据库连接是否成功调用 成功返回true
            if (dbHelper.CheckConnected())
            {
                Log.Info("数据库已连接");
            }
            //获取数据库时间 如果获取不到默认获取程序本地时间
            Log.Info("数据库时间:" + dbHelper.Now);

            Log.Info("数据库测试结束");
        }


        /// <summary>
        /// 数据库助手使用
        /// </summary>
        public void DBHelperUseDemo()
        {
            // 检测是否数据库连接正常
            dbHelper.CheckConnected();

            // 普通查询调用
            var result = dbHelper.CommandSQL("SELECT * FROM tb_test");
            // 查询条数判断
            if (result.EffectNum > 0)
            {
                // 取出表一的相关数据
                // 如果查询有多个select 可以通过result.dataSet取得
                int id = (int)result.Rows[0]["id"];
                Log.Info($"id:{id}");
            }

            // 非查询sql调用
            var result2 = dbHelper.ExecuteSQL("UPDATE tb_test SET content = 'Hello' WHERE id = 1");
            // 返回影响记录数量
            if (result2 > 0)
            {
                Log.Info("success");
            }

            // 使用sql构建器来执行sql
            // 这种方式快捷，但是也只能应付一些简单的数据处理
            // var result5 = dbHelper.CreateBuilder().Fields("id", "userid").Where("id > 0").Select();
            var result5 = SqlBuilder.Create(dbHelper).Fields("id", "userid").Where("id > 0").Select();
            // 查询条数判断
            if (result5.EffectNum > 0)
            {
                // 取出表一的相关数据
                // 如果查询有多个select 可以通过result.dataSet取得
                int id = (int)result5.Rows[0]["id"];
                Log.Info($"id:{id}");
            }

            // 存储过程调用
            // 此处创建参数可以有多中方式
            // 详情可以参考重载中的几种方式
            var result4 = dbHelper.Procedure("pr_test", Parameter.Create("@id", 1), "@id2".ToParameter(2));
            // 存储过程中返回已经默认写好了
            // 直接调用结果的变量即可得到，但需要根据返回进行强转
            if ((int)result4.ReturnValue == 0)
            {
                // 如果有select返回
                var count = result4.Tables.Count;
                Log.Info($"count:{count}");
            }

            // 异步执行SQL
            // 某些数据更新不需要阻塞主线程且不需要即时返回数据
            // 可以通过此函数进行异步队列执行
            dbHelper.PushSQL("UPDATE tb_test SET content = 'Hello' WHERE id = 1");
            // 异步执行存储过程
            // 功能同 异步执行SQL
            dbHelper.PushProcedure("pr_test", Parameter.Create("@id", 1), "@id2".ToParameter(2));
        }

        /// <summary>
        /// 数据代理使用
        /// </summary>
        public void DataAgentUseDemo()
        {
            // 创建一个数据表代理
            // 代理是为了某些高频读写操作而设计的缓存
            // 代理可以事先根据条件读取一张表
            // 读取成功后可以长时间对表进行读和写
            // var dbagentRows = dbHelper.CreateDataEntityRows("id", "tb_test", "id > 0");
            var dbagentRows = DataEntityRows.Load(dbHelper, "id", "tb_test", "id > 0");
            // 读取表id为100的记录
            var row = dbagentRows[100];
            // 读取表id为100记录的content字段
            var content = row["content"];
            // 读取表id为100记录的content字段
            var content2 = row.GetObject<string>("content");
            Log.Info($"content:{content},{content2}");
        }

        /// <summary>
        /// 非关系型数据助手使用
        /// </summary>
        public void NoDBStorageUseDemo()
        {
            // 框架引入一个菲关系型数据结构的概念设计
            // 通过新建一个NoDBStorage对象，确定键值的类型，这个类型和数据库中的字段类型对应即可，在写入一些必要条件，即可完成初始化
            // 这个类可以创建一个只存在键值关系的数据结构，创建之后即可通过对象进行快速的数据访问和存储
            // 注意一旦读取，数据将托管的在程序内存中，数据库只是用于持久化保存方案，所以如果键值关系是比较重要且处理频繁的数据
            // 切记不要直接修改数据库
            // 另外由于整个框架除了引入一些必要框架外 全部原生类和算法实现，所以对于数据的插入没有进行注入检测 需要根据各自需求进行检测
            // var db = dbHelper.CreateNoSqlStorage<int, string>("id", "content", "tb_cus_accounts", 10000/* 这个时间为持久化更新周期 */);
            var db = new NoSqlStorage<int, string>(dbHelper, "id", "content", "tb_cus_accounts", 10000/* 这个时间为持久化更新周期 */);
            // 通过建立的对象来加入一个新的数据
            // 注意此对象所在的表 不能够存在 一些约束性规则 来阻止记录的插入 否则失败
            db.TryAdd(100, "hello world");

            Console.ReadLine();
            // 通过建立的对象来拾取数据
            // 如果存在则为true 并且返回数据对象
            if (db.TryGetValue(100, out string value))
            {
                Log.Info($"value:{value}");
            }

            Console.ReadLine();
            // 通过建立的对象进行修改值
            // 修改的数据会周期性写入持久化数据库中
            db.SetValue(100, "hello world again!!!");
            // 立即刷新持久化数据
            // 当有需求需要立刻通过键值更新到持久化中的时候可以使用
            // 建议不要频繁使用，这样不就没价值了不是
            db.Flush();
            // 如果需要重新拉取数据，那么就调用这个吧
            // 清空函数会把内存中的数据清空，然后后续操作全部会重新读取数据库最新数据
            db.Clear();
        }

        public void CheckConnectedException(SqlServerDbHelper helper, Exception exception)
        {
            Log.Exception(exception, "CheckConnectedException");
        }

        public void CommandSQLException(SqlServerDbHelper helper, string sql, Exception exception)
        {
            Log.Exception(exception, "CommandSQLException");
        }

        public void ProcedureException(SqlServerDbHelper helper, string procedure, SqlParameter[] sqlParameters, Exception exception)
        {
            Log.Exception(exception, "ProcedureException");
        }

        #region 数据库表配置加载器测试样本
        /// <summary>
        /// 数据库表配置加载器
        /// </summary>
        public void ConfigLoaderDemo()
        {
            // 创建一个加载器
            // var loader = dbHelper.CreateConfigLoader<TestConfig>("SELECT * FROM tb_configs WITH(NOLOCK)");
            var loader = new ConfigLoader<TestConfig>(dbHelper, "SELECT * FROM tb_configs WITH(NOLOCK)");
            // 遍历配置
            foreach (var item in loader.Configs)
            {
                // do something
            }
            // 根据主键查找某一个配置
            var config = loader.Find(1);
            Log.Info("config name:" + config.name);
            // 重新拉取所有配置
            loader.Reload();
        }

        /// <summary>
        /// 测试配置
        /// 配置需要继承 ConfigLoaderItem 加载器子类
        /// </summary>
        class TestConfig : DataTableConfig
        {
            public int id;
            public string name;
            public float ratio;

            protected override void SetConfig(DataRow row)
            {
                id = (int)row["id"];
                name = row["name"].ToString();
                // 此处写法是为了避免编译器导致的无限小数问题 而没有直接强转
                ratio = float.Parse(row["ratio"].ToString());
            }

            protected override object SetPrimaryKey(DataRow row)
            {
                /** 两种写法都可 **/
                // return row["id"];
                return id;
            }
        }
        #endregion
    }
}
