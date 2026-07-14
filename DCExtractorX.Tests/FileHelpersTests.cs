using DCExtractor.Data;

namespace DCExtractorX.Tests
{
    public class FileHelpersTests
    {
        [Fact]
        public void ChangeExtension_FileWithExtension_ReplacesIt()
        {
            string result = FileHelpers.ChangeExtension(@"C:\models\enemy.mds", "smd");
            Assert.Equal(@"C:\models\enemy.smd", result);
        }

        [Fact]
        public void ChangeExtension_FileWithoutExtension_AppendsIt()
        {
            string result = FileHelpers.ChangeExtension(@"C:\models\enemy", "smd");
            Assert.Equal(@"C:\models\enemy.smd", result);
        }

        [Fact]
        public void ChangeExtension_ExtensionWithoutDot_IsNormalized()
        {
            string result = FileHelpers.ChangeExtension(@"C:\models\enemy.mds", "obj");
            Assert.Equal(@"C:\models\enemy.obj", result);
        }
    }
}
