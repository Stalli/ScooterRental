using DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Domain
{
    public class Rent : IEntity
    {
        public Rent(Scooter scooter, DateTime start)
        {
            Scooter = scooter;
            StartTimestamp = start;
        }
        public string Id { get; set; }
        public DateTime StartTimestamp { get; set; }

        public Scooter Scooter { get; private set; }

        public DateTime? EndTimestamp { get; set; }
    }
}
