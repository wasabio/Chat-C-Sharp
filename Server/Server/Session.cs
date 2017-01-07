using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace chat
{
    class Session
    {
        public static List<Session> sessions = new List<Session>();
        public List<Room> rooms = new List<Room>(); //Une session peut être dans plusieurs rooms

        private Socket s;
        private int id;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public Session(int id, Socket s)
        {
            this.id = id;
            this.s = s;

            try
            {
                sessions.Add(this);
                linkToRoom(Room.rooms[0]);
                Thread thread = new Thread(receive);
                thread.Start();

                List<string> roomList = new List<string>();     //Envoie la liste de rooms, sous forme de liste de string
                lock (Room.rooms)
                {
                    foreach (Room r in Room.rooms)
                        roomList.Add(r.name);
                }

                send(new Message(roomList, "Welcome_room", 0));     //Lorsque c'est un message du serveur, le serveur prend l'id du client
                                                        //On envoie la liste des topics
                send(new Message(new List<string>() { "Welcome in " + Room.rooms[0].name + ", your client ID is: " + id }, "Welcome_room", 0));
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        public void send(Message message)
        {
            try
            {
                if (s.Connected)
                {
                    Message.Serialize(message, s); //Envoie un objet Message via un NetworkStream du Socket
                }
                else
                {
                    throw new SocketException();
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Socket error : message not sent");
            }
        }

        public void receive()
        {
            try
            {
                Message message;

                while (true)
                {
                    message = Message.Deserialize(s);
                    message.Sender = this.id;       //On sécurise l'ID de l'émetteur, pas d'usurpation d'ID possible

                    if (message.Room == null && message.Content[0] == "signup")     //Register a new user
                    {
                        bool status = Auth.register(message.Sender, message.Content[1], message.Content[2]);

                        if (status)
                        {
                            send(new Message(new List<string>() { "signup", "true" }, "Welcome_room", id));
                        }
                        else
                        {
                            send(new Message(new List<string>() { "signup", "false" }, "Welcome_room", id));
                        }
                    }
                    else if (message.Room == null && message.Content[0] == "signin")    //Login an existing user
                    {
                        bool status = Auth.login(message.Content[1], message.Content[2]);

                        if (status) send(new Message(new List<string>() { "signin", "true" }, "Welcome_room", id));
                        else send(new Message(new List<string>() { "signin", "false" }, "Welcome_room", id));
                    }
                    else if (message.Room == null)   //Message a destination du serveur : creation/subscribe a une room
                    {
                        bool trouve = false;
                        for (int i = 0; i < Room.rooms.Count; i++)   //On cherche si la room existe dans la liste de rooms (via son nom)
                        {
                            if (Room.rooms[i].name == message.Content[0])   //Si on trouve la room on subscribe la session sur cette room
                            {
                                linkToRoom(Room.rooms[i]); //fait l'association de listes entre la room et la session
                                trouve = true;
                            }
                        }
                        if (trouve == false) //Creation de la room
                        {
                            new Room(message.Content[0], this);
                            //On envoie la nouvelle liste de room aux clients
                            Message msg = new Message(new List<string>() { message.Content[0], "add" }, null, 0);   //Message d'upload des rooms
                            Server.queue.Enqueue(msg);                                                              //mis en queue pour broadcast
                        }
                    }
                    else    //Message classique dans une Room
                    {
                        Console.WriteLine("[Client " + id + " says] " + message.Content[0] + "  TO " + message.Room);
                        Server.queue.Enqueue(message);  //Ajout du message entrant a la liste, il sera transmis a la room par un thread serveur
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[Client " + id + " disconnected]");
                disconnect();
            }
            finally
            {
                sessions.Remove(this);
                removeAllRooms();   //Supprime les rooms qui deviennent vides
            }
        }

        private void disconnect()
        {
            try
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
            catch (SocketException)
            {
                Console.WriteLine("Error while disconnecting Client ID: " + id);
            }
        }

        public void linkToRoom(Room r)
        {
            r.sessions.Add(this);  //Ajout de la session dans la room
            rooms.Add(r);          //Ajout de la room dans la session 
        }

        private void removeAllRooms()
        {
            Console.WriteLine("     Client " + this.id + " was subscribed in " + this.rooms.Count + " rooms.");
            for (int i = this.rooms.Count - 1; i >= 0; i--)     //Itération inversée (Sinon décalage des index lors des supressions)
            {
                removeRoom(this.rooms[i]);
            }
        }

        private void removeRoom(Room r)
        {
            r.Remove(this);
            this.rooms.Remove(r);   //On appelle la methode de la room qui va enlever cette session de sa liste et verifier si on doit supprimer la room
        }
    }
}
