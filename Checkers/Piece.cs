using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Checkers {
    class Piece {
        public Color color { get; set; }
        public int buttonNum { get; set; }
        public bool captured { get; set; }
        public bool king { get; set; }


        public Piece(Color color, int buttonNum) {
            this.color = color;
            this.buttonNum = buttonNum;
            this.captured = false;
            this.king = false;
        }

        public List<Move> getMoves(List<Piece> pieces) {
            List<Move> moves = new List<Move>();

            List<Move> checkMoves = new List<Move>();

            if (buttonNum % 8 != 1 && (color == Color.Brown || king)) checkMoves.Add(new Move(this, buttonNum - 9));
            if (buttonNum % 8 != 0 && (color == Color.Brown || king)) checkMoves.Add(new Move(this, buttonNum - 7));

            if (buttonNum % 8 != 1 && (color == Color.Wheat || king)) checkMoves.Add(new Move(this, buttonNum + 7));
            if (buttonNum % 8 != 0 && (color == Color.Wheat || king)) checkMoves.Add(new Move(this, buttonNum + 9));


            // Check that those moves are possible
            foreach (Move move in checkMoves) {
                int num = move.moveTo;
                bool canMove = num > 0 && num < 65;

                // First check the move is possible by checking that the space isn't white.
                if (canMove) {
                    Button button = (Button) Form1.ActiveForm.Controls.Find("button"+num, true)[0];
                    Console.WriteLine(num);
                    Console.WriteLine(button.BackColor.Name);
                    if (button.BackColor == Color.White) {
                        canMove = false;
                    }
                }

                foreach (Piece piece in pieces) {
                    if (!canMove) break;

                    if (piece.buttonNum == num && piece.color == color) {
                        canMove = false;
                        break;
                    } else if (piece.buttonNum == num) {
                        // The space being moved to is occupied by an enemy.
                        if (piece.color != Color.Green) {
                            int beyond;

                            if (num < this.buttonNum) beyond = piece.buttonNum - (this.buttonNum - num);
                            else beyond = piece.buttonNum + (num - this.buttonNum);


                            if (beyond <= 0 || beyond >= 65) {
                                canMove = false;
                                break;
                            }


                            Button button = (Button)Form1.ActiveForm.Controls.Find("button" + beyond, true)[0];
                            Console.WriteLine(beyond);
                            Console.WriteLine(button.BackColor.Name);
                            if (button.BackColor == Color.White) {
                                canMove = false;
                                break;
                            }

                            bool canMove2 = true;

                            foreach (Piece piece2 in pieces) {
                                if (piece2.buttonNum == beyond) {
                                    canMove2 = false;
                                    break;
                                } 
                            }

                            if (canMove2) {
                                Move move2 = new Move(this, beyond);
                                move2.piecesTaken.Add(piece);
                                moves.Add(move2);
                            }

                            canMove = false;
                            break;
                        }

                        // The space being moved to is unoccupied.
                        else break;


                    }
                }

                if (canMove) moves.Add(move);

            }


            return moves;
        }

    }
}
