using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Room
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public IList<string> Connections { get; set; }

        private Room()
        {

        }

        public Room(string id, string name)
        {
            Id = id;
            Name = name;
            Connections = new List<string>();
        }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }

    }
}
