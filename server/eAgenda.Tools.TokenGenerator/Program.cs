using System.Security.Cryptography;

namespace eAgenda.Tools.TokenGenerator;

internal class Program
{
    static void Main(string[] args)
    {
        var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        Console.Write("Cifra de 32 bytes: " + key);

        Console.ReadLine();
    }
}
