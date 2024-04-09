using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Tests.Extensions;

public class StringExtensionsTests
{
    [Test]
    public void CanEncodeThenDecodeAString()
    {
        var str = "pexels-andre-furtado-1264210.jpg";

        var enc = str.ToHex();
        Assert.That(enc != str);
        Assert.That(enc.Contains('"'), Is.False);
        Assert.That(enc.Contains('\''), Is.False);

        var unenc = enc.FromHex();
        Assert.That(str, Is.EqualTo(unenc));
    }

    [Test]
    public void CanEncodeThenDecodeAStringWithUnicodeCharacters()
    {
        var str = "pexels-tansu-topuzoğlu-7688377.jpg";

        var enc = str.ToHex();
        Assert.That(enc, Is.Not.EqualTo(str));

        var unenc = enc.FromHex();
        Assert.That(unenc, Is.EqualTo(str));
    }
}
