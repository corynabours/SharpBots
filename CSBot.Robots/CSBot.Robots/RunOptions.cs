using System;
using System.Collections.Generic;
using System.Linq;

namespace CSBot.Robots
{
    public class RunOptions
    {
        public RunOptions(string[] args)
        {
            // look for resolution arg
            var xres = 800;
            var yres = 800;
            var arguments = args.ToList();

            if (arguments.Count() > 2)
            {
                if (IsNumeric(arguments[0]) && IsNumeric(arguments[1]))
                {
                    xres = Convert.ToInt32(args[0]);
                    yres = Convert.ToInt32(args[1]);
                    arguments.RemoveAt(0);
                    arguments.RemoveAt(0);
                }
            }

            var seed = DateTime.Now.Ticks;
            var seedArgument = SearchForArgumentStartingWith("#", arguments);
            if (!string.IsNullOrEmpty(seedArgument))
                seed = Convert.ToInt32(seedArgument);

            var timeout = 50000;
            var timeoutArgument = SearchForArgumentStartingWith("-timeout=", arguments);
            if (!string.IsNullOrEmpty(timeoutArgument))
                timeout = Convert.ToInt32(timeoutArgument);


            /*
show_radar = false
ARGV.grep( /^show_radar/ )do |item|
  show_radar = true
  ARGV.delete(item)
end*/
            var teams = 8;
            var teamsArgument = SearchForArgumentStartingWith("-teams=", arguments);
            if (!string.IsNullOrEmpty(teamsArgument))
                teams = Convert.ToInt32(teamsArgument);
            if (teams < 0 || teams > 8) teams = 8;

            if (arguments.Count == 0 || arguments.Count > 8)
            { 
                Error = true;
                return;
            }


            Battlefield = new Battlefield(xres * 2, yres * 2, timeout, seed);

            var c = 0;
            var teamDivider = Math.Ceiling(Convert.ToDouble(arguments.Count / teams));
            Error = false;
            foreach (var argument in arguments)
            {
                Robot robotShell = null;
                try
                {
                    var o = Activator.CreateInstance(argument, argument + "." + argument);
                    robotShell = (Robot)o.Unwrap();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading robot: " + argument);
                    Console.WriteLine(ex.ToString());
                    Error = true;
                }
                var team = (int)(c / teamDivider);
                c += 1;
                var robot = new RobotRunner(robotShell, Battlefield, team);
                Battlefield.Add(robot);
            }
        }

        public bool Error { get; set; }

        public Battlefield Battlefield { get; set; }

        private static string SearchForArgumentStartingWith(string pattern, List<string> arguments)
        {
            var value = string.Empty;
            for (var index = 0; index < arguments.Count; index++)
            {
                if (arguments[index].Substring(0, Math.Min(pattern.Length, arguments[index].Length)).ToLower() != pattern.ToLower()) continue;
                value = arguments[index].Substring(pattern.Length);
                arguments.RemoveAt(index);
            }
            return value;
        }

        private static bool IsNumeric(string value)
        {
            double result;
            return Double.TryParse(value, out result);
        }
    }
}
