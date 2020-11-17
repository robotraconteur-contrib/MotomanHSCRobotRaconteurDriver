using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Options;
using RobotRaconteur;
using RobotRaconteur.Companion.InfoParser;

namespace MotomanHSCRobotRaconteurDriver
{
    class Program
    {
        static int Main(string[] args)
        {

            bool shouldShowHelp = false;
            string robot_info_file = null;

            var options = new OptionSet {
                { "robot-info-file=", n => robot_info_file = n },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null }
            };

            List<string> extra;
            try
            {
                // parse the command line
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write("MotomanHSCRobotRaconteurDriver: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `MotomanHSCRobotRaconteurDriver --help' for more information.");
                return 1;
            }

            if (shouldShowHelp)
            {
                Console.WriteLine("Usage: MotomanHSCRobotRaconteurDriver [Options+]");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            if (robot_info_file == null)
            {
                Console.WriteLine("error: robot-info-file must be specified");
                return 1;
            }


            var robot_info = RobotInfoParser.LoadRobotInfoYamlWithIdentifierLocks(robot_info_file);
            using (robot_info.Item2)
            {



                using (var robot = new MotomanHSCRobot(robot_info.Item1))
                {
                    robot._start_robot();
                    using (var node_setup = new ServerNodeSetup("Motoman_robot", 58651, args))
                    {


                        RobotRaconteurNode.s.RegisterService("robot", "com.robotraconteur.robotics.robot", robot);

                        
                        Console.WriteLine("Press enter to exit");
                        Console.ReadKey();                      


                    }
                }
            }

            return 0;

        }
    }
}
