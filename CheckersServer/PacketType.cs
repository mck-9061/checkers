using System;
using System.Collections.Generic;
using System.Text;

namespace CheckersServer {
    enum PacketType {
        PLAYER,
        MOVE,
        ERROR,
        END,
        MOVELIST,
        REMOVE
    }
}
