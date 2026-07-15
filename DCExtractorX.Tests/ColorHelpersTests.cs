using Custom.Data;

namespace DCExtractorX.Tests
{
    public class ColorHelpersTests
    {
        [Fact]
        public void FromABGR1555_AllZeroBytes_IsFullyTransparentBlack()
        {
            byte[] bytes = { 0x00, 0x00 };
            System.Drawing.Color c = ColorHelpers.FromABGR1555(bytes, 0);

            Assert.Equal(0, c.A);
            Assert.Equal(0, c.R);
            Assert.Equal(0, c.G);
            Assert.Equal(0, c.B);
        }

        [Fact]
        public void FromABGR1555_AlphaBitOnly_IsFullyOpaqueBlack()
        {
            // byte0 bit7 is the alpha bit; every other bit is zero.
            byte[] bytes = { 0x80, 0x00 };
            System.Drawing.Color c = ColorHelpers.FromABGR1555(bytes, 0);

            Assert.Equal(255, c.A);
            Assert.Equal(0, c.R);
            Assert.Equal(0, c.G);
            Assert.Equal(0, c.B);
        }

        [Fact]
        public void FromABGR1555_HighGreenBitsOnly_ProducesExpectedGreenScaled()
        {
            // byte1's top 3 bits (0xE0) feed the low 3 bits of the 5-bit green channel.
            byte[] bytes = { 0x00, 0xE0 };
            System.Drawing.Color c = ColorHelpers.FromABGR1555(bytes, 0);

            Assert.Equal(0, c.A);
            Assert.Equal(0, c.R);
            Assert.Equal(57, c.G); // 7 * (255/31), truncated
            Assert.Equal(0, c.B);
        }

        [Fact]
        public void FromBufferRGB24Bit_ConvertsTriplets()
        {
            byte[] buffer = { 10, 20, 30, 40, 50, 60 };
            System.Drawing.Color[] colors = ColorHelpers.FromBufferRGB24Bit(buffer, buffer.Length);

            Assert.Equal(2, colors.Length);
            Assert.Equal(255, colors[0].A);
            Assert.Equal(10, colors[0].R);
            Assert.Equal(20, colors[0].G);
            Assert.Equal(30, colors[0].B);
        }

        [Fact]
        public void FromBufferRGBA32Bit_AppliesAlphaTimesTwoHack()
        {
            // HACK (CONFIRMED BY CODE, ColorHelpers.cs): alpha is doubled to compensate for
            // half-intensity alpha found in DC2 32-bit textures. Documented, not "fixed" here.
            byte[] buffer = { 10, 20, 30, 100 };
            System.Drawing.Color[] colors = ColorHelpers.FromBufferRGBA32Bit(buffer, buffer.Length);

            Assert.Single(colors);
            Assert.Equal(200, colors[0].A);
        }

        [Fact]
        public void FromBufferRGBA32Bit_AlphaClampsAt255()
        {
            byte[] buffer = { 10, 20, 30, 200 };
            System.Drawing.Color[] colors = ColorHelpers.FromBufferRGBA32Bit(buffer, buffer.Length);

            Assert.Equal(255, colors[0].A);
        }
    }
}
