using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckersServer.CheckersGame;

namespace CheckersServer {
    class PacketListener {
        // This class listens for packets sent by a client on a separate thread.
        public Socket socket { get; set; }

        public PacketListener(Socket socket) {
            this.socket = socket;
        }

        public void beginListening() {
            // Begin listening for packets from the Socket.
            new Task(listen).Start();
        }

        public void listen() {
            while (true) {
                byte[] buffer = new byte[100];
                int k = socket.Receive(buffer);
                string received = "";

                for (int i = 0; i < k; i++) received += (Convert.ToChar(buffer[i]));
                string[] args = received.Split("-");

                Console.WriteLine($"Received a packet from {socket.LocalEndPoint}: {received}");

                try {
                    if (args[0] == "CLICK") {
                        Color color = Color.FromName(args[2]);
                        Program.handler.handleClick(Convert.ToInt32(args[1]), color, Convert.ToInt32(args[3]));
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
    }
}
