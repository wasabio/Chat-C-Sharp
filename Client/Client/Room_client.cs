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

        public void add(String text)
        {
            chatbox = chatbox + text + "\r\n";
            int lines = chatbox.Split('\n').Length;
            int linesMax = 145;
            if (lines > linesMax)
            {
                int difference = lines - linesMax;
                chatbox = RemoveFirstLines(chatbox, difference);
            }
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

        private static string RemoveFirstLines(string text, int linesCount)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }
    }
}
