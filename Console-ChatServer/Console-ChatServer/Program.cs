using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;

namespace Console_ChatServer
{

    class Program
    {
        public static Hashtable clientsList = new Hashtable();
        public const int ServerPort = 8888;

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), ServerPort);
            TcpClient clientSocket = default;
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Chat Server: 127.0.0.1");
            Console.WriteLine("Chat Port  : "+ ServerPort);
            Console.WriteLine("Status     : Online");
            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();

                
                string dataFromClient = "";

                NetworkStream networkStream = clientSocket.GetStream();
                byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                clientsList.Add(dataFromClient, clientSocket);

                broadcast(dataFromClient + " Joined ", dataFromClient, false);

                Console.WriteLine(dataFromClient + " Joined chat room ");
                handleClient client = new handleClient();
                client.startClient(clientSocket, dataFromClient, clientsList);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }  //end broadcast function
    }//end Main class


    public class handleClient
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;
        /// <summary>
        /// Initialize Client
        /// </summary>
        /// <param name="inClientSocket">Client Socket</param>
        /// <param name="clineNo">Client name</param>
        /// <param name="cList">Client List</param>
        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = cList;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom;
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    bytesFrom = new byte[clientSocket.ReceiveBufferSize];
                    networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    Program.broadcast(dataFromClient, clNo, true);
                }
                catch (Exception ex)
                {
                    // Console.WriteLine(ex.ToString());
                }
            }//end while
        }//end doChat
    } //end class handleClient
}//end namespace