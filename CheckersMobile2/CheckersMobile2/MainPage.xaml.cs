using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;

namespace CheckersMobile2 {
    public partial class MainPage : ContentPage {
        private int player = 0;
        private Grid grid;
        private List<int> kings = new List<int>();
        private Label info;
        private TcpClient client;
        public MainPage() {
            InitializeComponent();

            grid = (Grid) this.Content;



            bool even = true;
            int iter = -1;
            foreach (View view in grid.Children) {
                if (view is Button) {

                    Button button = (Button) view;

                    button.Clicked += button_Click;

                    iter++;
                    if (iter == 8) {
                        even = !even;
                        iter = 0;
                    }

                    
                    button.Text = "";
                    int buttonNum = Convert.ToInt32(button.ClassId.Split('n')[1]);

                    if (buttonNum % 2 != 0 && !even && buttonNum < 25) button.BackgroundColor = Color.Wheat;
                    else if (buttonNum % 2 == 0 && even && buttonNum < 25) button.BackgroundColor = Color.Wheat;

                    else if (buttonNum % 2 != 0 && !even && buttonNum > 40) button.BackgroundColor = Color.Brown;
                    else if (buttonNum % 2 == 0 && even && buttonNum > 40) button.BackgroundColor = Color.Brown;


                    else if (buttonNum % 2 != 0 && !even) button.BackgroundColor = Color.Green;
                    else if (buttonNum % 2 == 0 && even) button.BackgroundColor = Color.Green;
                    else button.BackgroundColor = Color.White;
                } else if (view is Label) {
                    Label label = (Label) view;
                    if (label.ClassId == "infolabel") info = label;
                }
            }

            // Connect to the server
            try {
                client = new TcpClient();
                Console.WriteLine("Connecting......");

                client.Connect("192.168.1.106", 8001);
                // use the ipaddress as in the server program

                info.Text = "Connected. Wait for second player.";

                new Task(listen).Start();

            } catch (Exception e) {
                info.Text = e.StackTrace;
                Console.WriteLine("There might not be a server running.");
            }

        }

        private void listen() {
            byte[] bb;

            while (true) {
                bb = new byte[100];
                Stream stm = client.GetStream();


                int k = stm.Read(bb, 0, 100);

                String received = "";

                for (int i = 0; i < k; i++) received += (Convert.ToChar(bb[i]));


                string[] args = received.Split('-');

                Device.BeginInvokeOnMainThread(() => {
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
                });
            }
        }


        // Methods to handle packets received by the server.
        private void onPlayer(int player) {
            this.player = player;
            info.Text = $"You are player {player}";
        }

        private void button_Click(object sender, EventArgs e) {
            // Send a CLICK, SKIP or CONCEDE packet here.
            Button button = (Button)sender;
            int num = Convert.ToInt32(((Button)sender).ClassId.Split('n')[1]);
            if (num < 64) {
                string color = "";
                if (button.BackgroundColor == Color.Brown) color = "Brown";
                if (button.BackgroundColor == Color.Wheat) color = "Wheat";
                if (button.BackgroundColor == Color.Green) color = "Green";
                if (button.BackgroundColor == Color.White) color = "White";


                ClientPacket packet = new ClientPacket(ClientPacketType.CLICK, client.GetStream(), "-" + player + "-" + color + "-" + num);
                packet.Send();
            }
        }

        private void onMove(int f, int t) {
            Button from = null;
            Button to = null;

            foreach (View view in grid.Children) {
                if (view is Button) {
                    Button button = (Button) view;
                    try {
                        if (button.ClassId.Split('n')[1] == Convert.ToString(f)) from = button;
                        if (button.ClassId.Split('n')[1] == Convert.ToString(t)) to = button;

                        if (!kings.Contains(Convert.ToInt32(button.ClassId.Split('n')[1]))) button.Text = "";
                    }
                    catch (Exception e) {
                    }
                }
            }

            if (from == null || to == null) return;

            bool isKing = from.Text == "K";
            Color color = from.BackgroundColor;

            to.BackgroundColor = color;
            if (isKing) to.Text = "K";

            from.BackgroundColor = Color.Green;
            from.Text = "";
        }


        private void onError(int code) {
            switch (code) {
                case 0:
                    info.Text = "Unknown error!";
                    break;
                case 1:
                    info.Text = "It's not your turn!";
                    break;
                default:
                    break;
            }
        }


        private void onEnd(int won) {
            if (won == 0) info.Text = "Tie!";
            else if (won == player) info.Text = "You win!";
            else info.Text = "You lose!";
        }


        private void onMovelist(List<int> moves) {
            //try {
                foreach (View view in grid.Children) {
                    if (view is Button) {
                        Button button = (Button) view;
                        if (!kings.Contains(Convert.ToInt32(button.ClassId.Split('n')[1]))) button.Text = "";

                        if (moves.Contains(Convert.ToInt32(button.ClassId.Split('n')[1]))) {button.Text = "M";}
                    }
                }
            //} catch (Exception e) {
                //Log.Warning("error", e.StackTrace);
            //}
        }


        private void onRemove(int r) {
            Button remove = null;

            foreach (View view in grid.Children) {
                if (view is Button) {
                    Button button = (Button) view;
                    if (button.ClassId.Split('n')[1] == Convert.ToString(r)) remove = button;
                }
            }

            if (remove == null) {
                info.Text = "null";
                return;
            }

            remove.BackgroundColor = Color.Green;
            remove.Text = "";
        }
        

        private void onKing(int king) {
            kings.Add(king);

            foreach (View view in grid.Children) {
                if (view is Button) {
                    Button button = (Button) view;
                    if (button.ClassId.Split('n')[1] == Convert.ToString(king)) button.Text = "K";
                }
            }
        }
    }
}
