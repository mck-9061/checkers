using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CheckersMobile2 {
    class ClientPacket {
        public ClientPacketType type;
        public Stream destination;
        public String contents;

        public ClientPacket(ClientPacketType type, Stream destination, string contents) {
            this.type = type;
            this.destination = destination;
            this.contents = contents;
        }

        public void Send() {
            string packet = type + contents;

            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] buffer = asen.GetBytes(packet);
            destination.Write(buffer, 0, buffer.Length);
        }
    }
}
