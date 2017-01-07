using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace chat
{
    class Client
    {
        public static int id = -1;
        public Socket sock;
        Form1 f;
        Form2 f2;
        public bool connected;

        public Thread thread;
        public Message serverMessage = null;

        public delegate void ReceivedHandler(Client c, Message m);
        public event ReceivedHandler receivedEvent;

        public delegate void FormHandler(Client c, EventArgs e);
        public event FormHandler formEvent;

        public static List<Room_client> rooms = new List<Room_client>();
        
        public Client()
        {
            try
            {
                sock = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );

                IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1212);
                sock.Connect(iep);

                connected = true;

                thread = new Thread(receive);
            }
            catch (SocketException)
            {
                MessageBox.Show("Connexion error : can't connect to server");
                connected = false;
            }
        }
        
        public void send(Message message)
        {
            try
            {
                if (sock.Connected)
                {
                    Message.Serialize(message, sock);   //Envoie un objet Message via un NetworkStream du Socket
                }
                else
                {
                    throw new SocketException();
                }
            }
            catch (SocketException)
            {
                MessageBox.Show("Socket error : message couldn't be sent");
            }
        }

        public void receive()
        {
            try
            {
                while (true)
                {
                    serverMessage = Message.Deserialize(sock);

                    if (id == -1)       //Le serveur n'a pas encore attribué d'ID au client
                    {
                        if(serverMessage.Content[0].Equals("false"))   //Mauvais identifiants
                            f2.errorMessage();

                        else    //Le client a été connecté, il reçoit la liste des rooms et son id
                        {
                            //Passage de f2 à f
                            formEvent += new FormHandler(f2.NextForm);
                            raiseFormEvent();

                            id = serverMessage.Sender;    //Initie l'id du client
                            f.setRoomList(serverMessage.Content);   //On liste les rooms disponibles
                        }
                    }
                    else if (serverMessage.Sender == 0)
                    {
                        if(serverMessage.Content.Count == 1)    //Content 0 : Message serveur
                        {
                            serverMessage.Content[0] = "[SERVER] " + serverMessage.Content[0];  //Message du server
                            raiseReceivedEvent();   //Permet d'afficher un message dans la form
                        }
                        else if (serverMessage.Content[1].Equals("add"))    //Content 0 : nom room - Content 1 : add
                        {
                            f.addRoomToList(serverMessage.Content[0]);   //Rajout de la nouvelle room à la liste des rooms
                        }
                        else if (serverMessage.Content[1].Equals("remove"))    //Content 0 : nom room - Content 1 : remove
                        {
                            f.deleteRoomFromList(serverMessage.Content[0]);   //Suppression d'une room vide de la ListBox
                        }
                    }
                    else
                    {
                        serverMessage.Content[0] = "[" + serverMessage.Sender + "] " + serverMessage.Content[0];
                        raiseReceivedEvent();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Disconnected from server");
            }
            finally
            {
                Application.Exit();
            }
        }

        public void bind(Form1 f, Form2 f2)
        {
            receivedEvent += new ReceivedHandler(f.Afficher);
            this.f = f;
            this.f2 = f2;
        }
        
        protected virtual void raiseReceivedEvent()
        {
            if (receivedEvent != null)  //Check for subscribers
            {
                receivedEvent(this, serverMessage);
            }
        }

        protected virtual void raiseFormEvent()
        {
            if (formEvent != null)  //Check for subscribers
            {
                formEvent(this, null);
            }
        }
    }
}
