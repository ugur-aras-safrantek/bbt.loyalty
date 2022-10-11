using Bbt.Campaign.Shared.Static;
using StackExchange.Redis;

namespace Bbt.Campaign.EntityFrameworkCore.Redis
{
    public class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        private readonly string _prefix;
        private readonly IConnectionMultiplexer _connectionMultiplexerB2C;

        public RedisDatabaseProvider(string connection)
        {
            _prefix = "CampaignApi:";

            var cluster = true;
            //string[] redisConnectionArray = StaticValues.Campaign_Redis_ConStr.ToString().Split(";");
            if (cluster)
            {  
                ConfigurationOptions config = new ConfigurationOptions();
                config.ChannelPrefix = _prefix;
                config.EndPoints.Add(StaticValues.Campaign_Redis_ConStr);
                config.Password = StaticValues.Campaign_Redis_Password;
                config.AbortOnConnectFail = false;
                config.ConnectTimeout = 30000;
                config.CommandMap = CommandMap.Default;

                _connectionMultiplexerB2C = ConnectionMultiplexer.Connect(config);
            }
            else
                _connectionMultiplexerB2C = ConnectionMultiplexer.Connect(connection);
        }


        //try read three times wit 0.5 second wait
        public async Task<string> GetAsync(string cacheKey)
        {
            try
            {
                return await ReadRedis(cacheKey);
            }
            catch (Exception)
            {
                Task.Delay(500).Wait();//yarım saniye bekle
                try
                {
                    return await ReadRedis(cacheKey);
                }
                catch (Exception)
                {
                    Task.Delay(500).Wait();//yarım saniye bekle
                    try
                    {
                        return await ReadRedis(cacheKey);
                    }
                    catch (Exception ex3)
                    {
                        throw ex3;
                    }
                }
            }

        }
        
        private async Task<string> ReadRedis(string cacheKey)
        {
            return await _connectionMultiplexerB2C.GetDatabase().StringGetAsync($"{_prefix}{cacheKey}");
        }
        //Try 2 times to write into redis
        public async Task SetAsync(string cacheKey, string value)
        {
            try
            {
                await WriteRedis(cacheKey, value);
            }
            catch (Exception)
            {
                try
                {
                    Task.Delay(500).Wait();//yarım saniye bekle
                    await WriteRedis(cacheKey, value);
                }
                catch (Exception)
                {
                    try
                    {
                        Task.Delay(500).Wait();//yarım saniye bekle
                        await WriteRedis(cacheKey, value);
                    }
                    catch (Exception ex2)
                    {
                        throw ex2;
                    }
                }
            }

        }
        public async Task SetWithMinuteAsync(string cacheKey, string value, int minutes)
        {
            try
            {
                await WriteRedisWithTime(cacheKey, value, minutes);
            }
            catch (Exception)
            {
                try
                {
                    Task.Delay(500).Wait();//yarım saniye bekle
                    await WriteRedisWithTime(cacheKey, value, minutes);
                }
                catch (Exception)
                {
                    try
                    {
                        Task.Delay(500).Wait();//yarım saniye bekle
                        await WriteRedisWithTime(cacheKey, value, minutes);
                    }
                    catch (Exception ex2)
                    {
                        throw ex2;
                    }
                }
            }

        }

        public async Task FlushDatabase()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"{StaticValues.Campaign_Redis_ConStr},allowAdmin=true");
            var server = redis.GetServer(StaticValues.Campaign_Redis_ConStr);
            await server.FlushDatabaseAsync(2);
        }

        private async Task WriteRedis(string cacheKey, string value)
        {
            await _connectionMultiplexerB2C.GetDatabase().StringSetAsync($"{_prefix}{cacheKey}", value, TimeSpan.FromDays(int.Parse(StaticValues.Campaign_Redis_Ttl)), When.Always);
        }
        private async Task WriteRedisWithTime(string cacheKey, string value, int minutes)
        {
            await _connectionMultiplexerB2C.GetDatabase().StringSetAsync($"{_prefix}{cacheKey}", value, TimeSpan.FromMinutes(minutes), When.Always);
        }

        public async Task<bool> RemoveAsync(string cacheKey)
        {

            await _connectionMultiplexerB2C.GetDatabase().KeyDeleteAsync($"{_prefix}{cacheKey}");
            return true;
        }
        public async Task<bool> RemoveByPattern(string pattern)
        {
            pattern = $"{_prefix}{pattern}";
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect($"{StaticValues.Campaign_Redis_ConStr},allowAdmin=true");
            var server = redis.GetServer(StaticValues.Campaign_Redis_ConStr);
            var keys = server.Keys(database: _connectionMultiplexerB2C.GetDatabase().Database, pattern: "*" + pattern + "*");
            foreach (var key in keys)
            {
                if (_connectionMultiplexerB2C.GetDatabase().KeyExists(key))
                {
                    await _connectionMultiplexerB2C.GetDatabase().KeyDeleteAsync(key);
                }
            }
            return true;

        }
    }
}
