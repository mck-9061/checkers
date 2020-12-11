using Accessibility;
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
using CheckersServer;

namespace CheckersWindowsNet {
    public partial class Form1 : Form {

        private Button oldButton;
        private int player;
        List<int> kings = new List<int>();

        public Form1() {
            InitializeComponent();
            _ = Run();
        }

        private async Task Run() {
            await Task.Delay(200);
            initBoard();
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
                    else control.BackColor = Color.White;
                }
            }
        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void button_Click(object sender, EventArgs e) {
            // Send a CLICK, SKIP or CONCEDE packet here.
        }



        // Methods to handle packets received by the server.
        private void onPlayer(int player) {
            this.player = player;
        }

        private void onMove(int f, int t) {
            Button from = null;
            Button to = null;

            foreach (Button button in tableLayoutPanel1.Controls) {
                if (button.Name.Split("n")[0] == Convert.ToString(f)) from = button;
                if (button.Name.Split("n")[0] == Convert.ToString(t)) to = button;

                if (!kings.Contains(Convert.ToInt32(button.Name.Split("n")[0]))) button.Text = "";
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
            foreach (Button button in tableLayoutPanel1.Controls) {
                if (!kings.Contains(Convert.ToInt32(button.Name.Split("n")[0]))) button.Text = "";

                if (moves.Contains(Convert.ToInt32(button.Name.Split("n")[0]))) button.Text = "Move here";
            }
        }


        private void onRemove(int remove) {

        }
    }
}
