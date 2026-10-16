using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayNetwork
{
    internal class Route
    {
        public string Id;
        public Station Head;
        public int Count;

        public Route(string id)
        {           
            Id = id;
        }

        public bool IsEmpty()
        {
            return(Head == null);
        }        

        public Station Find(string stationName)
        {
            if (Head == null)
                return null;

            Station temp = Head;
            do
            {
                if (temp.Name==stationName)
                    return temp;
                temp = temp.Next;
            } while (temp != Head);

            return null;
        }

        public void AddLast(string stationName)
        {
            if (Find(stationName) != null)
                throw new InvalidOperationException($"Route {Id} already contains station {stationName}.");

            Station station = new Station(stationName);
            station.Owner = this;

            if (Head == null)
            {
                Head = station;
                station.Next = station;
                station.Prev = station;
            }
            else
            {
                Station tail = Head.Prev;
                tail.Next = station;
                station.Prev = tail;
                station.Next = Head;
                Head.Prev = station;
            }

            Count++;
        }

        public void AddAfter(string existingStationName, string newStationName)
        {
            Station current = Find(existingStationName);
            if (current == null)
                throw new InvalidOperationException($"Station {existingStationName} was not found.");
            if (Find(newStationName) != null)
                throw new InvalidOperationException($"Route {Id} already contains station {newStationName}.");

            Station station = new Station(newStationName);
            Station next = current.Next;

            station.Prev = current;
            station.Next = next;
            current.Next = station;
            next.Prev = station;
            station.Owner = this;
            Count++;
        }

        public void Remove(string stationName)
        {
            Station station = Find(stationName);
            if (station == null)
                throw new InvalidOperationException($"Station {stationName} was not found on route {Id}.");

            if (Count == 1)
            {
                Head = null;
            }
            else
            {
                station.Prev.Next = station.Next;
                station.Next.Prev = station.Prev;
                if (Head == station)
                    Head = station.Next;
            }

            station.Next = station.Prev = null;
            station.Owner = null;
            Count--;
        }

        public Station NextFrom(string stationName)
        {
            Station station = Find(stationName);
            if (station == null)
                throw new InvalidOperationException($"Station {stationName} was not found.");
            return station.Next;
        }

        public Station PreviousFrom(string stationName)
        {
            Station station = Find(stationName);
            if (station == null)
                throw new InvalidOperationException($"Station {stationName} was not found.");
            return station.Prev;
        }

        public bool Validate(out string message)
        {
            if (Count == 0)
            {
                bool validEmpty = Head == null;
                message = validEmpty ? "Valid empty route." : "Empty route has a non-null head.";
                return validEmpty;
            }

            if (Head == null)
            {
                message = "Non-empty route has a null head.";
                return false;
            }

            var seen = new HashSet<Station>();
            Station current = Head;
            int visited = 0;

            do
            {
                if (current == null || current.Next == null || current.Prev == null)
                {
                    message = "A station has a null circular link.";
                    return false;
                }

                if (!seen.Add(current))
                {
                    message = "Unexpected repeated station before returning to Head.";
                    return false;
                }

                if (current.Next.Prev != current || current.Prev.Next != current)
                {
                    message = $"Inconsistent links at station {current.Name}.";
                    return false;
                }

                if (current.Owner != this)
                {
                    message = $"Incorrect route owner at station {current.Name}.";
                    return false;
                }

                current = current.Next;
                visited++;
            } while (current != Head);

            bool valid = visited == Count;
            message = valid ? "Route structure is valid." : "Route count does not match traversal.";
            return valid;
        }

        public string Display()
        {
            if (Head == null)
                return $"Route {Id}: [empty]";

            var names = new List<string>();
            Station current = Head;
            do
            {
                names.Add(current.Name);
                current = current.Next;
            } while (current != Head);

            return $"Route {Id}: {string.Join(" -> ", names)} -> (back to {Head.Name}) (Stations: {Count})";
        }
    }
}
