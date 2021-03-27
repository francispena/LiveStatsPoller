using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace LiveStatsPoller
{
    class Program
    {
        private static int _pollCount;

        private static Timer _timer;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("Starting to poll....");

            _timer = new Timer
            {
                Interval = 1000,
                AutoReset = true,
                Enabled = false,
            };

            _timer.Elapsed += timer_Elapsed;
            _timer.Enabled = true;
            
            while (true)
            {
                Console.ReadKey();

            };
        }

        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var utcTimeNow = DateTime.UtcNow;
            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write($"{utcTimeNow}");

            if (utcTimeNow.Minute % 5 == 0)
            {
                _pollCount++;
                Task.Run(async () =>
                {
                    var pollResult = await Fetch();
                    Console.CursorLeft = 0;
                    Console.CursorTop = 2;
                    Console.Write($"Last Poll result: {pollResult}");
                    Console.CursorLeft = 0;
                    Console.CursorTop = 3;
                    Console.Write($"Polls made: {_pollCount}");
                    if (pollResult == "Games Done")
                    {
                        _timer.Enabled = false;
                    }
                });
            }
        }

        private static async Task<string> Fetch()
        {
            var httpService = new HttpService();

            var result = await httpService.GetAsync<string>("https://api.timkoto.com/dev/api/utility/v1/GetLiveStats", new Dictionary<string, string>
            {
                {"x-api-key", "052d7c2822msh1effd682c0dbce0p113fabjsn219fbe03967c"}
            });

            return result;
        }
    }
}
