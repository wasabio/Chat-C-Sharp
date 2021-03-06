﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat
{
    class Room
    {
        public static List<Room> rooms = new List<Room>();

        public List<Session> sessions = new List<Session>();    //Une room contient plusieurs sessions

        public String name; //Le nom pourrait servir d'id si on pouvait le rendre unique

        public Room(string name)    //Creation des rooms par defaut
        {
            this.name = name;
            rooms.Add(this);
            Console.WriteLine("Default Room added");
        }

        public Room(String name, Session session)   //Creation de room custom par une session
        {
            this.name = name;
            rooms.Add(this);            //Ajout de notre room a la liste de rooms du serveur
                                        //Le linkage est effectué quand le client décide de subscribe à la room
            Console.WriteLine("Room " + name + " created for session " + session.Id);
        }



        public void Remove(Session s)
        {
            Console.WriteLine("     Client " + s.Id + " removed from room " + name);
            this.sessions.Remove(s);
            if (this.sessions.Count == 0 && this.name != "Welcome_room")       //Si plus personne n'est dans la room, on la detruit, sauf si c'est la welcome room
            {
                rooms.Remove(this);
                Console.WriteLine("     Room " + this.name + " is now empty, deleting...");
                //Notifie les clients que la room n'existe plus.
                Message msg = new Message(new List<string>() { this.name, "remove" }, null, 0);
                Server.queue.Enqueue(msg);
            }
        }
    }
}
