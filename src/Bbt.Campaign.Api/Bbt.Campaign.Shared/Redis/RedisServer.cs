using System.Diagnostics;

namespace Bbt.Campaign.Shared.Redis
{
    public static class RedisServer
    {
        public static void StartRedis()
        {
            try
            {
                string serverFileName = "redis-server.exe";
                string cliFileName = "redis-cli.exe";

                var dir = Directory.GetCurrentDirectory() + "\\bin\\";
                var ProcessServer = new Process()
                {
                    StartInfo = new ProcessStartInfo(dir + serverFileName)
                };
                bool isStart = ProcessServer.Start();

                ProcessCli = new Process()
                {
                    StartInfo = new ProcessStartInfo(dir + cliFileName)
                };
                isStart = ProcessCli.Start();
            }
            catch (Exception EX)
            {
            }

        }

        private static Process ProcessServer { get; set; }
        private static Process ProcessCli { get; set; }

    }
}
