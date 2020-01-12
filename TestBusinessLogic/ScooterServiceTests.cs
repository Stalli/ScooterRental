using BusinessLogic.Domain;
using NUnit.Framework;
using BusinessLogic;

namespace TestBusinessLogic
{
    public class ScooterServiceTests
    {
        IScooterService ScooterService;

        [SetUp]
        public void Setup()
        {
            ScooterService = new ScooterService();
        }

        [Test]
        public void AddScooter()
        {
            var newScooterId = "1";
            Assert.IsNull(ScooterService.GetScooterById(newScooterId));

            ScooterService.AddScooter(newScooterId, 1);

            Assert.IsNotNull(ScooterService.GetScooterById(newScooterId));
        }

        [Test]
        public void RemoveScooter()
        {
            var newScooterId = "1";
            ScooterService.AddScooter(newScooterId, 1);
            Assert.IsNotNull(ScooterService.GetScooterById(newScooterId));

            ScooterService.RemoveScooter(newScooterId);

            Assert.IsNull(ScooterService.GetScooterById(newScooterId));
        }

        [Test]
        public void RemoveScooterNotExisted()
        {
            var scooterId = "1";
            Assert.IsNull(ScooterService.GetScooterById(scooterId));

            Assert.DoesNotThrow(() => ScooterService.RemoveScooter(scooterId));
        }
    }
}