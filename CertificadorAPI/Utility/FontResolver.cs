using PdfSharp.Fonts;

namespace FontChanging;
public class PoppinsFontResolver : IFontResolver
{
    private static byte[] _poppinsRegular;

    static PoppinsFontResolver()
    {
        _poppinsRegular = File.ReadAllBytes("C:\\Users\\pedro\\Downloads\\pdftest\\poppins.ttf");
    }
    public byte[]? GetFont(string faceName)
    {
        return faceName == "Poppins#" ? _poppinsRegular : null;
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        if (familyName == "Poppins" && !bold && !italic)
        {
            return new FontResolverInfo("Poppins#");
        }
        return null;
    }
}