
namespace TeaSpoons.LargeNumbers.Editor.Tests
{
    using System;
    using NUnit.Framework;
    using System;

    public class LargeIntTest
    {
        [Test]
        public void Constructor()
        {
            Assert.AreEqual(123_400_000, new LargeInt(1234, 4).RawDigits);
            Assert.AreEqual(123_400_000, new LargeInt(1234, 6).RawDigits);
            Assert.AreEqual(123_450_000, new LargeInt(123456, 4).RawDigits);
            Assert.AreEqual(100_000_000, new LargeInt(1234, 0).RawDigits);
        }

        [Test]
        public void SmallNumberAddition()
        {
            var three = new LargeInt(3, 0);
            var six = new LargeInt(6, 0);
            var negativeThree = new LargeInt(-3, 0);
            var negativeSix = new LargeInt(-6, 0);

            Assert.AreEqual(new LargeInt(9, 0), three + six);
            Assert.AreEqual(new LargeInt(9, 0), six + three);

            Assert.AreEqual(three, six - three);
            Assert.AreEqual(LargeInt.Zero, six + negativeSix);
            Assert.AreEqual(six, three - negativeThree);
        }

        [Test]
        public void LargeNumberAddition()
        {
            var gogol = new LargeInt(1, 100);
            var oneE101 = new LargeInt(1, 101);
            var sixPointFiveE102 = new LargeInt(65, 102);

            Assert.AreEqual(new LargeInt(11, 101), gogol + oneE101);
            Assert.AreEqual(new LargeInt(651, 102), gogol + sixPointFiveE102);

            Assert.AreEqual(new LargeInt(649, 102), sixPointFiveE102 - gogol);
        }

        [Test]
        public void LargeAndSmallNumberAddition()
        {
            var gogol = new LargeInt(1, 100);
            var three = new LargeInt(3, 0);

            Assert.AreEqual(gogol, gogol + three);
            Assert.AreEqual(gogol, gogol - three);
        }

        [Test]
        public void SmallNumberMultiplication()
        {
            var three = new LargeInt(3, 0);
            var six = new LargeInt(6, 0);
            var eighteen = new LargeInt(18, 1);

            Assert.AreEqual(eighteen, three * six);
            Assert.AreEqual(new LargeInt(216, 2), six * six * six);

            Assert.AreEqual(six, eighteen / three);
        }

        [Test]
        public void LargeNumberMultiplication()
        {
            var ten = new LargeInt(1, 1);
            var twenty = new LargeInt(2, 1);
            var fourHundred = new LargeInt(4, 2);
            var gogol = new LargeInt(1, 100);
            var oneE101 = new LargeInt(1, 101);

            Assert.AreEqual(new LargeInt(1, 200), gogol * gogol);
            Assert.AreEqual(oneE101, gogol * ten);
        }

        [Test]
        public void LargeNumberIntegerDivision()
        {
            var ten = new LargeInt(1, 1);
            var twenty = new LargeInt(2, 1);
            var fourHundred = new LargeInt(4, 2);
            var gogol = new LargeInt(1, 100);
            var oneE101 = new LargeInt(1, 101);
            var twoE102 = new LargeInt(2, 102);

            // Larger / Smaller
            Assert.AreEqual(ten, oneE101 / gogol);
            Assert.AreEqual(new LargeInt(2, 2), twoE102 / gogol);
            Assert.AreEqual(new LargeInt(2, 101), twoE102 / ten);
            Assert.AreEqual(twenty, fourHundred / twenty);

            // Smaller / Larger
            Assert.AreEqual(LargeInt.Zero, ten / twenty);
            Assert.AreEqual(LargeInt.Zero, gogol / oneE101);
        }

        [Test]
        public void LargeNumberFloatDivision()
        {
            var epsilon = 0.000001;

            var ten = new LargeInt(1, 1);
            var twenty = new LargeInt(2, 1);
            var fourHundred = new LargeInt(4, 2);
            var gogol = new LargeInt(1, 100);
            var oneE101 = new LargeInt(1, 101);

            Assert.That(LargeInt.Divide(ten, twenty), Is.EqualTo(0.5).Within(epsilon));
            Assert.That(LargeInt.Divide(twenty, ten), Is.EqualTo(2).Within(epsilon));
            Assert.That(LargeInt.Divide(ten, ten), Is.EqualTo(1).Within(epsilon));

            Assert.That(LargeInt.Divide(ten, fourHundred), Is.EqualTo(0.025).Within(epsilon));
            Assert.That(LargeInt.Divide(fourHundred, ten), Is.EqualTo(40).Within(epsilon));

            Assert.That(LargeInt.Divide(ten, gogol), Is.EqualTo(0.0).Within(epsilon));
            Assert.That(LargeInt.Divide(gogol, ten), Is.EqualTo(1e99).Within(1e90));

            Assert.That(LargeInt.Divide(gogol, oneE101), Is.EqualTo(0.1).Within(epsilon));
            Assert.That(LargeInt.Divide(oneE101, gogol), Is.EqualTo(10.0).Within(epsilon));
        }

