using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CheckersServer {
    class Packet {
        public Socket destination { get; set; }
        public PacketType type { get; set; }
        public string contents { get; set; }


        public Packet(Socket destination, PacketType type, string contents) {
            this.destination = destination;
            this.type = type;
            this.contents = contents;
        }


        public void Send() {
            ASCIIEncoding asen = new ASCIIEncoding();
            destination.Send(asen.GetBytes(type + contents));
        }
    }
}
