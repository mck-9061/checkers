using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accessibility;

namespace Checkers {
    public partial class Form1 : Form {
        private int playerTurn = 1;

        private List<Piece> pieces = new List<Piece>();

        private Piece aboutToMove;

        private Button oldButton;

        private int brownTaken = 0;
        private int whiteTaken = 0;

        public Form1() {
            InitializeComponent();
            _ = Run();
        }

        private async Task Run() {
            await Task.Delay(200);

            initBoard();

            // Draw initial pieces
            int i = -1;
            bool even = true;



            // Generate random board for testing purposes
            Random random = new Random();

            List<int> possiblePlaces = new List<int>();

            bool e2 = true;
            int i3 = -1;

            for (int i2 = 1; i2 < 65; i2++) {
                i3++;
                if (i3 == 8) {
                    e2 = !e2;
                    i3 = 0;
                }

                if ((i2 % 2 == 0 && e2) || (i2 % 2 != 0 && !e2)) possiblePlaces.Add(i2);
            }

            foreach (int j in possiblePlaces) Console.WriteLine(j);




            for (int num = 1; num < 25; num++) {
                i++;
                if (i == 8) {
                    even = !even;
                    i = 0;
                }

                if ((num % 2 == 0 && even) || (num % 2 != 0 && !even)) {

                    int place = possiblePlaces[random.Next(possiblePlaces.Count)];
                    possiblePlaces.Remove(place);

                    Piece piece = new Piece(Color.Wheat, num);
                    pieces.Add(piece);
                    drawPiece(piece);
                }
            }




            i = -1;
            even = false;

            Piece piece43 = null;

            for (int num = 41; num < 65; num++) {
                i++;
                if (i == 8) {
                    even = !even;
                    i = 0;
                }

                

                if ((num % 2 == 0 && even) || (num % 2 != 0 && !even)) {

                    int place = possiblePlaces[random.Next(possiblePlaces.Count)];
                    possiblePlaces.Remove(place);

                    Piece piece = new Piece(Color.Brown, num);
                    pieces.Add(piece);
                    drawPiece(piece);

                    if (num == 43) piece43 = piece;
                }
            }


            foreach (Button button in tableLayoutPanel1.Controls) {
                button.Click += button_Click;
            }

            foreach (Button button in Controls) {
                button.Click += button_Click;
            }
        }


        private void drawPiece(Piece piece) {
            int buttonNum = piece.buttonNum;
            Button button = (Button) tableLayoutPanel1.Controls.Find("button" + buttonNum, true)[0];
            button.BackColor = piece.color;
            if (piece.king) button.Text = "King";
        }




        private void initBoard() {
            int iter = -1;
            bool even = false;
            foreach (Control control in tableLayoutPanel1.Controls) {
                iter++;
                if (iter == 8) {
                    even = !even;
                    iter = 0;
                }
                if (control.Name.Contains("button")) {
                    control.Text = "";
                    int buttonNum = Convert.ToInt32(control.Name.Split("n")[1]);
                    if (buttonNum % 2 != 0 && !even) control.BackColor = Color.Green;
                    else if (buttonNum % 2 == 0 && even) control.BackColor = Color.Green;
                }
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void button_Click(object sender, EventArgs e) {
            Button button = (Button) sender;

            if ((button.BackColor == Color.Brown && playerTurn == 1) ||
                (button.BackColor == Color.Wheat && playerTurn == 2)) {
                int num = Convert.ToInt32(button.Name.Split("n")[1]);

                Piece piece = null;
                foreach (Piece p in pieces) {
                    if (p.buttonNum == num) {
                        piece = p;
                        break;
                    }
                }

                if (piece == null) return;

                foreach (Button b in tableLayoutPanel1.Controls) {
                    if (b.Text != "King") b.Text = "";
                }

                List<Move> moves = piece.getMoves(pieces);
                foreach (Move move in moves) {
                    string buttonName = "button" + move.moveTo;
                    Button moveTo = (Button) tableLayoutPanel1.Controls.Find(buttonName, true)[0];
                    moveTo.Text = "Move here";
                }

                aboutToMove = piece;
                oldButton = button;
            }
            else if (button.Text == "Move here") {
                Piece piece = aboutToMove;
                int num = Convert.ToInt32(button.Name.Split("n")[1]);

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
                    Button b = (Button) tableLayoutPanel1.Controls.Find("button" + taken.buttonNum, true)[0];
                    b.BackColor = Color.Green;

                    if (playerTurn == 1) brownTaken++;
                    else if (playerTurn == 2) whiteTaken++;

                    label3.Text = $"Brown has captured {brownTaken} of White's pieces.";
                    label2.Text = $"White has captured {whiteTaken} of Brown's pieces.";


                }


                // Check if the piece should be kinged
                if (piece.color == Color.Brown && num < 9) piece.king = true;
                if (piece.color == Color.Wheat && num > 56) piece.king = true;


                oldButton.BackColor = Color.Green;
                oldButton.Text = "";
                foreach (Button b in tableLayoutPanel1.Controls) {
                    if (b.Text != "King") b.Text = "";
                }


                // Move the piece
                piece.buttonNum = num;
                drawPiece(piece);
                aboutToMove = null;





                if (playerTurn == 1) {
                    playerTurn = 2;
                    label1.Text = "It is White's turn.";
                }
                else if (playerTurn == 2) {
                    playerTurn = 1;
                    label1.Text = "It is Brown's turn.";
                }
            }
            else if (button.Text == "Skip Move") {
                if (playerTurn == 1) {
                    playerTurn = 2;
                    label1.Text = "It is White's turn.";
                }
                else if (playerTurn == 2) {
                    playerTurn = 1;
                    label1.Text = "It is Brown's turn.";
                }
            }
            else if (button.Text == "Concede Match") {
                if (playerTurn == 1) {
                    MessageBox.Show("White wins!");
                }
                else if (playerTurn == 2) {
                    MessageBox.Show("Brown wins!");
                }

                this.Close();
            }
        }
    }
}
