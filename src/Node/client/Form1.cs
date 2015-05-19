using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Xml;
using System.Net;
using System.IO;
using UCCTaskSolver;

namespace client
{
    public partial class Form1 : Form
    {
        List<Store> listOfStores = new List<Store>();
        List<Store> storeList = new List<Store>();
        byte[] bytes = new byte[1024];
        Socket senderSock;
        List<int> IdStoreList = new List<int>();
        List<int> DemandSection = new List<int>();
        List<int> DurationSection = new List<int>();
        List<Point> LocationCoordSection = new List<Point>();
        double timeout;
        int counter;
        Store store = new Store();

        public Form1()
        {
            InitializeComponent();

            calculateTBox.Enabled = false;
            calcOurBtn.Enabled = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Sending message 
                XmlDocument registerMsg = new XmlDocument();
                registerMsg.Load("xml/registerMsg.xml");
                XmlNode node = registerMsg.SelectSingleNode("//Type");
                node.InnerText = "Node";
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

                textBox1.Text = "The server reply: " + theMessageToReceive;
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }
        }
        public void msg(string mesg)
        {

            textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + mesg;

        }


        private void Connect_Click(object sender, EventArgs e)
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
                IPHostEntry ipHost = Dns.GetHostEntry("192.168.143.139");

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
                button1.Enabled = true;
            }
            catch (Exception exc) { MessageBox.Show(exc.ToString()); }

        }

        private void calculateTBox_Click(object sender, EventArgs e)
        {
            var instance = new Algorithm(listOfStores, 9999);
            instance.SolveTsp();
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            //theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                calculateTBox.Enabled = true;
                string filename = theDialog.FileName;

                string[] filelines = File.ReadAllLines(filename);

                using (StreamReader sr = new StreamReader(File.OpenRead(filename)))
                {
                    //bool parse = false;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        if (line.StartsWith("NUM_LOCATIONS:"))
                        {
                            string[] data = line.Split(' ');
                            counter = Convert.ToInt32(data[1]);
                            MessageBox.Show(counter.ToString());

                        }
                        else if (line.StartsWith("DEMAND_SECTION"))
                        {
                            DemandSection.Add(0);
                            for (int i = 1; i < counter; i++)
                            {
                                line = sr.ReadLine();
                                string[] data = line.Split(' ');

                                DemandSection.Add(Convert.ToInt32(data[2]));
                            }
                        }
                        else if (line.StartsWith("LOCATION_COORD_SECTION"))
                        {
                            for (int i = 0; i < counter; i++)
                            {
                                line = sr.ReadLine();
                                string[] data = line.Split(' ');

                                IdStoreList.Add(Convert.ToInt32(data[2]));
                                LocationCoordSection.Add(new Point(Convert.ToInt32(data[3]), Convert.ToInt32(data[4])));
                            }
                        }
                        else if (line.StartsWith("DURATION_SECTION"))
                        {
                            DurationSection.Add(0);
                            for (int i = 1; i < counter; i++)
                            {
                                line = sr.ReadLine();
                                string[] data = line.Split(' ');

                                DurationSection.Add(Convert.ToInt32(data[2]));
                            }
                        }
                    }

                }
            }
            CreateStoreList();
        }


        private void CreateStoreList()
        {
            for (int i = 0; i < counter; i++)
            {
                store.id = IdStoreList.ElementAt(i);
                store.coordinate = LocationCoordSection.ElementAt(i);
                store.demand = DemandSection.ElementAt(i);
                store.duration = DurationSection.ElementAt(i);

                listOfStores.Add(store);
                listBox1.Items.AddRange(new object[] 
                {
                    "id: " + store.id, "coord: " + store.coordinate, "duration: " + store.duration, "demand: " + store.demand
                });
                //listBox1.Items.Add(store.ToString());
            }
        }

        private void calcOurBtn_Click(object sender, EventArgs e)
        {
            var instance = new Algorithm(storeList, 9999);
            double kk = instance.SolveTsp();
            MessageBox.Show(kk.ToString());
        }

        private void loadOurBtn_Click(object sender, EventArgs e)
        {
            //List<Store> storeList = new List<Store>();
            Store store1 = new Store();
            store1.id = 1;
            store1.coordinate = new Point(14, 46);
            store1.demand = 20;
            store1.duration = 0;
            storeList.Add(store1);
            Store store2 = new Store();
            store2.id = 2;
            store2.coordinate = new Point(47, 6);
            store2.demand = 20;
            store2.duration = 0;
            storeList.Add(store2);
            Store store3 = new Store();
            store3.id = 3;
            store3.coordinate = new Point(140, 26);
            store3.demand = 20;
            store3.duration = 0;
            storeList.Add(store3);

            listBox1.Items.AddRange(new object[] 
                {
                    "id: " + store1.id, "coord: " + store1.coordinate, "duration: " + store1.duration, "demand: " + store1.demand
                });
            listBox1.Items.AddRange(new object[] 
                {
                    "id: " + store2.id, "coord: " + store2.coordinate, "duration: " + store2.duration, "demand: " + store2.demand
                });

            listBox1.Items.AddRange(new object[] 
                {
                    "id: " + store3.id, "coord: " + store3.coordinate, "duration: " + store3.duration, "demand: " + store3.demand
                });

            calcOurBtn.Enabled = true;

        }
    }
    
}
