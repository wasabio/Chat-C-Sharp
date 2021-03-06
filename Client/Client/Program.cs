﻿using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace chat
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Client c = new Client();

            if (c.connected == true)
            {
                Form1 f = new Form1(c);
                Form2 f2 = new Form2(c, f);


                c.bind(f, f2);      //Permet au client d'acceder aux methodes de notre Form1 et Form2

                c.thread.Start();   //Lance le thread qui va ecouter le serveur, et print les messages dans Form1

                Application.Run(f2);

                c.sock.Shutdown(SocketShutdown.Both);
                c.sock.Close();
            }
        }
    }
}