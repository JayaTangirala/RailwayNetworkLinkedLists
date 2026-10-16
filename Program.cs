namespace RailwayNetwork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var network = new RailwayNetwork();

            // Seed a small example so the program can be explored immediately.
            Train t1 = network.CreateTrain("T1");
            t1.Append("C1");
            t1.Append("C2");
            t1.Append("C3");
            t1.Append("C4");

            Train t2 = network.CreateTrain("T2");
            t2.Append("X1");
            t2.Append("X2");

            Route r1 = network.CreateRoute("R1");
            r1.AddLast("Riverside");
            r1.AddLast("City Centre");
            r1.AddLast("Airport");
            r1.AddLast("University");

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== Dynamic Railway Network ===");
                Console.WriteLine("1. Display all trains and routes");
                Console.WriteLine("2. Append a carriage");
                Console.WriteLine("3. Insert a carriage at an index");
                Console.WriteLine("4. Detach a carriage");
                Console.WriteLine("5. Reverse a train");
                Console.WriteLine("6. Split a train after a carriage");
                Console.WriteLine("7. Merge one train into another");
                Console.WriteLine("8. Transfer a carriage range to another train");
                Console.WriteLine("9. Add a station to a route");
                Console.WriteLine("10. Remove a station from a route");
                Console.WriteLine("11. Navigate to next/previous station");               
                Console.WriteLine("0. Exit");

                string choice = Read("Choose an option: ");
                try
                {
                    switch (choice)
                    {
                        case "1":
                            network.DisplayAll();
                            break;

                        case "2":
                            {
                                Train train = network.GetTrain(Read("Train ID: "));
                                train.Append(Read("New carriage ID: "));
                                Console.WriteLine(train.Display());
                                break;
                            }

                        case "3":
                            {
                                Train train = network.GetTrain(Read("Train ID: "));
                                int index = ReadInt("Zero-based insertion index: ");
                                train.InsertAt(index, Read("New carriage ID: "));
                                Console.WriteLine(train.Display());
                                break;
                            }

                        case "4":
                            {
                                Train train = network.GetTrain(Read("Train ID: "));
                                train.DetachById(Read("Carriage ID to detach: "));
                                Console.WriteLine(train.Display());
                                break;
                            }

                        case "5":
                            {
                                Train train = network.GetTrain(Read("Train ID: "));
                                train.Reverse();
                                Console.WriteLine(train.Display());
                                break;
                            }

                        case "6":
                            {
                                Train train = network.GetTrain(Read("Source train ID: "));
                                string carriageId = Read("Split after carriage ID: ");
                                string newId = Read("New train ID: ");
                                Train second = train.SplitAfter(carriageId, newId);
                                network.RegisterTrain(second);
                                Console.WriteLine(train.Display());
                                Console.WriteLine(second.Display());
                                break;
                            }

                        case "7":
                            {
                                Train destination = network.GetTrain(Read("Destination train ID: "));
                                Train source = network.GetTrain(Read("Source train ID: "));
                                destination.MergeFrom(source);
                                Console.WriteLine(destination.Display());
                                Console.WriteLine(source.Display());
                                break;
                            }

                        case "8":
                            {
                                Train source = network.GetTrain(Read("Source train ID: "));
                                Train destination = network.GetTrain(Read("Destination train ID: "));
                                string first = Read("First carriage ID in range: ");
                                string last = Read("Last carriage ID in range: ");
                                source.TransferRangeTo(destination, first, last);
                                Console.WriteLine(source.Display());
                                Console.WriteLine(destination.Display());
                                break;
                            }

                        case "9":
                            {
                                Route route = network.GetRoute(Read("Route ID: "));
                                string after = Read("Existing station name (leave blank to append at end): ");
                                string newStation = Read("New station name: ");
                                if (string.IsNullOrWhiteSpace(after))
                                    route.AddLast(newStation);
                                else
                                    route.AddAfter(after, newStation);
                                Console.WriteLine(route.Display());
                                break;
                            }

                        case "10":
                            {
                                Route route = network.GetRoute(Read("Route ID: "));
                                route.Remove(Read("Station name to remove: "));
                                Console.WriteLine(route.Display());
                                break;
                            }

                        case "11":
                            {
                                Route route = network.GetRoute(Read("Route ID: "));
                                string station = Read("Current station name: ");
                                Console.WriteLine("Next: " + route.NextFrom(station).Name);
                                Console.WriteLine("Previous: " + route.PreviousFrom(station).Name);
                                break;
                            }                        

                        case "0":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Unknown option. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Operation failed: " + ex.Message);
                }
            }

            Console.WriteLine("Goodbye.");
        }

      

        private static string Read(string prompt)
        {
            Console.Write(prompt);
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                string input = Read(prompt);
                if (int.TryParse(input, out int value))
                    return value;
                Console.WriteLine("Enter a valid whole number.");
            }
        }
    }    
}
