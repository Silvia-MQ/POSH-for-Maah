using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POSH.sys;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using POSH.sys.strict;
using Posh_sharp.POSHBot.util;
using Newtonsoft.Json;


//Minqiu
//Later improvemnt 1: take ip adress and port from init file instead of a fixed value

namespace Posh_sharp.POSHBot
{
    public class Communicate
    {
        public Socket client;

        /// <summary>
        /// struct of behavviours
        /// The name of Attributes should match the key in json file from python
        /// </summary>
        public struct ToJsonMy
        {
            public int Reset { get; set; } 
            public int Forward { get; set; }
            public int TurnLeft { get; set; }
            public int TurnRight { get; set; }
            public int Backwards { get; set; }
            public int Iddle { get; set; }
            public int Cuddle { get; set; }
            public int Mothering { get; set; }
            public int Escape { get; set; }
            public int Greeting { get; set; }
            public int Seperation { get; set; }
            public int Surprise { get; set; }
            public int SocialCall { get; set; }
            public int Caress { get; set; }
        }

        /// <summary>
        /// struct of pet freq
        /// </summary>
        public struct JsonPet
        {
            public int PetNow { get; set; }
            public int PetPast { get; set; }
        }

        public ToJsonMy BehavFreq = new ToJsonMy();
        public JsonPet PetFreq = new JsonPet();
        /// <summary>
        /// Wrapper for InitTCP
        /// </summary>
        public Socket ExecuteClient()
        {
            client = InitTCP();
            return client;
        }
        /// <summary>
        /// Initialise a TCP connection
        /// and connect to endpoint with the port number of 12345
        /// </summary>
        static Socket InitTCP()
        {
            // Establish the remote endpoint
            // for the socket. This example
            // uses port 12345 on the local
            // computer.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[1]; //sometimes should be ipHost.AddressList[0] //MinQ: check the difference here
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 12345);

            // Creation TCP/IP Socket using
            // Socket Class Constructor
            Socket client = new Socket(ipAddr.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            // Connect Socket to the remote
            // endpoint using method Connect()
            client.Connect(localEndPoint);
            Console.WriteLine("Socket connected to -> {0} ",
                                  client.RemoteEndPoint.ToString());
            return client;
        }

        /// <summary>
        /// Send data through socket
        /// </summary>
        /// <param name="arg">
        /// The string to be sent to python, which should be the next behaviour of Maah 
        /// </param>
        /// <returns>
        /// The return data from python.
        /// Once a recognised command is sent to Python, 
        /// there will be two message to be recieved.
        /// Otherwise, only one message will be recieved
        /// </returns>
        public bool SendData(string arg)
        {
            byte[] message = Encoding.UTF8.GetBytes(arg);
            client.Send(message);
            Console.WriteLine("Data Sent:" + Encoding.UTF8.GetString(message));

            //Check if the sent command recognised by python
            if (RevieveData() is true)
                //recevie data again
                return RevieveData();
            else 
                return false ;
        }

        /// <summary>
        /// This function used to recieve data from python
        /// </summary>
        /// <returns> True for command executed, false for command fails</returns>
        public bool RevieveData()
        {
            byte[] receive = new byte[1024];
            var length = client.Receive(receive);

            //To check whether received data is matching behaviour freqency struct
            if (Encoding.UTF8.GetString(receive, 0, length).Contains("Unrecognised"))
            {
                Console.WriteLine("{0}", Encoding.UTF8.GetString(receive, 0, length)); // Message == unrecognised command

                //clear massage
                Array.Clear(receive, 0, length);

                //The behaviour fails
                return false;
            }
            else if (Encoding.UTF8.GetString(receive, 0, length).Contains("PetNow"))
            {
                //write Pet statue to PetFreq
                PetFreq = JsonConvert.DeserializeObject<JsonPet>(Encoding.UTF8.GetString(receive, 0, length));
                Console.WriteLine("Pet status recieved");

                //clear massage
                Array.Clear(receive, 0, length);

                return true;
            }
            else
            {
                //write behaviour frequencies to BehavFreq
                BehavFreq = JsonConvert.DeserializeObject<ToJsonMy>(Encoding.UTF8.GetString(receive, 0, length));
                Console.WriteLine("command executed,Behaviour frequencies received");

                //clear massage
                Array.Clear(receive, 0, length);

                return true;
            }            
        }
    }


