using System.Drawing;
using BusyBeetle.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace BusyBeetle.CoreTest
{
    [TestFixture]
    public class WorldTest
    {
        [SetUp]
        public void Setup()
        {
            _dispatcher = new SynchronusDispatcher();
            
            _testee = new World(_dispatcher, 100, 100);
        }

        private World _testee;
        private IDispatcher _dispatcher;

        [Test]
        public void GetAt_WhenWorldIsEmpty_ThenReturnedPixelIsWhite()
        {
            // arrange

            // act
            Color color = _testee.GetAt(50, 50);

            // assert
            _testee.Stop();
            color.ToArgb().ShouldBeEquivalentTo(Color.White.ToArgb());
        }

        [Test]
        public void SetAt_WhenWorldIsEmpty_ThenGivenPixelHasGivenColor()
        {
            // arrange
            Color color = Color.Red;

            // act
            _testee.SetAt(50, 50, color);
            _testee.Stop();

            // assert
            _testee.Bitmap.GetPixel(50, 50).ToArgb().ShouldBeEquivalentTo(color.ToArgb());
        }
    }
}