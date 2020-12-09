using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers {
    class Move {
        public Piece moving { get; set; }
        public int moveTo { get; set; }
        public List<Piece> piecesTaken { get; set; }

        public Move(Piece moving, int moveTo) {
            this.moving = moving;
            this.moveTo = moveTo;

            piecesTaken = new List<Piece>();
        }
    }
}