        [Test]
        public void HugeNumbers()
        {
            var wtf = new LargeInt(1, 65_000);
            var omg = new LargeInt(6123, 65_123);

            Assert.AreEqual(omg, wtf + omg);
        }

        [Test]
        public void HugeNegativeNumbers()
        {
            var negativeWtf = new LargeInt(-1, 65_000);
            var wtf = new LargeInt(1, 65_000);

            Assert.AreEqual(new LargeInt(0, 0), wtf + negativeWtf);
        }

        [Test]
        public void ImplicitConversionFromLong()
        {
            Assert.AreEqual(new LargeInt(12345, 4), (LargeInt)12345);
            Assert.AreEqual(new LargeInt(-12345, 4), (LargeInt)(-12345));

            Assert.AreEqual(new LargeInt(12345, 4), new LargeInt(1, 4) + 2345);
            Assert.AreEqual(new LargeInt(7655, 3), new LargeInt(1, 4) - 2345);

            Assert.IsTrue(new LargeInt(1, 2) == 100);
            Assert.IsFalse(new LargeInt(1, 2) == 123);
            Assert.IsTrue(new LargeInt(1, 2) != 123);
            Assert.IsTrue(100 == new LargeInt(1, 2));
            Assert.IsTrue(new LargeInt(123, 2) == 123);
        }

        [Test]
        public void ConversionToInt()
        {
            Assert.AreEqual(0, LargeInt.Zero.GetInt());
            Assert.AreEqual(1, new LargeInt(1, 0).GetInt());
            Assert.AreEqual(-1, new LargeInt(-1, 0).GetInt());

            Assert.AreEqual(9_999, new LargeInt(9_999, 3).GetInt());
            Assert.AreEqual(-9_999, new LargeInt(-9_999, 3).GetInt());

            Assert.AreEqual(100_001, new LargeInt(100_001, 5).GetInt());

            var almostIntMaxValue = 2_147_483_000;
            Assert.AreEqual(almostIntMaxValue, new LargeInt(almostIntMaxValue, 9).GetInt());
            Assert.AreEqual(-almostIntMaxValue, new LargeInt(-almostIntMaxValue, 9).GetInt());
        }
        
        [Test]
        public void ConversionToLong()
        {
            Assert.AreEqual(0L, LargeInt.Zero.GetLong());
            Assert.AreEqual(1L, new LargeInt(1, 0).GetLong());
            Assert.AreEqual(-1L, new LargeInt(-1, 0).GetLong());

            Assert.AreEqual(9_999L, new LargeInt(9_999, 3).GetLong());
            Assert.AreEqual(-9_999L, new LargeInt(-9_999, 3).GetLong());
            
            Assert.AreEqual(9_000_000_000_000_000_000L, new LargeInt(9, 18).GetLong());
            Assert.AreEqual(-9_000_000_000_000_000_000L, new LargeInt(-9, 18).GetLong());
            
            var over1 = new LargeInt(1, 19);
            
            Assert.Throws<Exception>(() => over1.GetLong());
            Assert.False(over1.TryGetLong(out _));
            
            var over2 = new LargeInt(3, 19);
            Assert.Throws<Exception>(() => over2.GetLong());
            Assert.False(over2.TryGetLong(out _));
        }

        [Test]
        public void Comparison()
        {
            // Positive numbers with same exponent
            Assert.IsTrue(new LargeInt(2, 2) > new LargeInt(1, 2));
            Assert.IsTrue(new LargeInt(2, 2) < new LargeInt(3, 2));

            // Positive numbers with different exponent
            Assert.IsTrue(new LargeInt(1, 9) > new LargeInt(987, 8));
            Assert.IsTrue(new LargeInt(987, 8) < new LargeInt(1, 9));

            // Equal numbers
            Assert.IsTrue(new LargeInt(2, 2) >= new LargeInt(2, 2));
            Assert.IsTrue(new LargeInt(2, 2) <= new LargeInt(2, 2));
            Assert.IsFalse(new LargeInt(2, 2) > new LargeInt(2, 2));
            Assert.IsFalse(new LargeInt(2, 2) < new LargeInt(2, 2));

            // Numbers with different signs
            Assert.IsTrue(new LargeInt(1, 2) > new LargeInt(-2, 2));
            Assert.IsTrue(new LargeInt(-1, 2) < new LargeInt(9, 1));

            // Negative numbers with same exponent
            Assert.IsTrue(new LargeInt(-1, 2) > new LargeInt(-3, 2));
            Assert.IsTrue(new LargeInt(-3, 2) < new LargeInt(-1, 2));
            Assert.IsTrue(new LargeInt(-1, 2) >= new LargeInt(-3, 2));
            Assert.IsTrue(new LargeInt(-3, 2) <= new LargeInt(-1, 2));

            // Negative numbers with different exponent
            Assert.IsTrue(new LargeInt(-1, 3) < new LargeInt(-1, 2));
            Assert.IsTrue(new LargeInt(-1, 2) > new LargeInt(-1, 3));
            Assert.IsTrue(new LargeInt(-1, 3) <= new LargeInt(-1, 2));
            Assert.IsTrue(new LargeInt(-1, 2) >= new LargeInt(-1, 3));
        }

