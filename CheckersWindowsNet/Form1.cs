using Accessibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheckersServer;

namespace CheckersWindowsNet {
    public partial class Form1 : Form {

        private int player;
        private List<int> kings;

        public Form1(int player) {
            this.player = player;
            kings = new List<int>();
            InitializeComponent();
            new Task(listen).Start();
            _ = Run();
        }

        private async Task Run() {
            await Task.Delay(200);
            initBoard();

            foreach (Button button in tableLayoutPanel1.Controls) {
                button.Click += button_Click;
            }
        }

        private void listen() {
            byte[] bb;

            while (true) {
                bb = new byte[100];
                Stream stm = Program.client.GetStream();
                

                int k = stm.Read(bb, 0, 100);

                String received = "";

                for (int i = 0; i < k; i++) received += (Convert.ToChar(bb[i]));


                string[] args = received.Split("-");

                switch (args[0]) {
                    case "PLAYER":
                        onPlayer(Convert.ToInt32(args[1]));
                        break;
                    case "MOVE":
                        onMove(Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                        break;
                    case "ERROR":
                        onError(Convert.ToInt32(args[1]));
                        break;
                    case "END":
                        onEnd(Convert.ToInt32(args[1]));
                        break;
                    case "MOVELIST":
                        List<int> moves = new List<int>();
                        foreach (string arg in args) {
                            if (arg != "MOVELIST" && arg != "") {
                                moves.Add(Convert.ToInt32(arg));
                            }
                        }

                        onMovelist(moves);
                        break;
                    case "REMOVE":
                        onRemove(Convert.ToInt32(args[1]));
                        break;
                    case "KING":
                        onKing(Convert.ToInt32(args[1]));
                        break;
                }

                Application.DoEvents();
            }
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

                    if (buttonNum % 2 != 0 && !even && buttonNum < 25) control.BackColor = Color.Wheat;
                    else if (buttonNum % 2 == 0 && even && buttonNum < 25) control.BackColor = Color.Wheat;

                    else if (buttonNum % 2 != 0 && !even && buttonNum > 40) control.BackColor = Color.Brown;
                    else if (buttonNum % 2 == 0 && even && buttonNum > 40) control.BackColor = Color.Brown;


                    else if (buttonNum % 2 != 0 && !even) control.BackColor = Color.Green;
                    else if (buttonNum % 2 == 0 && even) control.BackColor = Color.Green;
                    else control.BackColor = Color.White;
                }
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void button_Click(object sender, EventArgs e) {
            // Send a CLICK, SKIP or CONCEDE packet here.
            Button button = (Button) sender;
            int num = Convert.ToInt32(((Button)sender).Name.Split("n")[1]);
            if (num < 64) {
                ClientPacket packet = new ClientPacket(ClientPacketType.CLICK, Program.client.GetStream(), "-"+player+"-"+button.BackColor.Name+"-"+num);
                packet.Send();
            }
        }



        // Methods to handle packets received by the server.
        private void onPlayer(int player) {
            this.player = player;
        }

        private void onMove(int f, int t) {
            Button from = null;
            Button to = null;

            foreach (Button button in tableLayoutPanel1.Controls) {

                try {
                    if (button.Name.Split("n")[1] == Convert.ToString(f)) from = button;
                    if (button.Name.Split("n")[1] == Convert.ToString(t)) to = button;

                    if (!kings.Contains(Convert.ToInt32(button.Name.Split("n")[1]))) button.Text = "";
                } catch (Exception e) {
                }
            }

            if (from == null || to == null) return;

            bool isKing = from.Text == "King";
            Color color = from.BackColor;

            to.BackColor = color;
            if (isKing) to.Text = "King";

            from.BackColor = Color.Green;
            from.Text = "";
        }


        private void onError(int code) {
            switch (code) {
                case 0:
                    MessageBox.Show("Unknown error!");
                    break;
                case 1:
                    MessageBox.Show("It's not your turn!");
                    break;
                default:
                    break;
            }
        }


        private void onEnd(int won) {
            if (won == 0) MessageBox.Show("Tie!");
            else if (won == player) MessageBox.Show("You win!");
            else MessageBox.Show("You lose!");
        }


        private void onMovelist(List<int> moves) {
            try {
                foreach (Button button in tableLayoutPanel1.Controls) {
                    if (!kings.Contains(Convert.ToInt32(button.Name.Split("n")[1]))) button.Text = "";

                    if (moves.Contains(Convert.ToInt32(button.Name.Split("n")[1]))) button.Text = "Move here";
                }
            } catch (Exception e) {
                MessageBox.Show(e.StackTrace);
            }
        }


        private void onRemove(int r) {
            Button remove = null;

            foreach (Button button in tableLayoutPanel1.Controls) {

                if (button.Name.Split("n")[1] == Convert.ToString(r)) remove = button;
            }

            if (remove == null) {
                MessageBox.Show("null");
                return;
            }

            remove.BackColor = Color.Green;
            remove.Text = "";
        }

        private void onKing(int king) {
            kings.Add(king);

            foreach (Button button in tableLayoutPanel1.Controls) {
                if (button.Name.Split("n")[1] == Convert.ToString(king)) button.Text = "King";
            }
        }
    }
}
