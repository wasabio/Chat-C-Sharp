using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace chat
{
    class Room_client
    {
        public String name;
        public String chatbox;
        public bool subscribe = false;

        public Room_client(String n)
        {
            name = n;
            chatbox = "";
        }

        public void save(String text)
        {
            chatbox = text;
        }

        public static int getIndex(string name)
        {
            lock(Client.rooms)
            {
                for(int i = 0; i < Client.rooms.Count; i++)
                    if (name == Client.rooms[i].name) return i;
            }
            return -1;
        }
    }
}
