using System.Drawing;
using BusyBeetle.Core.Direction;
using FakeItEasy;
using BusyBeetle.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BusyBeetle.CoreTest
{
    [TestFixture]
    public class BeetleTest
    {
        private IWorld _world;
        private Beetle _testee;
        private readonly Color _testeeColor = Color.Green;
        private const int PosValue = 50;

        [SetUp]
        public void Setup()
        {
            _world = A.Fake<IWorld>();
            _world.Width = 100;
            _world.Height = 100;
            _testee = new Beetle(50, 50, _testeeColor);
        }

        [Test]
        public void UpdateColorAndDirection_WhenEncounteredColorIsOwnColor_ThenSetPixelWhite()
        {
            // arrange
            A.CallTo(() => _world.GetAt(_testee.PositionX, _testee.PositionY)).Returns(_testeeColor);

            // act
            Color actualColor = _testee.UpdateColorAndDirection(_world.GetAt(_testee.PositionX, _testee.PositionY));

            // assert
            actualColor.ShouldBeEquivalentTo(Color.White);
        }

        [Test]
        public void UpdateColorAndDirection_WhenEncounteredColorIsNotOwnColor_ThenSetPixelOwnColor()
        {
            // arrange
            A.CallTo(() => _world.GetAt(_testee.PositionX, _testee.PositionY)).Returns(Color.Blue);
            // act
            Color actualColor = _testee.UpdateColorAndDirection(_world.GetAt(_testee.PositionX, _testee.PositionY));
            // assert
            actualColor.ShouldBeEquivalentTo(_testeeColor);
        }

        [Test]
        public void CheckBorder_WhenPositionXIsGreaterThanWorldWidth_ThenSetPositionXZero()
        {
            // arrange
            _testee.PositionX = 100;
            // act
            _testee.ClampPosition(_world.Width, _world.Height);
            // assert
            Assert.AreEqual(_testee.PositionX, 0);
        }

        [Test]
        public void CheckBorder_WhenPositionXIsLessThanZero_ThenSetPositionXWorldWidthMinusOne()
        {
            // arrange
            _testee.PositionX = -1;
            // act
            _testee.ClampPosition(_world.Width, _world.Height);
            // assert
            Assert.AreEqual(_testee.PositionX, _world.Width - 1);
        }

        [Test]
        public void CheckBorder_WhenPositionYIsGreaterThanWorldHeight_ThenSetPositionYZero()
        {
            // arrange
            _testee.PositionY = 100;
            // act
            _testee.ClampPosition(_world.Width, _world.Height);
            // assert
            Assert.AreEqual(_testee.PositionY, 0);
        }

        [Test]
        public void CheckBorder_WhenPositionYIsLessThanZero_ThenSetPositionYWorldWidthMinusOne()
        {
            // arrange
            _testee.PositionY = -1;
            // act
            _testee.ClampPosition(_world.Width, _world.Height);
            // assert
            Assert.AreEqual(_testee.PositionY, _world.Width - 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsUp_ThenIncrementPositionY()
        {
            // arrange
            _testee.Direction = Direction.Up;
            _testee.PositionY = PosValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionY, PosValue + 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsRight_ThenIncrementPositionX()
        {
            // arrange
            _testee.Direction = Direction.Right;
            _testee.PositionX = PosValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionX, PosValue + 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsDown_ThenDecrementPositionY()
        {
            // arrange
            _testee.Direction = Direction.Down;
            _testee.PositionY = PosValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionY, PosValue - 1);
        }

        [Test]
        public void MoveStraight_WhenDirectionIsLeft_ThenDecrementPositionX()
        {
            // arrange
            _testee.Direction = Direction.Left;
            _testee.PositionX = PosValue;
            // act
            _testee.MoveStraight();
            // assert
            Assert.AreEqual(_testee.PositionX, PosValue - 1);
        }
    }
}
