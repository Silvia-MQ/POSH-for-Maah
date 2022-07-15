// A C# program for Client
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{

    class Program
    {
        public struct ToJsonMy
        {
            public string Fans { get; set; }  //属性的名字，必须与json格式字符串中的"key"值一样。
            public string Rapid { get; set; }
            public string boardTemperature { get; set; }
            public string Power { get; set; }
        }
        // Main Method
        static void Main(string[] args)
        {
            ExecuteClient();
        }

        // ExecuteClient() Method
        static void ExecuteClient()
        {

            try
            {

                // Establish the remote endpoint
                // for the socket. This example
                // uses port 11111 on the local
                // computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[1]; //sometimes should be ipHost.AddressList[0] //MinQ: check the difference here
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 12345);

                // Creation TCP/IP Socket using
                // Socket Class Constructor
                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    // Connect Socket to the remote
                    // endpoint using method Connect()
                    sender.Connect(localEndPoint);

                    // We print EndPoint information
                    // that we are connected
                    Console.WriteLine("Socket connected to -> {0} ",
                                  sender.RemoteEndPoint.ToString());

                    // Creation of message that
                    // we will send to Server
                    byte[] messageSent = Encoding.UTF8.GetBytes("Test Client<EOF>");
                    int byteSent = sender.Send(messageSent);

                    // Data buffer
                    byte[] messageReceived = new byte[1024];

                    // We receive the message using
                    // the method Receive(). This
                    // method returns number of bytes
                    // received, that we'll use to
                    // convert them to string
                    //int byteRecv = sender.Receive(messageReceived);
                    //Console.WriteLine("Message from Server -> {0}",
                     //     Encoding.UTF8.GetString(messageReceived,
                     //                                0, byteRecv));

                    var msg = sender.Receive(messageReceived);
                    Console.WriteLine("Origial -> {0}", Encoding.UTF8.GetString(messageReceived, 0, msg));
                    //JObject is the objected been serialized by json, in the form of string
                    //messageReceived is the bytes array, msg is the length of bytes array

                    if (Encoding.UTF8.GetString(messageReceived, 0, msg).Contains("Fans"))
                    {
                        ToJsonMy frame = JsonConvert.DeserializeObject<ToJsonMy>(Encoding.UTF8.GetString(messageReceived, 0, msg));
                        string Fans = frame.Fans; 
                        Console.WriteLine("Fans -> {0}",Fans);
                    }
                    else
                        Console.WriteLine("Message from Server -> {0}",
                            Encoding.UTF8.GetString(messageReceived, 0, msg));
                    //string message = frame.ToString();

                    Array.Clear(messageReceived, 0, msg);

                    msg = sender.Receive(messageReceived);
                    if (Encoding.UTF8.GetString(messageReceived, 0, msg).Contains("Fans"))
                    {
                        ToJsonMy frame = JsonConvert.DeserializeObject<ToJsonMy>(Encoding.UTF8.GetString(messageReceived, 0, msg));
                        string Fans = frame.Fans;
                        Console.WriteLine("Fans -> {0}", Fans);
                    }
                    else
                        Console.WriteLine("Message from Server -> {0}",
                            Encoding.UTF8.GetString(messageReceived, 0, msg));

                    // Close Socket using
                    // the method Close()
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
    }
}