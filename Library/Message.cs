using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace chat
{
    [Serializable]
    class Message : EventArgs
    {
        private List<string> content;
        private string room;
        private int sender;

        public Message(List<string> content, string room, int sender)
        {
            this.content = content;
            this.room = room;
            this.Sender = sender;
        }

        public List<string> Content
        {
            get
            { return content; }

            set
            { content = value; }
        }

        public string Room
        {
            get
            { return room; }

            set
            { room = value; }
        }

        public int Sender
        {
            get
            {
                return sender;
            }

            set
            {
                sender = value;
            }
        }

        public static void Serialize(Message m, Socket s)
        {
            NetworkStream ns = new NetworkStream(s);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(ns, m);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize the message. Reason: " + e.Message);
                throw;
            }
            finally
            {
                ns.Close();
            }
        }

        public static Message Deserialize(Socket s)
        {
            NetworkStream ns = new NetworkStream(s);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                return (Message)formatter.Deserialize(ns);
            }
            finally
            {
                ns.Close();
            }
        }
    }
}
