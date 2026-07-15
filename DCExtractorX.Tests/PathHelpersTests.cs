using Custom.IO;

namespace DCExtractorX.Tests
{
    public class PathHelpersTests
    {
        // NOTE: SubPathOf treats any path segment containing "." as a file and strips it down to its
        // containing directory (via Path.GetDirectoryName) before computing the subpath. In the legacy
        // codebase's only caller (FileHelpers.MoveFiles), the child argument is always pre-stripped to a
        // directory already. These tests lock in the actual, current behavior rather than idealized behavior.
        [Fact]
        public void SubPathOf_ChildDirectoryUnderParent_ReturnsRelativeSubpath()
        {
            string result = PathHelpers.SubPathOf(@"C:\Game\Data", @"C:\Game\Data\Models", false);
            Assert.Equal("Models", result);
        }

        [Fact]
        public void SubPathOf_ChildFilePathUnderParent_StripsFileNameThenReturnsDirectorySubpath()
        {
            string result = PathHelpers.SubPathOf(@"C:\Game\Data", @"C:\Game\Data\Models\enemy.mds", false);
            Assert.Equal("Models", result);
        }

        [Fact]
        public void SubPathOf_DifferentDrive_ReturnsChildPathUnchanged()
        {
            string result = PathHelpers.SubPathOf(@"C:\Game\Data", @"D:\Other\file.mds", false);
            Assert.Equal(@"D:\Other\file.mds", result);
        }

        [Fact]
        public void SubPathOf_IncludeParent_IncludesLastParentDirectory()
        {
            string result = PathHelpers.SubPathOf(@"C:\Game\Data", @"C:\Game\Data\Models\enemy.mds", true);
            Assert.Equal(@"Data\Models", result);
        }
    }
}
