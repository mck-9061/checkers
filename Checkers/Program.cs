using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers {
    static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // Network testing
            try {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting......");
            
                tcpclnt.Connect("192.168.1.106",8001);
                // use the ipaddress as in the server program
            
                Console.WriteLine("Connected");
                Console.WriteLine("Please wait for a second player to connect.");

                Stream stm = tcpclnt.GetStream();
                byte[] bb=new byte[100];
                int k=stm.Read(bb,0,100);

                String received = "";
            
                for (int i=0;i<k;i++) received += (Convert.ToChar(bb[i]));

                Console.WriteLine(received);


                tcpclnt.Close();
            }
        
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("There might not be a server running.");
            }



            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
