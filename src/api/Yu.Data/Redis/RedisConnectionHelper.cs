using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace Yu.Data.Redis
{
    public class RedisConnectionHelper
    {
        // 日志
        private readonly ILogger<RedisConnectionHelper> _logger;

        // 连接字符串
        // 127.0.0.1:6379,password=123456,connectTimeout=1000,connectRetry=1,syncTimeout=1000"
        private readonly string _redisConnectionString = string.Empty;

        // 自定义key前缀
        public string SysCustomKey { get; private set; }

        // 转换器字典
        private readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache
            = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        public RedisConnectionHelper(ILogger<RedisConnectionHelper> logger, IOptions<RedisOption> option)
        {
            _logger = logger;
            _redisConnectionString = option.Value.RedisExchangeHosts;
            SysCustomKey = option.Value.RedisKey;
        }

        /// <summary>
        /// 返回redis帮助器
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public RedisHelper GetRedisHelper(string connectionString = null)
        {
            var connectionMultiplexer = GetConnectionMultiplexer(connectionString);
            return new RedisHelper(this, connectionMultiplexer);
        }

        /// <summary>
        /// 根据连接字符串取得连接转换器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnectionMultiplexer(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = string.Empty;
            }

            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = InitConectionMultiplexer(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        /// <summary>
        /// 初始化连接转换器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns></returns>
        private ConnectionMultiplexer InitConectionMultiplexer(string connectionString)
        {
            connectionString = string.IsNullOrEmpty(connectionString) ? _redisConnectionString : connectionString;
            var connect = ConnectionMultiplexer.Connect(connectionString);

            connect.ConnectionFailed += Connect_ConnectionFailed;
            connect.ConnectionRestored += Connect_ConnectionRestored;
            connect.ErrorMessage += Connect_ErrorMessage;
            connect.ConfigurationChanged += Connect_ConfigurationChanged;
            connect.HashSlotMoved += Connect_HashSlotMoved;
            connect.InternalError += Connect_InternalError;

            return connect;
        }

        #region 事件

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogError("连接失败：" + e.ConnectionType + "," + e.EndPoint + "," + e.FailureType + "," + e.Exception);
        }

        /// <summary>
        /// 连接重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogError("连接重置：" + e.ConnectionType + "," + e.EndPoint + "," + e.FailureType + "," + e.Exception);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger.LogError("发生错误：" + e.EndPoint + "," + e.Message);
        }

        /// <summary>
        /// 连接配置变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_ConfigurationChanged(object sender, EndPointEventArgs e)
        {
            _logger.LogError("连接配置变化：" + e.EndPoint);
        }

        /// <summary>
        /// 集群变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_HashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            _logger.LogError("集群变化：" + e.OldEndPoint + "," + e.NewEndPoint + "," + e.HashSlot);
        }

        /// <summary>
        /// 类库内部异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_InternalError(object sender, InternalErrorEventArgs e)
        {
            _logger.LogError("集群变化：" + e.ConnectionType + "," + e.EndPoint + "," + e.Exception);
        }

        #endregion
    }
}
