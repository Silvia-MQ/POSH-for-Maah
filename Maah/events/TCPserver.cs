using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TCPServer
{
    class TCPserver
    {
        /// <summary>
        /// Set up a Socket object
        /// Use Connect() method trough the endPoint object, to request connection
        /// Send message trough Send()method, and recevie message trough Receive()method
        /// Close connection after use
        /// </summary>
        static Socket ClientSocket;
        static void TCPClient(string[] args)
        {
            String IP = "127.0.0.1";
            int port = 8885;

            IPAddress ip = IPAddress.Parse(IP);  
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // instantiate by ip and port
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            //Set up Connetion
            ClientSocket.Connect(endPoint); 


            Console.WriteLine("Type message:");
            var a = Console.ReadLine();

            //Change message format into Bytes
            byte[] message = Encoding.ASCII.GetBytes(a);  
            ClientSocket.Send(message);
            Console.WriteLine("Data Sent:" + Encoding.ASCII.GetString(message));
            byte[] receive = new byte[1024];

            //Legnth of the recevied data
            int length = ClientSocket.Receive(receive);  
            Console.WriteLine("Data Recieved:" + Encoding.ASCII.GetString(receive));

            //Close Connection
            ClientSocket.Close();  
        }
    }
}