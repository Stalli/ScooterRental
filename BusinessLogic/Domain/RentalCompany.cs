using BusinessLogic.Exceptions;
using DAL;
using BusinessLogic.TimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Domain
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IRepository<Rent> rentRepository;

        private IScooterService ScooterService;

        public RentalCompany(IScooterService scooterService)
        {
            ScooterService = scooterService;
            rentRepository = new MockRepository<Rent>();
        }

        public string Name => throw new NotImplementedException();

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            if (year.HasValue && year.Value != DateTime.Today.Year)
                includeNotCompletedRentals = false;

            var resultData = rentRepository.GetAll();
            if (!includeNotCompletedRentals)
                resultData = resultData.Where(r => r.EndTimestamp.HasValue).ToList();
            if (year.HasValue)
                resultData = resultData.Where(r => !r.EndTimestamp.HasValue || r.EndTimestamp.Value.Year == year).ToList();
            //if (includeNotCompletedRentals && year.HasValue && year.Value != DateTime.Today.Year)
            //    resultData = resultData.Where(r => r.EndTimestamp.HasValue).ToList();

            return resultData.Sum(r => 
                CalculateRentalPrice(r.StartTimestamp, r.EndTimestamp ?? TimeProvider.TimeProvider.Current.Now, r.Scooter.PricePerMinute));
        }

        public decimal EndRent(string scooterId)
        {
            var scooter = ScooterService.GetScooterById(scooterId);
            if (scooter == null) throw new ScooterDoesNotExistExeption();

            var activeRent = rentRepository.GetAll().FirstOrDefault(r => r.Scooter.Id == scooterId && !r.EndTimestamp.HasValue);
            if (activeRent == null) throw new NoActiveRentException();
            activeRent.EndTimestamp = TimeProvider.TimeProvider.Current.Now;

            scooter.IsRented = false;

            return CalculateRentalPrice(activeRent.StartTimestamp, activeRent.EndTimestamp.Value, scooter.PricePerMinute);
        }

        private decimal CalculateRentalPrice(DateTime start, DateTime finish, decimal pricePerMinute)
        {
            var timeTotal = finish - start;
            var minutesTotal = Convert.ToInt32(Math.Round(timeTotal.TotalMinutes, MidpointRounding.ToNegativeInfinity));

            var maxMinutePrice = Constants.MaxDayPrice / (24 * 60);
            if (pricePerMinute > maxMinutePrice)
            {
                var days = Convert.ToInt32(Math.Round(timeTotal.TotalDays, MidpointRounding.ToNegativeInfinity));
                var lastDayOriginalPrice = (minutesTotal - days * 24 * 60) * pricePerMinute;
                var lastDayResultPrice = 
                    lastDayOriginalPrice > Constants.MaxDayPrice ?
                    Constants.MaxDayPrice :
                    lastDayOriginalPrice;

                return days * Constants.MaxDayPrice + lastDayResultPrice;
            }

            return pricePerMinute * minutesTotal;
        }

        public void StartRent(string scooterId)
        {
            var scooter = ScooterService.GetScooterById(scooterId);
            if (scooter == null) throw new ScooterDoesNotExistExeption();
            if (scooter.IsRented) throw new ScooterIsAlreadyRentedExeption();

            if (IsRentActive(scooter.Id)) throw new ScooterIsAlreadyRentedExeption();

            rentRepository.Add(Guid.NewGuid().ToString(), new Rent(scooter, TimeProvider.TimeProvider.Current.Now));
            scooter.IsRented = true;
        }

        public bool IsRentActive(string scooterId)
        {
            return rentRepository.GetAll().Exists(r => r.Scooter.Id == scooterId && r.EndTimestamp != null);
        }
    }
}
