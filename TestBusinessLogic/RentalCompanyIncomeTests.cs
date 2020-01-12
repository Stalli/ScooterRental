using System;
using System.Collections.Generic;
using System.Text;
using BusinessLogic;
using BusinessLogic.Domain;
using BusinessLogic.TimeProvider;
using Moq;
using NUnit.Framework;

namespace TestBusinessLogic
{
    public class RentalCompanyIncomeTests
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
        public void IncomeThisYearWithNotCompleted()
        {
            ScooterService.AddScooter("1", 0.01M);
            ScooterService.AddScooter("2", 0.01M);

            var thisYearStartRentDate = new Mock<TimeProvider>();
            thisYearStartRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 1));

            var thisYearEndRentDate = new Mock<TimeProvider>();
            thisYearEndRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 10));

            var reportDate = new Mock<TimeProvider>();
            reportDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 15));

            TimeProvider.Current = thisYearStartRentDate.Object;
            RentalCompany.StartRent("1");
            RentalCompany.StartRent("2");

            TimeProvider.Current = thisYearEndRentDate.Object;
            var test = RentalCompany.EndRent("1");

            TimeProvider.Current = reportDate.Object;
            var scooter1Result = 9*24*60*0.01M;//9 days with cheap price
            var scooter2Result = 14*24*60*0.01M;//14 days with cheap price
            var expectedResult = scooter1Result + scooter2Result;

            Assert.AreEqual(expectedResult, RentalCompany.CalculateIncome(DateTime.Today.Date.Year, true));
        }

        [Test]
        public void IncomeThisYearWithoutNotCompleted()
        {
            ScooterService.AddScooter("1", 0.01M);
            ScooterService.AddScooter("2", 0.01M);

            var thisYearStartRentDate = new Mock<TimeProvider>();
            thisYearStartRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 1));

            var thisYearEndRentDate = new Mock<TimeProvider>();
            thisYearEndRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 10));

            var reportDate = new Mock<TimeProvider>();
            reportDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 15));

            TimeProvider.Current = thisYearStartRentDate.Object;
            RentalCompany.StartRent("1");
            RentalCompany.StartRent("2");

            TimeProvider.Current = thisYearEndRentDate.Object;
            var test = RentalCompany.EndRent("1");

            TimeProvider.Current = reportDate.Object;
            var scooter1Result = 9 * 24 * 60 * 0.01M;//9 days with cheap price
            var expectedResult = scooter1Result;

            Assert.AreEqual(expectedResult, RentalCompany.CalculateIncome(DateTime.Today.Date.Year, false));
        }

        [Test]
        public void IncomePreviousYearWithNotCompleted()
        {
            ScooterService.AddScooter("1", 0.01M);
            ScooterService.AddScooter("2", 0.01M);

            var previousYearStartRentDate = new Mock<TimeProvider>();
            previousYearStartRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.AddYears(-1).Year, 1, 1));

            var previousYearEndRentDate = new Mock<TimeProvider>();
            previousYearEndRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.AddYears(-1).Year, 1, 10));

            var reportDate = new Mock<TimeProvider>();
            reportDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 15));

            TimeProvider.Current = previousYearStartRentDate.Object;
            RentalCompany.StartRent("1");
            RentalCompany.StartRent("2");

            TimeProvider.Current = previousYearEndRentDate.Object;
            var test = RentalCompany.EndRent("1");

            TimeProvider.Current = reportDate.Object;
            var scooter1Result = 9 * 24 * 60 * 0.01M;//9 days with cheap price
            //even though we include not completed rents, we won't count them, as they will be finished in another year
            var expectedResult = scooter1Result;

            Assert.AreEqual(expectedResult, RentalCompany.CalculateIncome(DateTime.Today.Date.AddYears(-1).Year, true));
        }

        [Test]
        public void IncomePreviousYearWithoutNotCompleted()
        {
            ScooterService.AddScooter("1", 0.01M);
            ScooterService.AddScooter("2", 0.01M);

            var previousYearStartRentDate = new Mock<TimeProvider>();
            previousYearStartRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.AddYears(-1).Year, 1, 1));

            var previousYearEndRentDate = new Mock<TimeProvider>();
            previousYearEndRentDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.AddYears(-1).Year, 1, 10));

            var reportDate = new Mock<TimeProvider>();
            reportDate.SetupGet(tp => tp.Now).Returns(new DateTime(DateTime.Today.Date.Year, 1, 15));

            TimeProvider.Current = previousYearStartRentDate.Object;
            RentalCompany.StartRent("1");
            RentalCompany.StartRent("2");

            TimeProvider.Current = previousYearEndRentDate.Object;
            var test = RentalCompany.EndRent("1");

            TimeProvider.Current = reportDate.Object;
            var scooter1Result = 9 * 24 * 60 * 0.01M;//9 days with cheap price
            var expectedResult = scooter1Result;

            Assert.AreEqual(expectedResult, RentalCompany.CalculateIncome(DateTime.Today.Date.AddYears(-1).Year, false));
        }
    }
}
