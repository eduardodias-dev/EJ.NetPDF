namespace EJ.NetPDF.API.Tests.Util;

public class GenericStringGenerator
{
    public static string Generate(int size)
    {
        var rand = new Random();
        var chars = Enumerable.Range(0, size)
            .Select(i => Convert.ToChar(rand.Next(33,126))).ToArray();

        return new string(chars);
    }
}