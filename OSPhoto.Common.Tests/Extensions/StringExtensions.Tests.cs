using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Tests.Extensions;

public class StringExtensionsTests
{
    [Test]
    public void CanEncodeThenDecodeAString()
    {
        var str = "pexels-andre-furtado-1264210.jpg";

        var enc = str.ToHex();
        Assert.AreNotEqual(enc, str);
        Assert.IsFalse(enc.Contains('"'));
        Assert.IsFalse(enc.Contains('\''));

        var unenc = enc.FromHex();
        Assert.AreEqual(str, unenc);
    }

    [Test]
    public void CanEncodeThenDecodeAStringWithUnicodeCharacters()
    {
        var str = "pexels-tansu-topuzoğlu-7688377.jpg";

        var enc = str.ToHex();
        Assert.AreNotEqual(enc, str);

        var unenc = enc.FromHex();
        Assert.AreEqual(unenc, str);
    }
}
