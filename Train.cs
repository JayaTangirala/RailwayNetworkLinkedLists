using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayNetwork
{
    internal class Train
    {
        public string Id;
        public Carriage Head,Tail;
        public int Count;

        public Train(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Train ID cannot be empty.", nameof(id));

            Id = id;
        }

        public bool IsEmpty => Head == null;

        public Carriage Find(string carriageId)
        {
            for (Carriage current = Head; current != null; current = current.Next)
                if (string.Equals(current.Id, carriageId, StringComparison.OrdinalIgnoreCase))
                    return current;

            return null;
        }

        public void Append(string carriageId) => Append(new Carriage(carriageId));

        public void Append(Carriage carriage)
        {
            EnsureDetached(carriage);

            if (Find(carriage.Id) != null)
                throw new InvalidOperationException($"Train {Id} already contains carriage {carriage.Id}.");

            if (IsEmpty)
            {
                Head = Tail = carriage;
            }
            else
            {
                Tail.Next = carriage;
                carriage.Prev = Tail;
                Tail = carriage;
            }

            carriage.Owner = this;
            Count++;
        }

        public void InsertAt(int zeroBasedIndex, string carriageId)
        {
            if (zeroBasedIndex < 0 || zeroBasedIndex > Count)
                throw new ArgumentOutOfRangeException(nameof(zeroBasedIndex));

            Carriage newCarriage = new Carriage(carriageId);
            if (Find(carriageId) != null)
                throw new InvalidOperationException($"Train {Id} already contains carriage {carriageId}.");

            if (zeroBasedIndex == Count)
            {
                Append(newCarriage);
                return;
            }

            if (zeroBasedIndex == 0)
            {
                newCarriage.Next = Head;
                Head.Prev = newCarriage;
                Head = newCarriage;
            }
            else
            {
                Carriage current = GetAt(zeroBasedIndex);
                Carriage previous = current.Prev;
                previous.Next = newCarriage;
                newCarriage.Prev = previous;
                newCarriage.Next = current;
                current.Prev = newCarriage;
            }

            newCarriage.Owner = this;
            Count++;
        }

        public Carriage GetAt(int zeroBasedIndex)
        {
            if (zeroBasedIndex < 0 || zeroBasedIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(zeroBasedIndex));

            // Traverse from the nearer endpoint.
            if (zeroBasedIndex <= Count / 2)
            {
                Carriage current = Head;
                for (int i = 0; i < zeroBasedIndex; i++)
                    current = current.Next;
                return current;
            }
            else
            {
                Carriage current = Tail;
                for (int i = Count - 1; i > zeroBasedIndex; i--)
                    current = current.Prev;
                return current;
            }
        }

        public void Detach(Carriage carriage)
        {
            if (carriage == null || carriage.Owner != this)
                throw new InvalidOperationException("The carriage does not belong to this train.");

            if (carriage.Prev != null)
                carriage.Prev.Next = carriage.Next;
            else
                Head = carriage.Next;

            if (carriage.Next != null)
                carriage.Next.Prev = carriage.Prev;
            else
                Tail = carriage.Prev;

            carriage.Next = null;
            carriage.Prev = null;
            carriage.Owner = null;
            Count--;

            if (Count == 0)
                Head = Tail = null;
        }

        public void DetachById(string carriageId)
        {
            Carriage carriage = Find(carriageId);
            if (carriage == null)
                throw new InvalidOperationException($"Carriage {carriageId} was not found in train {Id}.");

            Detach(carriage);
        }

        public void Reverse()
        {
            Carriage current = Head;
            while (current != null)
            {
                Carriage oldNext = current.Next;
                current.Next = current.Prev;
                current.Prev = oldNext;
                current = oldNext;
            }

            Carriage oldHead = Head;
            Head = Tail;
            Tail = oldHead;
        }

        // Splits the train after the specified carriage. The suffix is returned as a new train.
        public Train SplitAfter(string carriageId, string newTrainId)
        {
            Carriage splitPoint = Find(carriageId);
            if (splitPoint == null)
                throw new InvalidOperationException($"Carriage {carriageId} was not found in train {Id}.");

            Train second = new Train(newTrainId);
            Carriage firstOfSecond = splitPoint.Next;

            // Splitting after the tail produces an empty second train.
            if (firstOfSecond == null)
                return second;

            splitPoint.Next = null;
            firstOfSecond.Prev = null;

            second.Head = firstOfSecond;
            second.Tail = Tail;

            int moved = 0;
            for (Carriage current = second.Head; current != null; current = current.Next)
            {
                current.Owner = second;
                moved++;
            }

            second.Count = moved;
            Count -= moved;
            Tail = splitPoint;
            return second;
        }

        // Appends all carriages from another train. The source becomes empty.
        public void MergeFrom(Train source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (ReferenceEquals(this, source))
                throw new InvalidOperationException("A train cannot be merged with itself.");
            if (source.IsEmpty)
                return;

            if (IsEmpty)
            {
                Head = source.Head;
                Tail = source.Tail;
                Count = source.Count;
            }
            else
            {
                Tail.Next = source.Head;
                source.Head.Prev = Tail;
                Tail = source.Tail;
                Count += source.Count;
            }

            for (Carriage current = source.Head; current != null; current = current.Next)
                current.Owner = this;

            source.Head = source.Tail = null;
            source.Count = 0;
        }

        // Moves the inclusive range firstId..lastId to the end of destination.
        public void TransferRangeTo(Train destination, string firstId, string lastId)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (ReferenceEquals(this, destination))
                throw new InvalidOperationException("Source and destination trains must be different.");

            Carriage first = Find(firstId);
            Carriage last = Find(lastId);
            if (first == null || last == null)
                throw new InvalidOperationException("Both range boundary carriages must belong to the source train.");

            int rangeCount = 0;
            bool lastReached = false;
            for (Carriage current = first; current != null; current = current.Next)
            {
                rangeCount++;
                if (current == last)
                {
                    lastReached = true;
                    break;
                }
            }

            if (!lastReached)
                throw new InvalidOperationException("The last carriage must occur at or after the first carriage.");

            // Check duplicate IDs before changing either train.
            for (Carriage current = first; ; current = current.Next)
            {
                if (destination.Find(current.Id) != null)
                    throw new InvalidOperationException($"Destination already contains carriage {current.Id}.");

                if (current == last)
                    break;
            }

            Carriage before = first.Prev;
            Carriage after = last.Next;

            if (before != null)
                before.Next = after;
            else
                Head = after;

            if (after != null)
                after.Prev = before;
            else
                Tail = before;

            first.Prev = null;
            last.Next = null;
            Count -= rangeCount;

            if (IsEmpty)
                Head = Tail = null;

            if (destination.IsEmpty)
            {
                destination.Head = first;
                destination.Tail = last;
            }
            else
            {
                destination.Tail.Next = first;
                first.Prev = destination.Tail;
                destination.Tail = last;
            }

            destination.Count += rangeCount;
            for (Carriage current = first; current != null; current = current.Next)
            {
                current.Owner = destination;
                if (current == last)
                    break;
            }
        }

        public bool Validate(out string message)
        {
            if (Count < 0)
            {
                message = "Count cannot be negative.";
                return false;
            }

            if (Count == 0)
            {
                bool validEmpty = Head == null && Tail == null;
                message = validEmpty ? "Valid empty train." : "Empty train has non-null endpoints.";
                return validEmpty;
            }

            if (Head == null || Tail == null)
            {
                message = "Non-empty train has a null endpoint.";
                return false;
            }

            if (Head.Prev != null || Tail.Next != null)
            {
                message = "Head.Prev and Tail.Next must be null for a linear train.";
                return false;
            }

            var seen = new HashSet<Carriage>();
            Carriage previous = null;
            int visited = 0;

            for (Carriage current = Head; current != null; current = current.Next)
            {
                if (!seen.Add(current))
                {
                    message = "Unexpected cycle detected in train.";
                    return false;
                }

                if (current.Prev != previous)
                {
                    message = $"Incorrect Prev link at carriage {current.Id}.";
                    return false;
                }

                if (current.Owner != this)
                {
                    message = $"Incorrect owner reference at carriage {current.Id}.";
                    return false;
                }

                previous = current;
                visited++;
            }

            bool valid = visited == Count && previous == Tail;
            message = valid ? "Train structure is valid." : "Count or Tail does not match traversal.";
            return valid;
        }

        public string Display()
        {
            if (IsEmpty)
                return $"Train {Id}: [empty]";

            var ids = new List<string>();
            for (Carriage current = Head; current != null; current = current.Next)
                ids.Add(current.Id);

            return $"Train {Id}: Head -> {string.Join(" <-> ", ids)} <- Tail (Count: {Count})";
        }

        private static void EnsureDetached(Carriage carriage)
        {
            if (carriage == null)
                throw new ArgumentNullException(nameof(carriage));
            if (carriage.Owner != null || carriage.Next != null || carriage.Prev != null)
                throw new InvalidOperationException("Carriage must be detached before insertion.");
        }
    }
}
