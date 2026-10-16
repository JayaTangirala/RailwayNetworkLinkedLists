using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayNetwork
{
    internal class Carriage
    {
        public string Id;
        public Carriage Next, Prev;
        public Train Owner;

        public Carriage(string id)
        {
            Id = id;
        }

        public override string ToString() => Id;
    }
}
