using Custom.Data;

namespace DCExtractorX.Tests
{
    public class SwizzleTests
    {
        [Fact]
        public void UnSwizzle_PowerOfTwoDimensions_ReturnsFullBuffer()
        {
            const int width = 16;
            const int height = 16;
            byte[] buffer = new byte[width * height];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)i;

            byte[] result = Swizzle.UnSwizzle(buffer, width, height);

            Assert.Equal(width * height, result.Length);
        }

        [Fact]
        public void UnSwizzle_NonPowerOfTwoDimensions_DoesNotThrow()
        {
            // KNOWN LIMITATION (CONFIRMED BY CODE, Swizzle.cs): non-power-of-2 images (e.g. 256x255)
            // gracefully bail out of the unswizzle loop instead of throwing IndexOutOfRangeException,
            // at the cost of losing the last row of data. This test locks in that documented behavior.
            const int width = 16;
            const int height = 15;
            byte[] buffer = new byte[width * height];

            byte[] result = Swizzle.UnSwizzle(buffer, width, height);

            Assert.Equal(width * height, result.Length);
        }

        [Fact]
        public void UnSwizzle4Bit_DoesNotThrow_AndReturnsExpectedLength()
        {
            const int width = 16;
            const int height = 16;
            byte[] buffer = new byte[(width * height) / 2];

            byte[] result = Swizzle.UnSwizzle4Bit(buffer, width, height);

            Assert.Equal(width * height, result.Length);
        }
    }
}
