using FF8_MaxDraw_Patcher.Utils;

namespace FF8_MaxDraw_Patcher_Tests
{
    public class ByteUtilTests
    {
        // ----------------------------
        // FromHexString
        // ----------------------------

        [Fact]
        public void FromHexString_BasicHex_ParsesCorrectly()
        {
            // Act
            var result = ByteUtil.FromHexString("0A1B2C");

            // Assert
            Assert.Equal(new byte[] { 0x0A, 0x1B, 0x2C }, result);
        }

        [Fact]
        public void FromHexString_IgnoresWhitespace()
        {
            // Act
            var result = ByteUtil.FromHexString("0A 1B 2C");

            // Assert
            Assert.Equal(new byte[] { 0x0A, 0x1B, 0x2C }, result);
        }

        [Fact]
        public void FromHexString_IgnoresExcessiveWhitespace()
        {
            // Act
            var result = ByteUtil.FromHexString("  0A 1B     2C  ");

            // Assert
            Assert.Equal(new byte[] { 0x0A, 0x1B, 0x2C }, result);
        }

        [Fact]
        public void FromHexString_Allows0xPrefix()
        {
            // Act
            var result = ByteUtil.FromHexString("0xDEAD");

            // Assert
            Assert.Equal(new byte[] { 0xDE, 0xAD }, result);
        }

        [Fact]
        public void FromHexString_Allows0xPrefixCapitalized()
        {
            // Act
            var result = ByteUtil.FromHexString("0XD1A5");

            // Assert
            Assert.Equal(new byte[] { 0xD1, 0xA5 }, result);
        }

        [Fact]
        public void FromHexString_Allows0xPrefix_WithWhitespace()
        {
            // Act
            var result = ByteUtil.FromHexString("   0x D1 A5 ");

            // Assert
            Assert.Equal(new byte[] { 0xD1, 0xA5 }, result);
        }

        [Fact]
        public void FromHexString_EmptyString_ReturnsEmpty()
        {
            // Act
            var result = ByteUtil.FromHexString("");

            // Assert
            Assert.Equal(new byte[] { }, result);
        }

        [Fact]
        public void FromHexString_NullString_ThrowsNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ByteUtil.FromHexString(null));
        }

        [Fact]
        public void FromHexString_InvalidHex_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => ByteUtil.FromHexString("ZZZZ"));
        }

        // ----------------------------
        // ConcatArrays
        // ----------------------------


        [Fact]
        public void ConcatArrays_NullArray_ThrowsNullException()
        {
            byte[] a = { 1, 2 };
            byte[] b = { 3, 4 };

            Assert.Throws<ArgumentNullException>(() => ByteUtil.ConcatArrays(a, null, b));
        }

        [Fact]
        public void ConcatArrays_NullArrays_ThrowsNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ByteUtil.ConcatArrays(null));
        }

        [Fact]
        public void ConcatArrays_BasicConcat()
        {
            byte[] a = { 1, 2 };
            byte[] b = { 3, 4 };
            byte[] c = { 5 };

            // Act
            var result = ByteUtil.ConcatArrays(a, b, c);

            // Assert
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, result);
        }

        [Fact]
        public void ConcatArrays_BasicConcat_OutOfOrder()
        {
            byte[] a = { 5, 1 };
            byte[] b = { 3, 2 };
            byte[] c = { 4 };

            // Act
            var result = ByteUtil.ConcatArrays(a, b, c);

            // Assert
            Assert.Equal(new byte[] { 5, 1, 3, 2, 4 }, result);
        }

        [Fact]
        public void ConcatArrays_BasicConcat_WithDuplicates()
        {
            byte[] a = { 1, 1 };
            byte[] b = { 3, 2 };
            byte[] c = { 1 };

            // Act
            var result = ByteUtil.ConcatArrays(a, b, c);

            // Assert
            Assert.Equal(new byte[] { 1, 1, 3, 2, 1 }, result);
        }

        [Fact]
        public void ConcatArrays_AllowsEmptyArrays()
        {
            // Act
            var result = ByteUtil.ConcatArrays(Array.Empty<byte>(), new byte[] { 9 }, Array.Empty<byte>());

            // Assert
            Assert.Equal(new byte[] { 9 }, result);
        }

        [Fact]
        public void ConcatArrays_SingleArray_ReturnsSameContent()
        {
            byte[] a = { 10, 20, 30 };

            // Act
            var result = ByteUtil.ConcatArrays(a);

            // Assert
            Assert.Equal(a, result);
        }

        [Fact]
        public void ConcatArrays_NoArrays_ReturnsEmpty()
        {
            // Act
            var result = ByteUtil.ConcatArrays();

            // Assert
            Assert.Empty(result);
        }
    }
}