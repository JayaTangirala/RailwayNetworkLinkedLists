using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayNetwork
{
    internal class RailwayNetwork
    {
        private readonly Dictionary<string, Train> trains =
           new Dictionary<string, Train>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Route> routes =
            new Dictionary<string, Route>(StringComparer.OrdinalIgnoreCase);

        public Train CreateTrain(string id)
        {
            if (trains.ContainsKey(id))
                throw new InvalidOperationException($"Train {id} already exists.");
            Train train = new Train(id);
            trains.Add(id, train);
            return train;
        }

        public void RegisterTrain(Train train)
        {
            if (train == null)
                throw new ArgumentNullException(nameof(train));
            if (trains.ContainsKey(train.Id))
                throw new InvalidOperationException($"Train {train.Id} already exists.");
            trains.Add(train.Id, train);
        }

        public IEnumerable<Train> Trains => trains.Values;
        public IEnumerable<Route> Routes => routes.Values;

        public Route CreateRoute(string id)
        {
            if (routes.ContainsKey(id))
                throw new InvalidOperationException($"Route {id} already exists.");
            Route route = new Route(id);
            routes.Add(id, route);
            return route;
        }

        public Train GetTrain(string id) =>
            trains.TryGetValue(id, out Train train)
                ? train
                : throw new KeyNotFoundException($"Train {id} was not found.");

        public Route GetRoute(string id) =>
            routes.TryGetValue(id, out Route route)
                ? route
                : throw new KeyNotFoundException($"Route {id} was not found.");

        public void DisplayAll()
        {
            Console.WriteLine("\n--- TRAINS ---");
            if (trains.Count == 0) Console.WriteLine("(none)");
            foreach (Train train in trains.Values)
                Console.WriteLine(train.Display());

            Console.WriteLine("\n--- ROUTES ---");
            if (routes.Count == 0) Console.WriteLine("(none)");
            foreach (Route route in routes.Values)
                Console.WriteLine(route.Display());
        }
    }
}
