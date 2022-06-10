using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSH.sys.exceptions;
using System.IO;
using System.Threading;
using POSH.sys;
using System.Reflection;
using MaahBase;

#if LOG_ON
    using log4net;
#endif

namespace POSH.executing
{
    /// <summary>
    /// Launches a POSH agent or a set of agents.
    /// 
    /// Synopsis:
    ///     launcher [OPTIONS] -v -p=library
    ///     eg.-v -p=Maah_4

    /// Description:
    ///     Launches a POSH agent by fist initialising the world and then the
    ///     agents. The specified library is the behaviour library that will be used.
    /// 
    ///     -v, --verbose
    ///         writes more initialisation information to the standard output.
    /// 
    ///     -h, --help
    ///         print this help message.
    /// 
    ///     -p, --plan-file=PLANFILE
    ///         initialises a single agent to use the given plan. Only the name of
    ///         the plan without the path needs to be given, as it is assumed to have
    ///         the ending '.lap' and reside in the default location in the
    ///         corresponding behaviour library.
    /// </summary>
    class Launcher
    {
        const string helpText = @"
          Launches a POSH agent or a set of agents.
     
     
             Synopsis:
             launcher [OPTIONS] -v -p = library

         Description:
             Launches a POSH agent by fist initialising the world and then the
             agents. The specified library is the behaviour library that will be used.

             -h, --help
                 print this help message.
             
             -v, --verbose
                 writes more initialisation information to the standard output.
     
         Environment:
             ALL used paths are currently relative to the execution assembly directory. 
             Ideally all POSH related elements should be in the same folder anyway. 

             -p, --plan-dir=PLANFILE
                initialises a single agent to use the given plan. Only the name of
                the plan without the path needs to be given, as it is assumed to have
                the ending '.lap' and reside in the default location in the
                corresponding behaviour library.         

             ";

        internal AssemblyControl control;

        public Launcher()
        {
            control = AssemblyControl.GetControl();
        }

        /// <summary>
        /// Parses the command line options and returns them.
        /// 
        /// The are returned in the order help, verbose, world_file, world_args,
        /// agent_file, plan_file. help and verbose are boolean variables. All the
        /// other variables are strings. If they are not given, then an empty string
        /// is returned.
        /// </summary>
        /// <param name="argv"></param>
        /// <returns></returns>
        /// <exception cref="UsageException"> whenever something goes wrong with the input string</exception>
        protected sys.Tuple<bool, bool, string> ProcessOptions(string[] args)
        {
            // default values
            bool help = false, verbose = false;
            string plan = "";
            // parse options


            foreach (string arg in args)
            {
                string[] tuple = arg.Split(new string[] { "=" }, 2, StringSplitOptions.None);
                switch (tuple[0])
                {
                    case "-h":
                    case "--help":
                        help = true;
                        break;
                    case "-v":
                    case "--verbose":
                        verbose = true;
                        break;
                    case "-p":
                    case "--plan-dir":
                        if (!control.isPlan("",tuple[1]))
                            throw new UsageException(string.Format("cannot find specified plans file '{0}' in the '{1}' path", tuple[1],control.config["PlanPath"]));
                        plan = tuple[1];
                        break;
                    default:
                        if (tuple[0].StartsWith("-") || tuple.Length > 1)
                            throw new UsageException("unrecognised option: " + tuple[0]);
                        break;
                }
            }
            if (help)
                return new sys.Tuple<bool, bool, string>(help, false, "");

            // all fine
            return new sys.Tuple<bool, bool, string>(help, verbose, plan);
        }


        // write the class for TCP, check the input for initTCP
        /// <summary>
        /// Initialise the TCP server and returns the object.
        /// </summary>
        /// <returns></returns>
        //protected POSH.sys.Tuple<TCP, bool> InitTCP(string worldArgs, string assembly, List<POSH.sys.Tuple<string, object>> agentsInit, bool verbose, Type world)
       // {
       //     return new POSH.sys.Tuple<TCP, bool>(null, false);
       // }




        public static void Main(string[] args)
        {

            bool help = false, verbose = false;
            string plan = "";

            AgentBase[] agents = null;

            // process command line arguments
            Launcher application = new Launcher();

            sys.Tuple<bool, bool,string> arguments = null;
            if (args is string[] && args.Length > 0)
            {
                arguments = application.ProcessOptions(args); 
            }
            else
            {
                Console.Out.WriteLine("for help use --help");
                Console.ReadKey();
                return;
            }
            if (arguments != null && arguments.First)
            {
                Console.Out.WriteLine(helpText);
                Console.ReadKey();
                return;
            }

            help = arguments.First;
            verbose = arguments.Second;
            plan = arguments.Third;


            if (verbose)
                Console.Out.WriteLine("- Laoding behaviour plans for Maah");

            //POSH.sys.strict.Agent[] agents = null;
            agents = application.control.CreateAgents(verbose, "", null, null);
            //agents = MaahBase.CreateAgents(verbose, plan); 

            if (agents == null)
                return;
            // start the agents
            bool loopsRunning = application.control.StartAgents(verbose, agents);

            loopsRunning = application.control.Running(verbose, agents, loopsRunning);

        }






    }
}
