using System;
using System.Collections.Generic;
using System.Linq;
using CSBot.Robots;

namespace CSRobotsNoUI
{
    internal class Program
    {
        private static void Main(string[] args)
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
                Usage();


            var battlefield = new Battlefield(xres*2, yres*2, timeout, seed);

            var c = 0;
            var teamDivider = Math.Ceiling(Convert.ToDouble(arguments.Count/teams));
            var error = false;
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
                    error = true;
                }
                var team = (int) (c/teamDivider);
                c += 1;
                var robot = new RobotRunner(robotShell, battlefield, team);
                battlefield.Add(robot);
            }

            if (error)
            {
                Usage();
            }
            else
            RunOutOfGui(battlefield);
        }

        private static string SearchForArgumentStartingWith(string pattern, List<string> arguments)
        {
            var value = string.Empty;
            for (var index = 0; index < arguments.Count; index++)
            {
                if (arguments[index].Substring(0, Math.Min(pattern.Length,arguments[index].Length)).ToLower() != pattern.ToLower()) continue;
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


        public static void Usage()
        {
            Console.WriteLine(
                "usage: CSRobotsNoUI [resolution] [#match] [-speed=<N>] [-timeout=<N>] [-teams=<N>] <FirstRobotClassName[.rb]> <SecondRobotClassName[.rb]> <...>");
            Console.WriteLine("\t[resolution] (optional) should be of the form 640 480 or 800 600. default is 800 800");
            Console.WriteLine("\t[match] (optional) to replay a match, put the match# here, including the #sign.  ");
            Console.WriteLine("\t[-timeout=<N>] (optional, default 50000) number of ticks a match will last at most.");
            Console.WriteLine(
                "\t[-teams=<N>] (optional) split robots into N teams. Match ends when only one team has robots left.");
            Console.WriteLine("\tthe names of the rb files have to match the class names of the robots");
            Console.WriteLine("\t(up to 8 robots)");
            Console.WriteLine("\te.g. 'ruby rrobots.rb SittingDuck NervousDuck'");
            Console.WriteLine("\t or 'ruby rrobots.rb 600x600 #1234567890 SittingDuck NervousDuck'");
        }

        public static void RunOutOfGui(Battlefield battlefield)
        {
            Console.WriteLine("match ends if only 1 bot/team left or dots get here-->|");

            while (!battlefield.GameOver)
            {
                battlefield.Tick();
                if (Convert.ToInt32(battlefield.Time%(battlefield.Timeout/54)) == 0) Console.Write(".");
            }

            PrintOutcome(battlefield);
            //exit(0);
        }

        /*def run_in_gui(battlefield, xres, yres, speed_multiplier, show_radar)
  require 'tkarena'
  arena = TkArena.new(battlefield, xres, yres, speed_multiplier)
  arena.show_radar = show_radar
  game_over_counter = battlefield.teams.all?{|k,t| t.size < 2} ? 250 : 500
  outcome_printed = false
  arena.on_game_over{|battlefield|
    unless outcome_printed
      print_outcome(battlefield)(
      outcome_printed = true
    end
    exit 0 if game_over_counter < 0
    game_over_counter -= 1
  }
  arena.run
end*/

        public static void PrintOutcome(Battlefield battlefield)
        {
            var winners = battlefield.Robots.Where(robot => !robot.Dead()).ToList();
            Console.WriteLine("");
            if (battlefield.Robots.Count > battlefield.Teams)
            {
                /* teams = battlefield.teams.find_all{|name,team| !team.all?{|robot| robot.dead} }
            puts "winner_is:     { #{
              teams.map do |name,team|
                "Team #{name}: [#{team.join(', ')}]"
              end.join(', ')
            } }"
            puts "winner_energy: { #{
              teams.map do |name,team|
                "Team #{name}: [#{team.map do |w| ('%.1f' % w.energy) end.join(', ')}]"
              end.join(', ')
            } }"*/
            }
            else
            {
                Console.WriteLine("winner_is:     [" + GetNames(winners) + "]");
                Console.WriteLine("winner_energy: [" + GetEnergies(winners) + "]");
            }
            Console.WriteLine("elapsed_ticks: " +  battlefield.Time);
            Console.WriteLine("seed :         " + battlefield.Seed);
            Console.WriteLine("");
            Console.WriteLine("robots :");
            foreach (var robot in battlefield.Robots)
            {
                Console.WriteLine("  " + robot.Name + ":");
                Console.WriteLine("    damage_given: " + robot.DamageGiven);
                Console.WriteLine("    damage_taken: " + (100 - robot.Energy));
                Console.WriteLine("    kills:        " + robot.Kills);
            }
        }

        private static string GetEnergies(IEnumerable<RobotRunner> winners)
        {
            var energies = string.Empty;
            foreach (var robot in winners)
            {
                if (!string.IsNullOrEmpty(energies))
                    energies += ",";
                energies += robot.Energy;
            }
            return energies;
        }

        private static string GetNames(IEnumerable<RobotRunner> winners)
        {

            var names = string.Empty;
            foreach (var robot in winners)
            {
                if (!string.IsNullOrEmpty(names))
                    names += ",";
                names += robot.Name;
            }
            return names;
        }
    }
}
