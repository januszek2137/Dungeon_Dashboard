namespace Dungeon_Dashboard.Controllers.Tests {

    [TestClass()]
    public class CharacterStatCounterTests {
        private readonly CharacterStatCounter __statCalculator = new CharacterStatCounter();

        [TestMethod()]
        public void CharacterStatCounterTest() {
            Assert.Fail();
        }

        [TestMethod()]
        public void CalculateProficiencyBonusTest() {
            Assert.Fail();
        }

        [TestMethod()]
        public void CalculateStatModifier_StatIs10_Returns0() {
            int stat = 10;
            int expectedModifier = 0;

            int actualModifier = __statCalculator.CalculateStatModifier(stat);

            Assert.AreEqual(expectedModifier, actualModifier);
        }

        [TestMethod]
        public void CalculateStatModifier_StatIs8_ReturnsMinus1() {
            int stat = 8;
            int expectedModifier = -1;

            int actualModifier = __statCalculator.CalculateStatModifier(stat);

            Assert.AreEqual(expectedModifier, actualModifier);
        }

        [TestMethod]
        public void CalculateStatModifier_StatIs12_ReturnsPlus1() {
            int stat = 12;
            int expectedModifier = +1;

            int actualModifier = __statCalculator.CalculateStatModifier(stat);

            Assert.AreEqual(expectedModifier, actualModifier);
        }

        [TestMethod]
        public void CalculateStatModifier_StatIs20_ReturnsPlus5() {
            int stat = 20;
            int expectedModifier = +5;

            int actualModifier = __statCalculator.CalculateStatModifier(stat);

            Assert.AreEqual(expectedModifier, actualModifier);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CalculateStatModifier_StatIsNegative_ThrowsException() {
            int stat = -5;

            __statCalculator.CalculateStatModifier(stat);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CalculateStatModifier_StatIs31_ThrowsException() {
            int stat = 31;

            __statCalculator.CalculateStatModifier(stat);
        }

        [TestMethod]
        public void CalculatePassiveWisdom_Wisdom10_Returns10() {
            int wisdom = 10;
            int expected = 10;

            int actual = __statCalculator.CalculatePassiveWisdom(wisdom);

            Assert.AreEqual(expected, actual, "Pasywna mądrość dla Wisdom 10 powinna wynosić 10.");
        }

        [TestMethod]
        public void CalculatePassiveWisdom_Wisdom12_Returns11() {
            int wisdom = 12;
            int expected = 11;

            int actual = __statCalculator.CalculatePassiveWisdom(wisdom);

            Assert.AreEqual(expected, actual, "Pasywna mądrość dla Wisdom 12 powinna wynosić 11.");
        }

        [TestMethod]
        public void CalculatePassiveWisdom_Wisdom14_Returns12() {
            int wisdom = 14;
            int expected = 12;

            int actual = __statCalculator.CalculatePassiveWisdom(wisdom);

            Assert.AreEqual(expected, actual, "Pasywna mądrość dla Wisdom 14 powinna wynosić 12.");
        }

        [TestMethod]
        public void CalculatePassiveWisdom_Wisdom8_Returns9() {
            int wisdom = 8;
            int expected = 9;

            int actual = __statCalculator.CalculatePassiveWisdom(wisdom);

            Assert.AreEqual(expected, actual, "Pasywna mądrość dla Wisdom 8 powinna wynosić 9.");
        }

        [TestMethod]
        public void CalculatePassiveWisdom_Wisdom20_Returns15() {
            int wisdom = 20;
            int expected = 15;

            int actual = __statCalculator.CalculatePassiveWisdom(wisdom);

            Assert.AreEqual(expected, actual, "Pasywna mądrość dla Wisdom 20 powinna wynosić 15.");
        }

        [TestMethod]
        public void CalculatePassiveWisdom_NegativeWisdom_ThrowsArgumentOutOfRangeException() {
            int wisdom = -1;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                __statCalculator.CalculatePassiveWisdom(wisdom)
            );
        }
    }
}