using DC.IO;

namespace DCExtractorX.Tests
{
    public class DCProgressTests
    {
        [Fact]
        public void ValueChanged_RaisesEventWithNewValue()
        {
            int? observed = null;
            void handler(int v) => observed = v;

            DCProgress.ValueChanged += handler;
            try
            {
                DCProgress.value = 42;
                Assert.Equal(42, observed);
            }
            finally
            {
                DCProgress.ValueChanged -= handler;
            }
        }

        [Fact]
        public void Canceled_ReflectsCancelRequestedCallback()
        {
            var original = DCProgress.CancelRequested;
            try
            {
                DCProgress.CancelRequested = () => true;
                Assert.True(DCProgress.canceled);

                DCProgress.CancelRequested = () => false;
                Assert.False(DCProgress.canceled);
            }
            finally
            {
                DCProgress.CancelRequested = original;
            }
        }
    }
}
