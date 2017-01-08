using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace chat
{
    partial class Form2 : Form
    {
        private Client c;
        private Form1 f;
        delegate void SetTextCall();
        delegate void SetTextCallback(object sender, EventArgs e);


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
            {
                if (textPwd.Text.Equals(textPwd2.Text))       //Mdp = Confirmation Mdp
                {
                    Message message = new Message(new List<string>() { "signup", textUser.Text, textPwd.Text }, null, Client.id);
                    c.send(message);
                }
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


        public void NextForm(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(NextForm);  //Thread safe : ajout texte à textbox
                this.Invoke(d, new object[] { this, null });
            }
            else
            {
                f.Show();
                this.Hide();
            }
        }

    }
}