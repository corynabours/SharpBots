using NUnit.Framework;

namespace CSBot.Robots.Tests
{
    [TestFixture]
    public class BulletTests
    {
        private Battlefield _battlefield;
        [SetUp]
        public void SetUp()
        {
            _battlefield = new Battlefield(800,800,1000,60);
        }

        [Test]
        public void ShouldBeCreatable()
        {
            var bullet = new Bullet(_battlefield, 20, 21, 180, 20, 3.0m, null);
            Assert.That(bullet.Energy, Is.EqualTo(3.0m));
            Assert.That(bullet.X, Is.EqualTo(20));
            Assert.That(bullet.Y, Is.EqualTo(21));
            Assert.That(bullet.Dead, Is.False);
        }

        [Test]
        public void ShouldDieWhenItExitsTheScreen()
        {
            var bullet = new Bullet(_battlefield, 20, 21, 180, 30, 3.0m, null);
            bullet.Tick();
            Assert.That(bullet.Dead, Is.True);

            bullet = new Bullet(_battlefield, 780, 21, 0, 30, 3.0m, null);
            bullet.Tick();
            Assert.That(bullet.Dead, Is.True);

            bullet = new Bullet(_battlefield, 20, 21, 90, 30, 3.0m, null);
            bullet.Tick();
            Assert.That(bullet.Dead, Is.True);

            bullet = new Bullet(_battlefield, 20, 780, 270, 30, 3.0m, null);
            bullet.Tick();
            Assert.That(bullet.Dead, Is.True);
        }

        [Test]
        public void ShouldDieWhenItHitsARobot()
        {
            var customRobot = new TestRobot();
            var robot = new RobotRunner(customRobot, _battlefield, 1);
            var firingRobot = new RobotRunner(customRobot, _battlefield, 1);
            _battlefield.Add(robot);
            var bullet = new Bullet(_battlefield, robot.X + 30, robot.Y, 180, 30, 3.0m, firingRobot);
            bullet.Tick();
            Assert.That(bullet.Dead, Is.True);
            Assert.That(_battlefield.NewExplosions.Count,Is.EqualTo(1));
            Assert.That(robot.Energy, Is.EqualTo(97.0m));
            Assert.That(firingRobot.DamageGiven, Is.EqualTo(3.0m));
        }
    }
}
