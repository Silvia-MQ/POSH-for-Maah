using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSH.sys;
using System.Threading;
using System.IO;
using POSH.sys.parse;
using POSH.sys.events;
using POSH.sys.strict;

namespace MaahBase
{
    /// <summary>
    /// Implementation of a Maah Posh Agent.
    /// </summary>
    public class MaahBase
    {
        

        protected internal bool _execLoop;// = false;
        protected internal bool _loopPause;// = false;

        //later write code for tcp and event handler here
        //private List<TCPserver>

        public MaahBase(string plan)
        {
            //instantiate TCP here

        }
        
        // load the plan
        
        // loop thread control

        public List<POSH.sys.Tuple<string,string>> CreateAgents(string plan)
        {
            AgentBase[] agents = null;
            agents = application.control.CreateAgents(verbose, assembly, agentsInit, setting);
            agents.LoadPlan(plan);
        }
        
    }
    

}