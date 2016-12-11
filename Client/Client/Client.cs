﻿using System;
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
        int ADELETE = 41;
        public static int id = -1;
        public Socket sock;
        Form1 f;
        public bool connected;

        public Thread thread;
        public Message serverMessage = null;

        public delegate void ReceivedHandler(Client c, Message m);
        public event ReceivedHandler receivedEvent;

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

                    if (id == -1)
                    {
                        id = serverMessage.Sender;    //Initie l'id du client
                        f.setRoomList(serverMessage.Content);   //On liste les rooms disponibles
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
                        //pareil pour delete
                    }
                    else
                    {
                        serverMessage.Content[0] = "[" + serverMessage.Sender + "] " + serverMessage.Content[0];
                        raiseReceivedEvent();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Disconnected from server ");
            }
            finally
            {
                Application.Exit();
            }
        }

        public void bind(Form1 f)
        {
            receivedEvent += new ReceivedHandler(f.Afficher);
            this.f = f;
        }
        
        protected virtual void raiseReceivedEvent()
        {
            if (receivedEvent != null)  //Check for subscribers
            {
                receivedEvent(this, serverMessage);
            }
        }
    }
}
