using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using CheckersServer.CheckersGame;

namespace CheckersServer {
    class Program {
        // Implementing a server may be the best way to make this cross-platform.
        // If the server handles everything to do with the game, making clients for the game is trivial.
        // Therefore, we need a server application that receives events from clients, handles them, then sends
        // to each client what to do because of this event.

        public static Socket p1 { get; set; }
        public static Socket p2 { get; set; }
        public static GameHandler handler { get; set; }

        static void Main(string[] args) {
            try {
                IPAddress ip = IPAddress.Parse("192.168.1.106");
                
                TcpListener listener = new TcpListener(ip,8001);

                listener.Start();
        
                Console.WriteLine("The server is running at port 8001...");    
                Console.WriteLine("The local End point is " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for player 1 connection...");
        
                p1 = listener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + p1.RemoteEndPoint);

                Console.WriteLine("Waiting for player 2 connection...");
        
                p2 = listener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + p2.RemoteEndPoint);


                // "Packets" will be sent between the server and the clients with the format "HEADER-info"
                // The header says what the packet's intent is and the info says what to do.
                // For example, if the server sent the client a "MOVE-23-34" packet, the client would move the piece in slot 23 to slot 34.
                // See packets.md for full packet documentation.


                Packet packet = new Packet(p1, PacketType.PLAYER, "-one");
                packet.Send();
                Console.WriteLine("Sent player-one to Player 1");

                Packet packet2 = new Packet(p2, PacketType.PLAYER, "-two");
                packet2.Send();
                Console.WriteLine("Sent player-two to Player 2");
        
                
                listener.Stop();

                handler = new GameHandler();
                PacketListener p1listener = new PacketListener(p1);
                PacketListener p2listener = new PacketListener(p2);


                p1listener.beginListening();
                p2listener.beginListening();

            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }    
        }
    }
}
