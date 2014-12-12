using System.Drawing;
using BusyBeetle.Core.Direction;
using FakeItEasy;
using BusyBeetle.Core;
using NUnit.Framework;

namespace BusyBeetle.CoreTest
{
    [TestFixture]
    public class BeetleTest
    {
        private IWorld _world;
        private Beetle _testee;
        private readonly Color _testeeColor = Color.Green;
        private int posValue = 50;

        [SetUp]
        public void Setup()
        {
            _world = A.Fake<IWorld>();
            _world.Width = 100;
            _world.Height = 100;
            _testee = new Beetle(50, 50, _testeeColor, _world);
        }

        [Test]
        public void UpdateColorAndDirection_WhenEncounteredColorIsOwnColor_ThenSetPixelWhite()
        {
            // arrange
            A.CallTo(() => _world.GetAt(50, 50)).Returns(_testeeColor);

            // act
            _testee.UpdateColorAndDirection();

            // assert
            A.CallTo(() => _world.SetAt(50, 50, Color.White)).MustHaveHappened();
        }

        [Test]
        public void UpdateColorAndDirection_WhenEncounteredColorIsNotOwnColor_ThenSetPixelOwnColor()
        {
            // arrange
            A.CallTo(() => _world.GetAt(50, 50)).Returns(Color.Blue);
            // act
            _testee.UpdateColorAndDirection();
            // assert
            A.CallTo(() => _world.SetAt(50, 50, _testeeColor)).MustHaveHappened();
        }

        [Test]
        public void CheckBorder_WhenPositionXIsGreaterThanWorldWidth_ThenSetPositionXZero()
        {
            // arrange
            _testee.PositionX = 100;
            // act
            _testee.CheckBorder();
            // assert
            Assert.AreEqual(_testee.PositionX, 0);
        }

        [Test]
        public void CheckBorder_WhenPositionXIsLessThanZero_ThenSetPositionXWorldWidthMinusOne()
        {
            // arrange
            _testee.PositionX = -1;
            // act
            _testee.CheckBorder();
            // assert
            Assert.AreEqual(_testee.PositionX, _world.Width - 1);
        }

        [Test]
        public void CheckBorder_WhenPositionYIsGreaterThanWorldHeight_ThenSetPositionYZero()
        {
            // arrange
            _testee.PositionY = 100;
            // act
            _testee.CheckBorder();
            // assert
            Assert.AreEqual(_testee.PositionY, 0);
        }

        [Test]
        public void CheckBorder_WhenPositionYIsLessThanZero_ThenSetPositionYWorldWidthMinusOne()
        {
            // arrange
            _testee.PositionY = -1;
            // act
            _testee.CheckBorder();
            // assert
            Assert.AreEqual(_testee.PositionY, _world.Width - 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsUp_ThenIncrementPositionY()
        {
            // arrange
            _testee.Direction = Direction.Up;
            _testee.PositionY = posValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionY, posValue + 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsRight_ThenIncrementPositionX()
        {
            // arrange
            _testee.Direction = Direction.Right;
            _testee.PositionX = posValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionX, posValue + 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsDown_ThenDecrementPositionY()
        {
            // arrange
            _testee.Direction = Direction.Down;
            _testee.PositionY = posValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionY, posValue - 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsLeft_ThenDecrementPositionX()
        {
            // arrange
            _testee.Direction = Direction.Left;
            _testee.PositionX = posValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionX, posValue - 1);
        }
    }
}