        [Test]
        public void Parsing()
        {
            // Missing values (Null, empty)
            Assert.Throws<ArgumentNullException>(() => LargeInt.Parse(null));
            Assert.Throws<FormatException>(() => LargeInt.Parse(string.Empty));

            // Wrong/Undefined formatting
            var badParseInputs = new[] { "not a number", "1.2.3", "123e", "e123", "1e2e3" };
            foreach (var s in badParseInputs)
            {
                Assert.Throws<FormatException>(() => LargeInt.Parse(s));
            }

            // Check normal behavior
            void CheckParse(string text, long expectedRaw, ushort expectedExp)
            {
                var li = LargeInt.Parse(text);
                Assert.AreEqual(expectedRaw, li.RawDigits, $"RawDigits for \"{text}\"");
                Assert.AreEqual(expectedExp, li.Exponent, $"Exponent for \"{text}\"");
            }

            CheckParse("0", 0L, 0);
            CheckParse("123", 123_000_000L, 2);
            CheckParse("-456", -456_000_000L, 2);
            CheckParse("+756", 756_000_000L, 2);
            CheckParse("1.23e2", 123_000_000L, 2);
            CheckParse("+1.23e2", 123_000_000L, 2);
            CheckParse("-1.23E2", -123_000_000L, 2);
            CheckParse("1_234", 123_400_000L, 3);
            CheckParse("-5_678", -567_800_000L, 3);
            CheckParse("+1_234e3", 123_400_000L, 3);
            CheckParse("-5_678E3", -567_800_000L, 3);
        }

        [Test]
        public void TryParsing()
        {
            // Missing values (Null, empty)
            Assert.IsFalse(LargeInt.TryParse(null, out var li));
            Assert.AreEqual(LargeInt.Zero, li);
            Assert.IsFalse(LargeInt.TryParse("", out li));
            Assert.AreEqual(LargeInt.Zero, li);

            // Wrong/Undefined formatting
            var badTryInputs = new[] { "not a number", "1.2.3", "123e", "e123", "1e2e3" };
            foreach (var s in badTryInputs)
            {
                var ok = LargeInt.TryParse(s, out var tvBad);
                Assert.IsFalse(ok, $"TryParse(\"{s}\") should be false");
                Assert.AreEqual(LargeInt.Zero, tvBad, $"Value for \"{s}\"");
            }

            // Check normal behavior
            void CheckTryParse(string text, long expectedRaw, ushort expectedExp)
            {
                LargeInt.TryParse(text, out li);
                Assert.AreEqual(expectedRaw, li.RawDigits, $"RawDigits for \"{text}\"");
                Assert.AreEqual(expectedExp, li.Exponent, $"Exponent for \"{text}\"");
            }

            CheckTryParse("0", 0L, 0);
            CheckTryParse("123", 123_000_000L, 2);
            CheckTryParse("+456", 456_000_000L, 2);
            CheckTryParse("-789", -789_000_000L, 2);
            CheckTryParse("3.21e3", 321_000_000L, 3);
            CheckTryParse("+3.21e3", 321_000_000L, 3);
            CheckTryParse("-3.21e3", -321_000_000L, 3);
            CheckTryParse("1_234", 123_400_000L, 3);
            CheckTryParse("-5_678", -567_800_000L, 3);
            CheckTryParse("+1_234e3", 123_400_000L, 3);
            CheckTryParse("-5_678E3", -567_800_000L, 3);
        }

        /// <summary>
        /// This test makes sure that we have 8 significant bits and are not losing precision in the range just below a billion (10^9).
        /// </summary>
        [Test]
        public void SomeValuesUnderOneBillionArePrecise()
        {
            const int startingValue = 990_000_000;
            const int maxValue = 999_999_999;
            var largeInt = (LargeInt)startingValue;

            for (var number = startingValue + 1; number < maxValue; number++)
            {
                largeInt += 1;
                Assert.AreEqual(number, largeInt.GetInt());
            }
        }

        [Test]
        public void RawDigitPrecision()
        {
            Assert.AreEqual(777_777_770, ((LargeInt)77_777_777).RawDigits);
            Assert.AreEqual(777_777_777, ((LargeInt)777_777_777).RawDigits);
            Assert.AreEqual(777_777_778, ((LargeInt)777_777_778).RawDigits);
            Assert.AreEqual(777_777_777, ((LargeInt)7_777_777_777).RawDigits);
            Assert.AreEqual(777_777_778, ((LargeInt)7_777_777_787).RawDigits);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual((LargeInt)123_456_789, (LargeInt)123_456_789);

            // Still equal because there's only 9 significant digits
            Assert.AreEqual((LargeInt)1_234_567_893, (LargeInt)1_234_567_898);
            // Not equal because the 9th significant digit is different
            Assert.AreNotEqual((LargeInt)1_234_567_883, (LargeInt)1_234_567_893);
        }
    }
}
