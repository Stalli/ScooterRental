using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Domain;
using BusinessLogic.Exceptions;
using DAL;

namespace BusinessLogic
{
    public class ScooterService : IScooterService
    {
        private readonly IRepository<Scooter> scooterRepository;

        public ScooterService()
        {
            scooterRepository = new MockRepository<Scooter>();
        }

        public void AddScooter(string id, decimal pricePerMinute)
        {
            scooterRepository.Add(id, new Scooter(id, pricePerMinute));
        }

        public Scooter GetScooterById(string scooterId)
        {
            return scooterRepository.Get(scooterId);
        }

        public IList<Scooter> GetScooters()
        {
            return scooterRepository.GetAll();
        }

        public void RemoveScooter(string id)
        {
            scooterRepository.Delete(id);
        }
    }
}
