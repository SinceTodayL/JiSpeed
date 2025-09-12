using StackExchange.Redis;
using Microsoft.Extensions.Configuration;


namespace JISpeed.Infrastructure.Redis
{
    public class RedisService
    {
        private readonly IDatabase _db;
        private readonly ConnectionMultiplexer _redis;

        public RedisService(IConfiguration configuration)
        {
            // 从配置文件读取连接字符串
            var connectionString = configuration["Redis:ConnectionString"];

            // 初始化 Redis 连接（使用 Lazy 延迟加载，提高性能）
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _db = _redis.GetDatabase(); // 默认使用 DB0
        }

        // 提供公共方法操作 Redis
        public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
            => await _db.StringSetAsync(key, value, expiry);

        public async Task<string> GetStringAsync(string key)
            => await _db.StringGetAsync(key);

        // 其他常用方法...
        public async Task<bool> KeyDeleteAsync(string key) => await _db.KeyDeleteAsync(key);
        public async Task<bool> KeyExistsAsync(string key) => await _db.KeyExistsAsync(key);
    }
}