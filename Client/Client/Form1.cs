using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace chat
{
    partial class Form1 : Form
    {
        private Client c;
        private int previousIndex = 0;

        delegate void SetTextCallback(object sender, Message m);
        delegate void SetTextCall(String name);

        public Form1(Client c)
        {
            InitializeComponent();
            AcceptButton = button1;
            this.c = c;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void button1_Click_1(object sender, EventArgs e)        //Envoie de message
        {
            if (!texte.Text.Equals(""))
            {
                string room = listBox1.GetItemText(listBox1.SelectedItem);
                Message message = new Message(new List<string>() { texte.Text }, room, Client.id);   //go to welcome room
                string text = "[You] " + message.Content[0] + "\r\n";

                discussion.AppendText(text);  //Affichage dans la textbox

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
                    Client.rooms[id].chatbox += text;   //Rajout du message dans la memoire si on ne voit pas la discussion : la taille du string augmente, mais lors de 
                }                                       //l'affichage dans la textbox, la textbox a un algo qui limite la taille
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
                    Client.rooms[previousIndex].save(discussion.Text);           //Sauvegarde la discussion dans la mémoire                     
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

            listBox1.SelectedIndex = 0;
            Client.rooms[0].subscribe = true;   //Par defaut, on subscribe a la 1ere room d'accueil
        }


        public void addRoomToList(string room)
        {
            if (this.listBox1.InvokeRequired)
            {
                SetTextCall d = new SetTextCall(addRoomToList);  //Thread safe : ajout room à listbox
                this.Invoke(d, new object[] { room });
            }
            else
            {
                Client.rooms.Add(new Room_client(room));
                listBox1.Items.Add(room);
            }
        }

        public void deleteRoomFromList(string room)
        {
            if (this.listBox1.InvokeRequired)    //Thread safe : suppression room de la Listbox
            {
                SetTextCall d = new SetTextCall(deleteRoomFromList);
                this.Invoke(d, new object[] { room });
            }
            else
            {
                for (int n = listBox1.Items.Count - 1; n >= 0; --n)     //Boucle inverse (sinon problème avec les index)
                {
                    if (listBox1.Items[n].ToString().Equals(room))
                    {
                        listBox1.Items.RemoveAt(n);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)  //Bouton pour rajouter une room
        {
            String roomName = textBox1.Text;
            if(roomName != "")
            {
                Message message = new Message(new List<string>() { roomName }, null, Client.id);   //On envoie un message au serveur pour ajouter la room
                
                c.send(message);
                textBox1.Clear();
            }
        }


        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)   //Exit the Form thread
        {
            Application.ExitThread();
        }
    }
}
