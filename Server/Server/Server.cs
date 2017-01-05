using System;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Threading;

namespace chat
{
    class Server
    {
        public static Queue<Message> queue = new Queue<Message>();
        public Server()
        {
            int counter = 0;
          
            try
            {
                Socket s = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                );
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1212);
                s.Bind(iep);
                s.Listen(1);

                Auth test = new Auth();     //Init database connection : Check if the local database file .db exists
                new Room("Welcome_room");    //Welcome room
                //new Room("Room_1");
                //new Room("Finish !");

                Thread thread = new Thread(Broadcast);    //Thread qui gere la diffusion des messages
                thread.Start();

                while (true)    //Accueil client + création session client
                {
                    Console.WriteLine("Waiting for clients...");
                    Socket socketClient = s.Accept();
                    counter += 1;
                    Console.WriteLine("[Client " + counter + " connected]");

                    Session client = new Session(counter, socketClient);
                }
            }
            catch (SocketException)
            {
                //Dans le cas où le client ferme la connexion
                Console.WriteLine("Connexion lost with clients.");
                while (true) ;
            }
        }

        public void Broadcast() //Envoie les messages de la queue vers les thread d'envoie de tous les clients de la room concernée
        {
            try
            {
                while (true)
                {
                    if(queue.Count != 0)
                    {
                        Message m = queue.Dequeue();
                        if (m.Sender == 0)  //Envoie à tous les clients
                        {
                            sendToAll(m);
                        }
                        else    //Envoie à un seul client
                        {
                            sendToRoom(m);
                        }
                    } 
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Broadcast error : " + e.ToString());
            }
        }

        private void sendToRoom(Message m)
        {
            try
            {
                Room r = null;
                for (int i = 0; i < Room.rooms.Count; i++)   //On cherche la room dans la liste de rooms par son id
                    if (Room.rooms[i].name == m.Room) r = Room.rooms[i];

                for (int i = 0; i < r.sessions.Count; i++)
                {
                    if (r.sessions[i].Id != m.Sender)
                    {
                        r.sessions[i].send(m);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void sendToAll(Message m)
        {
            try
            {
                for (int i = 0; i < Session.sessions.Count; i++)
                {
                    Session.sessions[i].send(m);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
