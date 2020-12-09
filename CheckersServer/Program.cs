using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CheckersServer {
    class Program {
        // Implementing a server may be the best way to make this cross-platform.
        // If the server handles everything to do with the game, making clients for the game is trivial.
        // Therefore, we need a server application that receives events from clients, handles them, then sends
        // to each client what to do because of this event.


        static void Main(string[] args) {
            try {
                IPAddress ip = IPAddress.Parse("192.168.1.106");
                
                TcpListener listener = new TcpListener(ip,8001);

                listener.Start();
        
                Console.WriteLine("The server is running at port 8001...");    
                Console.WriteLine("The local End point is " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for player 1 connection...");
        
                Socket p1 = listener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + p1.RemoteEndPoint);

                Console.WriteLine("Waiting for player 2 connection...");
        
                Socket p2 = listener.AcceptSocket();
                Console.WriteLine("Connection accepted from " + p2.RemoteEndPoint);


                // "Packets" will be sent between the server and the clients with the format "HEADER-info"
                // The header says what the packet's intent is and the info says what to do.
                // For example, if the server sent the client a "MOVE-23-34" packet, the client would move the piece in slot 23 to slot 34.
                // See packets.md for full packet documentation.

                ASCIIEncoding asen = new ASCIIEncoding();
                p1.Send(asen.GetBytes("PLAYER-one"));
                Console.WriteLine("Sent game-start to Player 1");

                p2.Send(asen.GetBytes("PLAYER-two"));
                Console.WriteLine("Sent player-two to Player 2");
        
                
                listener.Stop();
            
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }    
        }
    }
}
