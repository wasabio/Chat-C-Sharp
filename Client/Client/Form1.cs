using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace chat
{
    partial class Form1 : Form
    {
        private Client c;
        private int previousIndex = 0;

        delegate void SetTextCallback(object sender, Message m);

        public Form1(Client c)
        {
            InitializeComponent();
            AcceptButton = button1;
            this.c = c;
        }

        private void texte_TextChanged(object sender, EventArgs e)
        {

        }

        private void discussion_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)        //Envoie de message
        {
            if (!texte.Text.Equals(""))
            {
                string room = listBox1.GetItemText(listBox1.SelectedItem);
                Message message = new Message(new List<string>() { texte.Text },  room, Client.id);   //go to welcome room
                string text = "[You] " + message.Content[0] + "\r\n";
                //int id = Room_client.getIndex(room);

                discussion.AppendText(text);  //Affichage dans la textbox
                //Client.rooms[id].add(text);   //Sauvegarde discussion dans la memoire 

                c.send(message);
                texte.Clear();
                
                clearDiscussion();
            }            
        }

        public void Afficher(object sender, Message m)
        {
            if (this.discussion.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Afficher);  //Thread safe : ajout texte à textbox
                this.Invoke(d, new object[] { this, m });
            }
            else
            {
                string text = m.Content[0] + "\r\n";
                string room = listBox1.GetItemText(listBox1.SelectedItem);
                
                if (m.Room == room)
                {
                    discussion.AppendText(text);  //Affichage dans la textbox si la room qui reçoit le message est selectionnée
                }
                else
                {
                    int id = Room_client.getIndex(m.Room);
                    Client.rooms[id].add(text);   //Sauvegarde discussion dans la memoire si on ne voit pas la discussion
                }
            }
            m = null;
        }


        private void clearDiscussion()
        {
            int lines = discussion.Lines.Length;
            int linesMax = 145;
            if (lines > linesMax)
            {
                int difference = lines - linesMax;
                discussion.Lines = discussion.Lines.Skip(difference).ToArray();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (previousIndex != listBox1.SelectedIndex)
            {
                string room = listBox1.GetItemText(listBox1.SelectedItem); //Recupere le nom de la room selectionnée
                int index = Room_client.getIndex(room);             //Trouve son index

                if (Client.rooms[index].subscribe == false) //envoi du message subscribe pour la room nouvellement selectionnée au serveur
                {
                    c.send(new Message(new List<string>() { room }, null, 0));

                    Client.rooms[index].subscribe = true;   //On active la room chez le client
                }



                texte.Focus();

                lock (discussion)                                   
                {
                    Client.rooms[previousIndex].add(discussion.Text);           //Sauvegarde la discussion dans la mémoire
                    discussion.Clear();

                    discussion.AppendText(Client.rooms[index].chatbox);       //Affiche son texte
                    previousIndex = listBox1.SelectedIndex;
                }
            }

        }

        public void setRoomList(List<string> roomList)
        {
            foreach (string r in roomList)
            {
                Client.rooms.Add(new Room_client(r));
                listBox1.Items.Add(r);
            }
            // here
            // Bug to fix here !
            // here
            //listBox1.DataSource = roomList;
            listBox1.SelectedIndex = 0;
        }


        public void addRoomToList(string room)
        {
            Client.rooms.Add(new Room_client(room));
            listBox1.Items.Add(room);
        }
       
    }
}
