using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayNetwork
{
    internal class Station
    {
        public string Name;
        public Station Next, Prev;
        public Route Owner;

        public Station(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
