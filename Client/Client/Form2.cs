using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat
{
    partial class Form2 : Form
    {
        private Client c;
        delegate void SetTextCall();
        Form1 f;

        public Form2(Client c, Form1 f)
        {
            InitializeComponent();
            label3.Hide();
            textPwd2.Hide();
            register.Hide();
            login.Hide();
            back.Hide();
            this.c = c;
            this.f = f;
            //this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            //f.Show();
        }

        private void signup_Click(object sender, EventArgs e)
        {
            back.Show();
            signin.Hide();
            signup.Hide();
            label3.Show();
            textPwd2.Show();
            register.Show();
        }


        //Inscription : Envoi un message au serveur pour s'enregister avec les infos du mec
        private void register_Click(object sender, EventArgs e)
        {
            if (!textUser.Text.Equals("") && !textPwd.Text.Equals("") && !textPwd2.Text.Equals("")) //Champs non vides
                if(textPwd.Text.Equals(textPwd2))       //Mdp = Confirmation Mdp
                {
                    Message message = new Message(new List<string>() { "signup", textUser.Text, textPwd.Text }, null, Client.id);
                    c.send(message);
                }
        }


        //Connection : Envoi un message au serveur pour se connecter avec les infos du mec
        private void signin_Click(object sender, EventArgs e)
        {
            if (!textUser.Text.Equals("") && !textPwd.Text.Equals("")) //Champs non vides
            {
                Message message = new Message(new List<string>() { "signin", textUser.Text, textPwd.Text }, null, Client.id);
                c.send(message);
            }
        }

        public void errorMessage()
        {
            if (this.login.InvokeRequired)
            {
                SetTextCall d = new SetTextCall(errorMessage);  //Thread safe : ajout room à listbox
                this.Invoke(d);
            }
            else
            {
                login.Show();
            }
        }

        private void back_Click(object sender, EventArgs e)
        {
            back.Hide();
            signin.Show();
            signup.Show();
            label3.Hide();
            textPwd2.Hide();
            register.Hide();
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void Form2_FormClosing(Object sender, FormClosingEventArgs e)
        {
            //Application.Exit();
            //Application.Run(f);
            //f.Show();
            //Thread thread = new Thread(f.Show);
            //thread.Start();
            //while (true) { };
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(null);
            form1.Show();
        }
    }
}