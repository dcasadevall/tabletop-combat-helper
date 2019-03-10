using System;
using NUnit.Framework;

namespace Math.Tests {
    [TestFixture]
    public class IntVector2Test {
        [TestCase(2, 1, 1, 1, false)]
        [TestCase(1, 1, 1, 1, true)]
        [TestCase(-1, 1, -1, 1, true)]
        [TestCase(-1, 3, -1, 4, false)]
        [TestCase(-1, 1, 1, 1, false)]
        public void TestEqualityOperator(int x, int y, int secondX, int secondY, bool expectedAreEqual) {
            IntVector2 first = IntVector2.Of(x, y);
            IntVector2 second = IntVector2.Of(secondX, secondY);
            Assert.AreEqual(expectedAreEqual, first == second);
        }
        
        [TestCase(2, 1, 1, 1, true)]
        [TestCase(1, 1, 1, 1, false)]
        [TestCase(-1, 1, -1, 1, false)]
        [TestCase(-1, 3, -1, 4, true)]
        [TestCase(-1, 1, 1, 1, true)]
        public void TestInequalityOperator(int x, int y, int secondX, int secondY, bool expectedAreDifferent) {
            IntVector2 first = IntVector2.Of(x, y);
            IntVector2 second = IntVector2.Of(secondX, secondY);
            Assert.AreEqual(expectedAreDifferent, first != second);
        }

        [TestCase(1, 1, 1, 1, 2, 2)]
        [TestCase(2, 1, 3, 1, 5, 2)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(6, 2, -7, -3, -1, -1)]
        public void TestSumOperator(int x, int y, int secondX, int secondY, int sumX, int sumY) {
            IntVector2 first = IntVector2.Of(x, y);
            IntVector2 second = IntVector2.Of(secondX, secondY);
            IntVector2 sum = IntVector2.Of(sumX, sumY);
            Assert.AreEqual(sum, first + second);
        }
        
        [TestCase(1, 1, 1, 1, 0, 0)]
        [TestCase(2, 1, 3, 1, -1, 0)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(6, 2, -2, 3, 8, -1)]
        public void TestSubtractOperator(int x, int y, int secondX, int secondY, int substractedX, int subtractedY) {
            IntVector2 first = IntVector2.Of(x, y);
            IntVector2 second = IntVector2.Of(secondX, secondY);
            IntVector2 subtracted = IntVector2.Of(substractedX, subtractedY);
            Assert.AreEqual(subtracted, first - second);
        }
        
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(2, -1, 3, 1, 6, -1)]
        [TestCase(6, -2, 0, -3, 0, 6)]
        public void TestMultiplyOperator(int x, int y, int secondX, int secondY, int multiplitedX, int multipliedY) {
            IntVector2 first = IntVector2.Of(x, y);
            IntVector2 second = IntVector2.Of(secondX, secondY);
            IntVector2 multiplication = IntVector2.Of(multiplitedX, multipliedY);
            Assert.AreEqual(multiplication, first * second);
        }
        
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(2, -1, 3, 1, 0, -1)]
        [TestCase(6, -8, 2, -4, 3, 2)]
        public void TestDivisionOperator(int x, int y, int secondX, int secondY, int dividedX, int dividedY) {
            IntVector2 first = IntVector2.Of(x, y);
            IntVector2 second = IntVector2.Of(secondX, secondY);
            IntVector2 division = IntVector2.Of(dividedX, dividedY);
            Assert.AreEqual(division, first / second);
        }

        [Test]
        public void TestGivenDivisionByZero_ThrowsException() {
            try {
                IntVector2 result = IntVector2.One / IntVector2.Zero;
                Assert.Fail("Should throw exception");
            } catch (Exception e) {
                // ignored
            }
        }
    }
}