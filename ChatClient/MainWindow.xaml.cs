using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        public MainWindow()
        {
            InitializeComponent();
            txtboxUser.Focus();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(txtboxInput.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
            txtboxInput.Text = "";
            txtboxInput.Focus();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            readData = "Connected to Chat Server ...";
            msg();
            clientSocket.Connect("127.0.0.1", 8888);
            serverStream = clientSocket.GetStream();
            
            byte[] outStream = Encoding.ASCII.GetBytes(txtboxUser.Text + "$");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            Thread ctThread = new Thread(getMessage);
            ctThread.Start();
            txtboxUser.IsEnabled = false;
            btnConnect.IsEnabled = false;
            txtboxInput.IsEnabled = true;
            btnSend.IsEnabled = true;
            txtboxInput.Focus();




        }

        /// <summary>
        /// Listen message form the server side
        /// </summary>
        private void getMessage()
        {
            while(true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                buffSize = clientSocket.ReceiveBufferSize;
                byte[] inStream = new byte[buffSize];
                Console.WriteLine(buffSize);
                //int numberOfBytes = serverStream.Read(inStream, 0, buffSize);
                string returndata = Encoding.ASCII.GetString(inStream, 0, serverStream.Read(inStream, 0, buffSize));
                Console.WriteLine(returndata);
                readData = "" + returndata;
                msg();
            }
        }

        private void msg()
        {
            txtblockMessage.Dispatcher.Invoke(new Action(updateText));
        }

        private void updateText()
        {
            txtblockMessage.Text = txtblockMessage.Text + Environment.NewLine + " >> " + readData;
            messageScrollviewer.ScrollToBottom();
        }
    }
}
