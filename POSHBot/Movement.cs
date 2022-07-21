using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSH.sys;
using POSH.sys.annotations;
using Posh_sharp.POSHBot.util;
using POSH.sys.strict;


/// <summary>
/// This file contains all actions and senses of Maah
/// </summary>
namespace Posh_sharp.POSHBot
{
    public class Movement : UTBehaviour
    {
        public Movement(AgentBase agent)
            : base(agent,
            new string[] {},
            new string[] {})
        {

        }


        /// <summary>
        /// Whether Maah is been petted now
        /// </summary>
        /// <returns></returns>
        [ExecutableSense("been_petted")]
        public int been_petted()
        {
            return GetBot().GetFreq(true);
        }

        [ExecutableSense("forward_frq")]
        public int forward_frq()
        {
            return GetBot().GetFreq("Forward");
        }

        [ExecutableSense("backward_frq")]
        public int backward_frq()
        {
            return GetBot().GetFreq("Backward");
        }

        [ExecutableSense("turnleft_frq")]
		public int turnleft_frq()
        {
            return GetBot().GetFreq("TurnLeft");
        }

		[ExecutableSense("turnright_frq")]
		public int turnright_frq()
        {
            return GetBot().GetFreq("TurnRight");
        }


        /// <summary>
        /// How many times that Maah been petted for last 20 behaviours
        /// </summary>
        /// <returns></returns>
        [ExecutableSense("petted_frequency_past")]
        public int petted_frequency_past()
        {
            return GetBot().GetFreq(false);
        }


        [ExecutableSense("greeting_frq")]
		public int greeting_frq()
        {
            return GetBot().GetFreq("greeting");

        }

		[ExecutableSense("separation_frq")]
		public int separation_frq()
        {
            return GetBot().GetFreq("Seperation");
        }

        [ExecutableSense("surprise_frq")]
        public int surprise_frq()
        {
            return GetBot().GetFreq("Surprise");
        }

        [ExecutableSense("cuddle_frq")]
        public int cuddle_frq()
        {
            return GetBot().GetFreq("Cuddle");
        }

        [ExecutableSense("mothering_frq")]
        public int mothering_frq()
        {
            return GetBot().GetFreq("Mothering");
        }

        [ExecutableSense("social_call_frq")]
        public int social_call_frq()
        {
            return GetBot().GetFreq("SocialCall");
        }

        [ExecutableSense("escape_frq")]
        public int escape_frq()
        {
            return GetBot().GetFreq("Escape");
        }

        [ExecutableSense("caress_frq")]
        public int caress_frq()
        {
            return GetBot().GetFreq("Caress");
        }



        ///
        /// ACTIONS
        /// 

        /// <summary>
        /// Idleing
        /// </summary>
        /// <returns></returns>
        [ExecutableAction("idle")]
        public bool idle()
        {
            return GetBot().Send("Iddle");
        }

        [ExecutableAction("act_forward")]
		public bool act_forward()
        {
            return  GetBot().Send("Forward"); 
        }

        [ExecutableAction("act_backward")]
        public bool act_backward()
        {
            return GetBot().Send("Backward"); 
        }


        [ExecutableAction("act_turnleft")]
        public bool act_turnleft()
        {
            return GetBot().Send("TurnLeft");
        }

        [ExecutableAction("act_turnright")]
        public bool act_turnright()
        {
            return GetBot().Send("TurnRight");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ExecutableAction("act_greeting")]
        public bool act_greeting()
        {
            return GetBot().Send("Greeting");
        }

        [ExecutableAction("act_separation")]
        public bool act_separation()
        {
            return GetBot().Send("Seperation");
        }

        [ExecutableAction("act_surprise")]
        public bool act_surprise ()
        {
            return GetBot().Send("Surprise");
        }

        [ExecutableAction("act_cuddle")]
        public bool act_cuddle()
        {
            return GetBot().Send("Cuddle");
        }

        [ExecutableAction("act_mothering")]
        public bool act_mothering()
        {
            return GetBot().Send("Mothering");
        }

        [ExecutableAction("act_social_call")]
        public bool act_social_call()
        {
            return GetBot().Send("SocialCall");
        }

        [ExecutableAction("act_escape")]
        public bool act_escape()
        {
            return GetBot().Send("Escape");
        }

        [ExecutableAction("act_caress")]
        public bool act_caress()
        {
            return GetBot().Send("Caress");
        }

}
    
}