using System;
using System.IO;
using System.Net.Sockets;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;

namespace CheckersAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.server);

            Button button = FindViewById<Button>(Resource.Id.button1);

            button.Click += delegate {
                try {
                    // Connect to server
                    TcpClient tcpclnt = new TcpClient();

                    tcpclnt.Connect(FindViewById<EditText>(Resource.Id.editText1).Text,
                        Convert.ToInt32(FindViewById<EditText>(Resource.Id.editText2).Text));
                    // use the ipaddress as in the server program

                    SetContentView(Resource.Layout.@out);

                    Stream stm = tcpclnt.GetStream();
                    byte[] bb = new byte[100];
                    int k = stm.Read(bb, 0, 100);

                    String received = "";

                    for (int i = 0; i < k; i++) received += (Convert.ToChar(bb[i]));

                    FindViewById<TextView>(Resource.Id.textView1).Text = received;

                    tcpclnt.Close();
                }
                catch (Exception e) {
                    SetContentView(Resource.Layout.error);
                }
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



    }
}