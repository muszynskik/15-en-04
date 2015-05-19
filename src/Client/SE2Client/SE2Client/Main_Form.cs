using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Windows.Forms.Design;



namespace SE2Client
{
	public partial class Main_Form : Form
	{
		byte[] bytes = new byte[1024];
		Socket senderSock;
		
		public Main_Form( )
		{
			InitializeComponent( );
		}

		private void Connect_Button_Click( object sender, EventArgs e )
		{
			try
			{
				/// Create one SocketPermission for socket access restrictions 
				SocketPermission permission = new SocketPermission(
					NetworkAccess.Connect,    /// Connection permission 
					TransportType.Tcp,        /// Defines transport types 
					"",                       /// Gets the IP addresses 
					SocketPermission.AllPorts /// All ports 
					);

				/// Ensures the code to have permission to access a Socket 
				permission.Demand( );

				/// Resolves a host name to an IPHostEntry instance            
				IPHostEntry ipHost = Dns.GetHostEntry( IP_TextBox.Text );

				/// Gets first IP address associated with a localhost 
				IPAddress ipAddr = ipHost.AddressList[0];

				/// Creates a network endpoint 
				IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 6666);

				/// Create one Socket object to setup Tcp connection 
				senderSock = new Socket(
					ipAddr.AddressFamily,/// Specifies the addressing scheme 
					SocketType.Stream,   /// The type of socket  
					ProtocolType.Tcp     /// Specifies the protocols  
					);

				senderSock.NoDelay = false;   /// Using the Nagle algorithm 

				/// Establishes a connection to a remote host 
				senderSock.Connect(ipEndPoint);
				IP_TextBox.Text = "Socket connected to " + senderSock.RemoteEndPoint.ToString( );

				///Connect_Button.IsEnabled = false;
				//button2.Enabled = true;
			}
			catch ( Exception ex )
			{
				MessageBox.Show( ex.ToString( ) );
			}
		}
		
		public string GetXMLAsString( XmlDocument myxml )
		{
			return myxml.OuterXml;
		}


		/// <summary>
		/// Receives the data from the server in bytes form.
		/// </summary>
		private void _ReceiveData( )
		{
			try
			{
				/// Receives data from a bound Socket. 
				int bytesRec = senderSock.Receive(bytes);

				/// Converts byte array to string 
				/// String theMessageToReceive = Encoding.Unicode.GetString(bytes, 0, bytesRec);
				String theMessageToReceive = Encoding.ASCII.GetString(bytes);

				/// Continues to read the data till data isn't available 
				while ( senderSock.Available > 0 )
				{
					bytesRec = senderSock.Receive(bytes);
					theMessageToReceive += Encoding.Unicode.GetString(bytes, 0, bytesRec);
				}

				IP_TextBox.Text = "The server reply: " + theMessageToReceive;

			}
			catch ( Exception ex )
			{
				MessageBox.Show( ex.ToString( ) );
			}
		} /// end of private void _ReceiveData( )


		public void msg( string mesg )
		{

			IP_TextBox.Text = IP_TextBox.Text + Environment.NewLine + " >> " + mesg;

		}

		private void SendRequest_Button_Click( object sender, EventArgs e )
		{
			try
			{
				/// Sending message 
				XmlDocument registerMsg = new XmlDocument( );
				registerMsg.Load("xml/registerMsg.xml");
				XmlNode node = registerMsg.SelectSingleNode("//Type");
				node.InnerText = "TaskManager";
				/// TESTING
				/// registerMsg.Save("blabla.xml");
				string theMessageToSend = GetXMLAsString(registerMsg);
				byte[] msg = Encoding.Unicode.GetBytes(theMessageToSend);

				/// Sends data to a connected Socket. 
				int bytesSend = senderSock.Send(msg);

				_ReceiveData( );
				///bytes [i]

				PictureBox point = new PictureBox( );
				ToolTip toolTip = new ToolTip( );
                string s = "";
				/// show the path
				int x = 0;
				for ( int i=0; i<1024; i++ )
				{
					if ( bytes[i].ToString( ) != "!" )
						x++;
                }
                for (int i = 0; i <x; i++)
                {
                    s +=bytes[i].ToString();
                    MessageBox.Show( ( (char)Convert.ToInt32( bytes[i] ) ).ToString( ) );
					point = new PictureBox( );
					point.Size = new System.Drawing.Size(30, 37);
					point.BackgroundImage = global::SE2Client.Properties.Resources.unnamed1;
					(( Bitmap )(point.BackgroundImage)).MakeTransparent(Color.White);
					point.BackgroundImageLayout = ImageLayout.Zoom;
					point.Location = new Point( 20*(i+1), 40*(i+1) );
					toolTip = new ToolTip( );
                    toolTip.SetToolTip(point, "asdfsadf");

					this.Controls.Add( point );

					// the arrow: global::SE2Client.Properties.Resources.arrow_27;
				}
                MessageBox.Show(s);
			}
			catch ( Exception ex )
			{
				MessageBox.Show(ex.ToString( ));
			}

		}
	}
}
