using Custom.Math;

namespace DCExtractorX.Tests
{
    public class MathHelpersTests
    {
        [Fact]
        public void ClampToZero_ValueBelowEpsilon_ReturnsZero()
        {
            Assert.Equal(0.0f, MathHelpers.ClampToZero(0.000001f));
        }

        [Fact]
        public void ClampToZero_ValueAboveEpsilon_ReturnsValue()
        {
            Assert.Equal(1.0f, MathHelpers.ClampToZero(1.0f));
        }

        [Fact]
        public void IsZero_NearZero_ReturnsTrue()
        {
            Assert.True(MathHelpers.IsZero(0.0000001f));
        }

        [Fact]
        public void IsZero_NotNearZero_ReturnsFalse()
        {
            Assert.False(MathHelpers.IsZero(1.0f));
        }

        [Fact]
        public void CloseToEqual_NearlyEqualValues_ReturnsTrue()
        {
            Assert.True(MathHelpers.CloseToEqual(1.0f, 1.0000001f));
        }

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(180.0, System.Math.PI)]
        [InlineData(360.0, System.Math.PI * 2)]
        public void ToRadians_ConvertsDegreesCorrectly(double degrees, double expectedRadians)
        {
            Assert.Equal(expectedRadians, MathHelpers.ToRadians(degrees), 6);
        }

        [Fact]
        public void ToDegrees_ConvertsRadiansCorrectly()
        {
            Assert.Equal(180.0, MathHelpers.ToDegrees(System.Math.PI), 6);
        }

        [Theory]
        [InlineData(0.0f, 0.0f)]
        [InlineData(0.5f, 5.0f)]
        [InlineData(1.0f, 10.0f)]
        public void Lerp_InterpolatesBetweenValues(float t, float expected)
        {
            Assert.Equal(expected, MathHelpers.Lerp(0.0f, 10.0f, t), 3);
        }
    }
}
