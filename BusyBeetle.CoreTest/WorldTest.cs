using System;
using System.Drawing;
using BusyBeetle.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BusyBeetle.CoreTest
{
    [TestFixture]
    public class WorldTest : IDisposable
    {
        [SetUp]
        public void Setup()
        {
            IWorldFactory worldFactory = CoreKernel.Get<WorldFactory>();
            _testee = worldFactory.Create(100, 100, GameType.BusyBeetle);
        }

        [TearDown]
        public void TearDown()
        {
            _testee.Stop();
        }

        private IWorld _testee;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _testee.Dispose();
            }
        }

        [Test]
        public void GetAt_WhenWorldIsEmpty_ThenReturnedPixelIsWhite()
        {
            // arrange
            _testee.SetAt(50, 50, Color.White);

            // act
            Color color = _testee.GetAt(50, 50);

            // assert
            color.ToArgb().ShouldBeEquivalentTo(Color.White.ToArgb());
        }

        [Test]
        public void SetAt_WhenWorldIsEmpty_ThenGivenPixelHasGivenColor()
        {
            // arrange
            Color color = Color.Red;

            // act
            _testee.SetAt(50, 50, color);

            // assert
            _testee.GetAt(50, 50).ToArgb().ShouldBeEquivalentTo(color.ToArgb());
        }
    }
}