    //POSHBot created as a means of evaluating Behaviour Oriented Design [BOD]
    //Much code here re-used from Andy Kwong's poshbot
    //It has been refactored on the 29/08/07 to make Bot a behaviour and clean
    //up the behaviour structure a bit.


    /// <summary>
    /// The Bot behaviour.
    ///    
    /// This behaviour does not provide any actions that are directly used in plans.
    /// Rather, it establishes the connection with UT and provides methods to
    /// control the bot which can be used by other behaviours.
    /// 
    /// The behaviour keeps a local copy of the bot state. Gamebots do not support
    /// queries on the agent sense, it sends a copy of the environment to the
    /// agent periodically.
    /// 
    /// To change connection IP, port and the bot's name, use the attributes
    /// Bot.ip, Bot.port and Bot.botname.
    /// </summary>
    public class POSHBot : UTBehaviour
    {
        Regex firstIntMatcher;
        Regex middleIntMatcher;
        Regex spaceMatcher;
        Regex itemMatcher;
        Regex attributeMatcher; 

        IPAddress ip;
        int port;
        string botName;

        int team;

        Communicate client = new Communicate();


        public POSHBot(AgentBase agent) : this(agent,null)
        {
            
        }

        public POSHBot(AgentBase agent, Dictionary<string,object> attributes)
            : base(agent, new string[] {}, new string[] {})
        {
            middleIntMatcher = new Regex(",(.*?),");
            firstIntMatcher = new Regex("(.*?),");
            spaceMatcher = new Regex(@"^(.+?)\s+(.+?)$");
            itemMatcher = new Regex(@"\{(.*?)\}");
            attributeMatcher = new Regex(@"\{(.*?)\s+(.*?)\}");
            // default connection values, use attributes to override
            ip = IPAddress.Parse("127.0.0.1");
            port = 3000;
            botName = "POSHbot";
            
            // Valid values for team are 0 and 1. If an invalid value is used gamebots
            //  will alternate the team on which each new bot is placed.
            team = -1;

            client.ExecuteClient();
        }

        /// <summary>
        /// Check the freqencies for each action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        internal int GetFreq(string action)
        {
            //check the freq of specific action
            int counts;
            if (action == "Reset")
                counts = client.BehavFreq.Reset;
            else if (action == "Forward")
                counts = client.BehavFreq.Forward;
            else if (action == "TurnLeft")
                counts = client.BehavFreq.TurnLeft;
            else if (action == "TurnRight")
                counts = client.BehavFreq.TurnRight;
            else if (action == "Backwards")
                counts = client.BehavFreq.Backwards;
            else if (action == "Iddle")
                counts = client.BehavFreq.Iddle;
            else if (action == "Cuddle")
                counts = client.BehavFreq.Cuddle;
            else if (action == "Mothering")
                counts = client.BehavFreq.Mothering;
            else if (action == "Escape")
                counts = client.BehavFreq.Escape;
            else if (action == "Greeting")
                counts = client.BehavFreq.Greeting;
            else if (action == "Seperation")
                counts = client.BehavFreq.Seperation;
            else if (action == "Surprise")
                counts = client.BehavFreq.Surprise;
            else if (action == "SocialCall")
                counts = client.BehavFreq.SocialCall;
            else if (action == "Caress")
                counts = client.BehavFreq.Caress;
            else
                counts = 0;

            return counts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        internal int GetFreq(string a, string b, string c)
        {
            //check the freq of specific action
            int counts = GetFreq(a)+GetFreq(b)+GetFreq(c);

            return counts;
        }

        /// <summary>
        /// Get the freq for petted
        /// Input:if true, get the pet state for now,
        /// otherwise, get the past pet freq
        /// </summary>
        /// <param name="now"> 
        /// </param>
        /// <returns></returns>
        internal int GetFreq(bool now)
        {
            int counts;
            //check the freq of specific action
            if (now is true)
            {
                counts = client.PetFreq.PetNow;
            }
            else
            {
                counts = client.PetFreq.PetPast;
            }
                
            return counts;
        }

        /// <summary>
        /// Send command to Python through TCP socket
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(string data)
        {
            return client.SendData(data);
        }



    }
}




