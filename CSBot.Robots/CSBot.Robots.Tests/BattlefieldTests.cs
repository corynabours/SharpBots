using NUnit.Framework;

namespace CSBot.Robots.Tests
{
    [TestFixture]
    public class BattlefieldTests
    {
        private Battlefield _battlefield;

        [SetUp]
        public void Setup()
        {
            _battlefield = new Battlefield(60, 60, 1000, 60);
        }
        
        [Test]
        public void ShouldCreateANewBattlefield()
        {
            Assert.That(_battlefield.Width,Is.EqualTo(60));
            Assert.That(_battlefield.Height, Is.EqualTo(60));
            Assert.That(_battlefield.Timeout, Is.EqualTo(1000));
            Assert.That(_battlefield.Seed, Is.EqualTo(60));
        }

        [Test]
        public void ShouldBeAbleToAddABullet()
        {
            var bullet = new Bullet(_battlefield, 20, 21, 180, 2, 5, null);
            _battlefield.Add(bullet);
            Assert.That(_battlefield.NewBullets.Count, Is.EqualTo(1));
            Assert.That(_battlefield.Bullets.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldBeAbleToAddAnExplosion()
        {
            var explosion = new Explosion(20, 22);
            _battlefield.Add(explosion);
            Assert.That(_battlefield.NewExplosions.Count, Is.EqualTo(1));
            Assert.That(_battlefield.Explosions.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldBeAbleToAddARobot()
        {
            var robot = new RobotRunner(null, _battlefield, 1);
            _battlefield.Add(robot);
            Assert.That(_battlefield.Robots.Count, Is.EqualTo(1));
        }

        [Test]
        public void ShouldRemoveDeadExplosionInATick()
        {
            var explosion = new Explosion(20, 22);
            _battlefield.Add(explosion);
            for (var i = 0;i<16;i++)
                explosion.Tick();
            _battlefield.Tick();
            Assert.That(_battlefield.NewExplosions.Count,Is.EqualTo(0));
            Assert.That(_battlefield.Explosions.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShouldIncrementTickCountInAnExplosion()
        {
            var explosion = new Explosion(20, 22);
            _battlefield.Add(explosion);
            _battlefield.Tick();
            Assert.That(explosion.T,Is.EqualTo(1));            
        }

        [Test]
        public void ShouldDeleteExplosionOn17ThTick()
        {
            var explosion = new Explosion(20, 22);
            _battlefield.Add(explosion);
            for (var i = 0; i < 17; i++)
                _battlefield.Tick();
            Assert.That(_battlefield.Explosions.Count, Is.EqualTo(0));
        }

        [Test]
        public void ShouldBeAbleToRemoveADeadBulletInaTick()
        {
            var bullet = new Bullet(_battlefield, 20, 21, 180, 2, 5, null);
            _battlefield.Add(bullet);
            bullet.Dead = true;
            _battlefield.Tick();
            Assert.That(_battlefield.Bullets.Count, Is.EqualTo(0));
        }

        [Test]
        public void BulletShouldMoveEachTick()
        {
            var bullet = new Bullet(_battlefield, 20, 21, 180, 2, 5, null);
            _battlefield.Add(bullet);
            _battlefield.Tick();
            Assert.That(bullet.X, Is.EqualTo(18.0d));
            Assert.That(bullet.Y, Is.EqualTo(21.0d));
        }

        [Test]
        public void UiShouldBeAbleToMarkAnExplosionAsAddedToUi()
        {
            var explosion = new Explosion(20, 22);
            _battlefield.Add(explosion);
            _battlefield.Process(explosion);
            Assert.That(_battlefield.Explosions.Count, Is.EqualTo(1));
            Assert.That(_battlefield.NewExplosions.Count, Is.EqualTo(0));
        }

        [Test]
        public void UiShouldBeAbleToMarkABulletAsAddedToUi()
        {
            var bullet = new Bullet(_battlefield, 20, 21, 180, 2, 5, null);
            _battlefield.Add(bullet);
            _battlefield.Process(bullet);
            Assert.That(_battlefield.Bullets.Count, Is.EqualTo(1));
            Assert.That(_battlefield.NewBullets.Count, Is.EqualTo(0));
        }

    }
}
