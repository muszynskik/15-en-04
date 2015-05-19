using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        byte[] bytes = new byte[1024];
        Socket senderSock;

        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Create one SocketPermission for socket access restrictions 
                SocketPermission permission = new SocketPermission(
                    NetworkAccess.Connect,    // Connection permission 
                    TransportType.Tcp,        // Defines transport types 
                    "",                       // Gets the IP addresses 
                    SocketPermission.AllPorts // All ports 
                    );

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance            
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost 
                IPAddress ipAddr = ipHost.AddressList[0];

                // Creates a network endpoint 
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 6666);

                // Create one Socket object to setup Tcp connection 
                senderSock = new Socket(
                    ipAddr.AddressFamily,// Specifies the addressing scheme 
                    SocketType.Stream,   // The type of socket  
                    ProtocolType.Tcp     // Specifies the protocols  
                    );

                senderSock.NoDelay = false;   // Using the Nagle algorithm 

                // Establishes a connection to a remote host 
                senderSock.Connect(ipEndPoint);
                label1.Text = "Socket connected to " + senderSock.RemoteEndPoint.ToString();

                //Connect_Button.IsEnabled = false;
                button2.Enabled = true;
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); 
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Sending message 
                XmlDocument registerMsg = new XmlDocument();
                registerMsg.Load("xml/registerMsg.xml");
                XmlNode node = registerMsg.SelectSingleNode("//Type");
                node.InnerText = "TaskManager";
                //TESTING
                //registerMsg.Save("blabla.xml");
                string theMessageToSend = GetXMLAsString(registerMsg);
                byte[] msg = Encoding.Unicode.GetBytes(theMessageToSend);

                // Sends data to a connected Socket. 
                int bytesSend = senderSock.Send(msg);

                ReceiveDataFromServer();

            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }

        }
        public string GetXMLAsString(XmlDocument myxml)
        {
            return myxml.OuterXml;
        }

        private void ReceiveDataFromServer()
        {
            try
            {
                // Receives data from a bound Socket. 
                int bytesRec = senderSock.Receive(bytes);

                // Converts byte array to string 
                //String theMessageToReceive = Encoding.Unicode.GetString(bytes, 0, bytesRec);
                String theMessageToReceive = Encoding.ASCII.GetString(bytes);

                // Continues to read the data till data isn't available 
                while (senderSock.Available > 0)
                {
                    bytesRec = senderSock.Receive(bytes);
                    theMessageToReceive += Encoding.Unicode.GetString(bytes, 0, bytesRec);
                }

                richTextBox1.Text = "The server reply: " + theMessageToReceive;
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }
        public void msg(string mesg)
        {

            richTextBox1.Text = richTextBox1.Text + Environment.NewLine + " >> " + mesg;

        }
    }
}
