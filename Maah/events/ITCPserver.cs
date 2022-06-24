using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using POSH.sys.exceptions;

namespace ITCPserver
{
    public interface ITCPserver
    {
        /// <summary>
        /// Set up a Socket object
        /// Use InitTCP() method to request connection
        /// Get brhaviour tree with getBehaviour()
        /// Send message trough sendData()method
        /// </summary>
        static Socket ClientSocket;
        
        /// <summary>
        /// Initialise a TCP connection
        /// </summary>
        static void InitTCP()
        {
            String IP = "127.0.0.1";
            int port = 8885;

            IPAddress ip = IPAddress.Parse(IP);  
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // instantiate by ip and port
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            //Set up Connetion
            ClientSocket.Connect(endPoint);
        }

        /// <summary>
        /// Get behaviour tree from python
        /// </summary>
        /// <returns>
        /// String of Behaviour list
        /// </returns>
        static string getBehaviour()
        {
            //Change message format into Bytes
            byte[] message = Encoding.ASCII.GetBytes("Behaviour tree required");
            ClientSocket.Send(message);
            Console.WriteLine("Data Sent:" + Encoding.ASCII.GetString(message));
            byte[] receive = new byte[1024];
            int length = ClientSocket.Receive(receive);

            if (length != 0)
                return Encoding.ASCII.GetString(receive);
            else
                throw new NameException(string.Format("Fail to get behaviour tree from python "));
        }

        /// <summary>
        /// Send data through TCP
        /// </summary>
        /// <param name="arg">
        /// The string to be sent to python, which should be the next behaviour of Maah 
        /// </param>
        /// <returns>
        /// The return data from python.
        /// Once Maah finish the behaviour, 'Done' will be sent
        /// </returns>
        static string sendData(string arg)
        {
            byte[] message = Encoding.ASCII.GetBytes(arg);
            ClientSocket.Send(message);
            Console.WriteLine("Data Sent:" + Encoding.ASCII.GetString(message));

            byte[] receive = new byte[1024];
            int length = ClientSocket.Receive(receive);
            Console.WriteLine("Data Recieved:" + Encoding.ASCII.GetString(receive));
            return receive.ToString();
        }
    }
}