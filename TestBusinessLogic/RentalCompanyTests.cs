using System;
using System.Collections.Generic;
using System.Text;
using BusinessLogic;
using BusinessLogic.Domain;
using BusinessLogic.Exceptions;
using BusinessLogic.TimeProvider;
using DAL;
using Moq;
using NUnit.Framework;

namespace TestBusinessLogic
{
    public class RentalCompanyTests
    {
        IScooterService ScooterService;
        IRentalCompany RentalCompany;

        [SetUp]
        public void Setup()
        {
            ScooterService = new ScooterService();
            RentalCompany = new RentalCompany(ScooterService);
        }

        [Test]
        public void RentScooter()
        {
            var newScooterId = "1";
            ScooterService.AddScooter(newScooterId, 1);

            Assert.DoesNotThrow(() => RentalCompany.StartRent(newScooterId));

            Assert.IsTrue(ScooterService.GetScooterById(newScooterId).IsRented);
        }

        [Test]
        public void RentScooterNotExisted()
        {
            var newScooterId = "1";
            Assert.IsNull(ScooterService.GetScooterById(newScooterId));

            Assert.Throws<ScooterDoesNotExistExeption>(() => RentalCompany.StartRent(newScooterId));
        }

        [Test]
        public void RentScooterWhichIsRented()
        {
            var newScooterId = "1";
            ScooterService.AddScooter(newScooterId, 1);
            RentalCompany.StartRent(newScooterId);

            Assert.Throws<ScooterIsAlreadyRentedExeption>(() => RentalCompany.StartRent(newScooterId));
        }

        [Test]
        public void EndRentLessThanADayCheap()
        {
            var rentTimeMinutes = 200;
            var newScooterPrice = 0.01M;//14.4 per day
            var result = EndRentTest(rentTimeMinutes, newScooterPrice);

            Assert.AreEqual(newScooterPrice * rentTimeMinutes, result);
        }

        [Test]
        public void EndRentLessThanADayExpensive()
        {
            var rentTimeMinutes = 200;
            var newScooterPrice = 0.2M;//28.8 per day
            var result = EndRentTest(rentTimeMinutes, newScooterPrice);

            Assert.AreEqual(Constants.MaxDayPrice, result);
        }

        [Test]
        public void EndRentMoreThanADayCheap()
        {
            var rentTimeMinutes = 2000;
            var newScooterPrice = 0.01M;//14.4 per day
            var result = EndRentTest(rentTimeMinutes, newScooterPrice);

            Assert.AreEqual(newScooterPrice * rentTimeMinutes, result);
        }

        [Test]
        public void EndRentMoreThanADayExpensive()
        {
            var rentTimeMinutes = 2000;//more than a day, but less than 2 days
            var newScooterPrice = 0.02M;//28.8 per day
            var result = EndRentTest(rentTimeMinutes, newScooterPrice);
            var lastDayPrice = (rentTimeMinutes - 60 * 24) * newScooterPrice > Constants.MaxDayPrice ?
                Constants.MaxDayPrice :
                (rentTimeMinutes - 60 * 24) * newScooterPrice;
            var expectedResult = Constants.MaxDayPrice + lastDayPrice;

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void EndRentScooterDoesNotExist()
        {
            var scooterId = "1";
            Assert.IsNull(ScooterService.GetScooterById(scooterId));

            Assert.Throws<ScooterDoesNotExistExeption>(() => RentalCompany.EndRent(scooterId));
        }

        [Test]
        public void EndRentNotStarted()
        {
            var newScooterId = "1";
            var newScooterPrice = 0.01M;//14.4 per day
            ScooterService.AddScooter(newScooterId, newScooterPrice);
            
            Assert.Throws<NoActiveRentException>(() => RentalCompany.EndRent(newScooterId));
        }

        private decimal EndRentTest(int minutes, decimal pricePerMinute)
        {
            var startTimeMock = new Mock<TimeProvider>();
            startTimeMock.SetupGet(tp => tp.Now).Returns(DateTime.Today.Date);

            var endTimeMock = new Mock<TimeProvider>();
            endTimeMock.SetupGet(tp => tp.Now).Returns(DateTime.Today.Date.AddMinutes(minutes));

            TimeProvider.Current = startTimeMock.Object;

            var newScooterId = "1";
            ScooterService.AddScooter(newScooterId, pricePerMinute);
            TimeProvider.Current = startTimeMock.Object;
            RentalCompany.StartRent(newScooterId);

            TimeProvider.Current = endTimeMock.Object;
            return RentalCompany.EndRent(newScooterId);
        }
    }
}
