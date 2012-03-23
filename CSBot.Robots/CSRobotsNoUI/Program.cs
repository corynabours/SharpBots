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
            var runOptions = new RunOptions(args);
            if (runOptions.Error)
            {
                Usage();
            }
            else
            {
                RunOutOfGui(runOptions.Battlefield);
            }
        }

        private static void Usage()
        {
            Console.WriteLine(
                "usage: CSRobotsNoUI [resolution] [#match] [-timeout=<N>] [-teams=<N>] <FirstRobotClassName[.rb]> <SecondRobotClassName[.rb]> <...>");
            Console.WriteLine("\t[resolution] (optional) should be of the form 640 480 or 800 600. default is 800 800");
            Console.WriteLine("\t[match] (optional) to replay a match, put the match# here, including the #sign.  ");
            Console.WriteLine("\t[-timeout=<N>] (optional, default 50000) number of ticks a match will last at most.");
            Console.WriteLine("\t[-teams=<N>] (optional) split robots into N teams. Match ends when only one team has robots left.");
            Console.WriteLine("\tthe names of the robot files have to match the class names of the robots, and the namespaces of the robot classes.");
            Console.WriteLine("\t(up to 8 robots)");
            Console.WriteLine("\te.g. 'CSRobotsNoUI SittingDuck NervousDuck'");
            Console.WriteLine("\t or 'CSRobotsNoUI 600 600 #1234567890 SittingDuck NervousDuck'");
        }

        private static void RunOutOfGui(Battlefield battlefield)
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

        private static void RunInGui(Battlefield battlefield, int xres, int yres, int speedMultiplier, bool showRadar)
        {
            //arena = TkArena.new(battlefield, xres, yres, speed_multiplier)
            //arena.show_radar = show_radar
            //game_over_counter = battlefield.teams.all? {|k,t | t.size < 2}?250 :500
            //outcome_printed = false
            //arena.on_game_over{|battlefield |
            //    unless outcome_printed
            //    {
            //       print_outcome(battlefield)
            //       outcome_printed = true
            //    }
            //    exit 0 if game_over_counter < 0
            //    game_over_counter -= 1
            //}
            //arena.run
        }

        private static void PrintOutcome(Battlefield battlefield)
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
