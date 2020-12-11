using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;

namespace CheckersServer.CheckersGame {
    class GameHandler {
        private int playerTurn = 1;

        private List<Piece> pieces = new List<Piece>();

        private Piece aboutToMove;


        private int brownTaken = 0;
        private int whiteTaken = 0;


        public void run() {
            // Create initial pieces
            int i = -1;
            bool even = true;

            for (int num = 1; num < 25; num++) {
                i++;
                if (i == 8) {
                    even = !even;
                    i = 0;
                }

                if ((num % 2 == 0 && even) || (num % 2 != 0 && !even)) {
                    Piece piece = new Piece(Color.Wheat, num);
                    pieces.Add(piece);
                }
            }




            i = -1;
            even = false;


            for (int num = 41; num < 65; num++) {
                i++;
                if (i == 8) {
                    even = !even;
                    i = 0;
                }



                if ((num % 2 == 0 && even) || (num % 2 != 0 && !even)) {
                    Piece piece = new Piece(Color.Brown, num);
                    pieces.Add(piece);
                }
            }


            // Begin to listen for client events from player 1.
        }


        public void handleClick(int player, Color color, int clicked) {
            try {

                // Make sure the correct player is clicking.

                if (player != playerTurn) {
                    Socket socket = null;
                    if (player == 1) socket = Program.p2;
                    else socket = Program.p1;


                    Packet error = new Packet(socket, PacketType.ERROR, "-1");
                    error.Send();
                    Console.WriteLine("Error packet sent");
                    return;
                }


                if ((color == Color.Brown && playerTurn == 1) ||
                    (color == Color.Wheat && playerTurn == 2)) {
                    int num = clicked;

                    Piece piece = null;
                    Console.WriteLine(num);
                    foreach (Piece p in pieces) {
                        Console.WriteLine(p.buttonNum);
                        if (p.buttonNum == num) {
                            piece = p;
                            break;
                        }
                    }

                    if (piece == null) {
                        Console.WriteLine("Piece is null");
                        return;
                    }
                    Console.WriteLine("Piece isn't null");

                    string options = "-";

                    foreach (Move move in piece.getMoves(pieces)) {
                        options += move.moveTo + "-";
                    }

                    Console.WriteLine(options);

                    Socket socket = null;
                    if (player == 1) socket = Program.p1;
                    else socket = Program.p2;

                    Packet packet = new Packet(socket, PacketType.MOVELIST, options);
                    packet.Send();
                    Console.WriteLine("Movelist sent");

                    aboutToMove = piece;

                }
                else if (color == Color.Green) {
                    Socket socket = null;
                    if (player == 1) socket = Program.p1;
                    else socket = Program.p2;

                    if (aboutToMove == null) {
                        Console.WriteLine("No packet sent; empty space clicked without picking piece to move");
                        return;
                    }

                    Piece piece = aboutToMove;
                    int num = clicked;

                    // Check for any captured pieces
                    Move move = null;
                    foreach (Move m in piece.getMoves(pieces)) {
                        if (m.moveTo == num) {
                            move = m;
                            break;
                        }
                    }

                    if (move.piecesTaken.Count != 0) {
                        Piece taken = move.piecesTaken[0];
                        pieces.Remove(taken);



                        Packet packet = new Packet(socket, PacketType.REMOVE, "-" + taken.buttonNum);
                        Console.WriteLine("Remove packet sent");


                        if (playerTurn == 1) brownTaken++;
                        else if (playerTurn == 2) whiteTaken++;
                    }


                    // Check if the piece should be kinged
                    bool king = false;
                    if (piece.color == Color.Brown && num < 9 && !piece.king) king = true;
                    if (piece.color == Color.Wheat && num > 56 && !piece.king) king = true;


                    if (king) {
                        piece.king = true;
                        Packet kingPacket = new Packet(socket, PacketType.KING, "-" + piece.buttonNum);
                        Console.WriteLine("King packet sent");
                    }



                    // Move the piece
                    Packet movePacket = new Packet(socket, PacketType.MOVE, $"-{piece.buttonNum}-{num}");
                    movePacket.Send();


                    Socket socket2 = null;
                    if (player == 1) socket2 = Program.p2;
                    else socket2 = Program.p1;

                    Packet movePacket2 = new Packet(socket2, PacketType.MOVE, $"-{piece.buttonNum}-{num}");
                    movePacket2.Send();

                    Console.WriteLine("Move packet sent");


                    piece.buttonNum = num;

                    if (playerTurn == 1) playerTurn = 2;
                    else playerTurn = 1;
                    aboutToMove = null;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return;
            }
        }
    }
}
