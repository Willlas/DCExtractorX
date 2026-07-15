using Custom.Math;

namespace DCExtractorX.Tests
{
    public class Matrix4x4Tests
    {
        [Fact]
        public void Identity_HasExpectedDiagonal()
        {
            Matrix4x4 m = new Matrix4x4();
            Assert.Equal(1.0f, m.m[0]);
            Assert.Equal(1.0f, m.m[5]);
            Assert.Equal(1.0f, m.m[10]);
            Assert.Equal(1.0f, m.m[15]);
        }

        [Fact]
        public void MultiplyByIdentity_ReturnsSameMatrix()
        {
            Matrix4x4 translation = Matrix4x4.GenerateTranslation(1.0f, 2.0f, 3.0f);
            Matrix4x4 result = translation * Matrix4x4.identity;

            for (int i = 0; i < 16; i++)
                Assert.Equal(translation.m[i], result.m[i], 5);
        }

        [Fact]
        public void Translation_SetsTranslationComponents()
        {
            Matrix4x4 m = Matrix4x4.GenerateTranslation(1.0f, 2.0f, 3.0f);
            Assert.Equal(1.0f, m.translation.x);
            Assert.Equal(2.0f, m.translation.y);
            Assert.Equal(3.0f, m.translation.z);
        }

        [Fact]
        public void InvertThenMultiplyByOriginal_ReturnsIdentity()
        {
            Matrix4x4 m = Matrix4x4.GenerateTranslation(4.0f, -2.0f, 7.5f);
            Matrix4x4 inverted = m.Inversed();
            Matrix4x4 result = m * inverted;

            for (int i = 0; i < 16; i++)
                Assert.Equal(Matrix4x4.identity.m[i], result.m[i], 3);
        }

        [Fact]
        public void Zero_OnlyClearsFirstElement_PreExistingBug()
        {
            // BUG (CONFIRMED BY CODE, migrated verbatim from the legacy repo's Matrix4x4.cs):
            // Zero()'s loop body is `m[0] = 0.0f;` instead of `m[i] = 0.0f;`, so it only clears the
            // first element and leaves the rest of the matrix untouched. This test locks in the
            // actual (buggy) current behavior; it is intentionally NOT fixed here pending a
            // deliberate, reviewed decision (see Phase 6 reliability fixes) since Zero()/isZero
            // do not appear to be on any load/export hot path today.
            Matrix4x4 m = new Matrix4x4();
            m.Zero();

            Assert.Equal(0.0f, m.m[0]);
            Assert.Equal(1.0f, m.m[5]);  // unaffected identity value survives the buggy loop.
            Assert.False(m.isZero);      // isZero is therefore also affected by this bug.
        }

        [Fact]
        public void IsZero_IdentityMatrix_ReturnsFalse()
        {
            Assert.False(Matrix4x4.identity.isZero);
        }
    }
}
