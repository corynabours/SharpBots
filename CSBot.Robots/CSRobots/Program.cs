using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CSBot.Robots;
using CSRobots.ViewModels;

namespace CSRobots
{
    internal class Program
    {
        public static RunOptions RunOptions { get; set; }
        private static void Main(string[] args)
        {
            var runOptions = new RunOptions(args);
            if (runOptions.Error)
            {
                Usage();
            }
            else
            {
                if (runOptions.ShowUi)
                    RunInGui(runOptions);
                else
                    RunOutOfGui(runOptions.Battlefield);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("usage: CSRobots [resolution] [#match] [-timeout=<N>] [-teams=<N>] <FirstRobotClassName[.rb]> <SecondRobotClassName[.rb]> <...>");
            Console.WriteLine("\t[resolution] (optional) should be of the form 640 480 or 800 600. default is 800 800");
            Console.WriteLine("\t[match] (optional) to replay a match, put the match# here, including the #sign.  ");
            Console.WriteLine("\t[-nogui] (optional) run the match without the gui, for highest possible speed.(ignores speed value if present)");
            Console.WriteLine("\t[-timeout=<N>] (optional, default 50000) number of ticks a match will last at most.");
            Console.WriteLine("\t[-teams=<N>] (optional) split robots into N teams. Match ends when only one team has robots left.");
            Console.WriteLine("\tthe names of the robot files have to match the class names of the robots, and the namespaces of the robot classes.");
            Console.WriteLine("\t(up to 8 robots)");
            Console.WriteLine("\te.g. 'CSRobots SittingDuck NervousDuck'");
            Console.WriteLine("\t or 'CSRobots 600 600 #1234567890 SittingDuck NervousDuck'");
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

        private static void RunInGui(RunOptions runOptions)
        {
            var t = new Thread(ThreadProc);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(runOptions); 
        }

        private static void ThreadProc(object arg)
        {
            var runOptions = arg as RunOptions;
            if (runOptions == null) return;
            MainView.RunOptions = runOptions;
            App.Main();
            PrintOutcome(runOptions.Battlefield);
        }


        private static void PrintOutcome(Battlefield battlefield)
        {
            var winners = battlefield.Robots.Where(robot => !robot.Dead()).ToList();
            Console.WriteLine("");
            if (battlefield.Robots.Count > battlefield.Teams)
            {
                var winningTeams = new List<int>();
                foreach (var robot in winners.Where(robot => !winningTeams.Contains(robot.Team)))
                    winningTeams.Add(robot.Team);
                foreach (var winningTeam in winningTeams)
                {
                    var teamNum = "Team #" + winningTeam.ToString(CultureInfo.InvariantCulture);
                    var teamMembers = battlefield.Robots.Where(robot => robot.Team == winningTeam).ToList();
                    var winningTeamNames = GetNames(teamMembers);
                    var winningTeamEnergies = GetEnergies(teamMembers);
                    Console.WriteLine("winner_is:     { " + teamNum + ": " + winningTeamNames + "}");
                    Console.WriteLine("winner_energy: { " + teamNum + ": " + winningTeamEnergies + "}");
                }
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
