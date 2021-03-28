using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace LiveStatsPoller
{
    class Program
    {

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        private static int _pollCount;

        private static Timer _timer;

        static void Main(string[] args)
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
            {
                // ERROR: Unable to get console mode.
               
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;
            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode))
            {
                // ERROR: Unable to set console mode
                //return false;
            }
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
            //DisableQuickEdit();
            while (true)
            {
                //Console.ReadKey();
            };
        }

        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var utcTimeNow = DateTime.UtcNow;
            Console.CursorLeft = 0;
            Console.CursorTop = 1;
            Console.Write($"{utcTimeNow}");

            if (utcTimeNow.Second == 0 && utcTimeNow.Minute % 5 == 0)
            {
                
                Task.Run(async () =>
                {
                    var pollResult = await Fetch();
                    Console.CursorLeft = 0;
                    Console.CursorTop = 2;
                    Console.Write($"Last Poll result: {pollResult}");
                    
                    var rankTeamsResult = await RankTeams();
                    Console.CursorLeft = 0;
                    Console.CursorTop = 3;
                    Console.Write($"RankTeam result: {rankTeamsResult}");
                    
                    _pollCount++;
                    Console.CursorLeft = 0;
                    Console.CursorTop = 4;
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

        private static async Task<string> RankTeams()
        {
            var httpService = new HttpService();

            var result = await httpService.GetAsync<string>("https://api.timkoto.com/dev/api/utility/v1/RankTeams", new Dictionary<string, string>
            {
                {"x-api-key", "052d7c2822msh1effd682c0dbce0p113fabjsn219fbe03967c"}
            });

            return result;
        }

       
    }
}
