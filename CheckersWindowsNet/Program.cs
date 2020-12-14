using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersWindowsNet {
    static class Program {
        public static TcpClient client;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            int player = 0;
            try {
                client = new TcpClient();
                Console.WriteLine("Connecting......");

                client.Connect("192.168.0.105", 8001);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");
                Console.WriteLine("Please wait for a second player to connect.");

                Stream stm = client.GetStream();
                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                String received = "";

                for (int i = 0; i < k; i++) received += (Convert.ToChar(bb[i]));

                Console.WriteLine(received);

                player = Convert.ToInt32(received.Split("-")[1]);

            } catch (Exception e) {
                MessageBox.Show(e.StackTrace);
                Console.WriteLine("There might not be a server running.");
            }



            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(player));
        }
    }
}
