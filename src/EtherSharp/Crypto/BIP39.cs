using System.Security.Cryptography;
using System.Text;

namespace EtherSharp.Crypto;
public static class BIP39
{
    public static byte[] MnemonicToSeed(string mnemonic, string passphrase = "")
    {
        byte[] buffer = new byte[64];
        TryMnemonicToSeed(mnemonic, buffer, passphrase);
        return buffer;
    }

    public static bool TryMnemonicToSeed(string mnemonic, Span<byte> destination, string passphrase = "")
    {
        if (destination.Length != 64)
        {
            return false;
        }

        string normalizedMnemonic = mnemonic.Normalize(NormalizationForm.FormKD);
        string normalizedSaltedPassword = $"mnemonic{passphrase.Normalize(NormalizationForm.FormKD)}";

        int passwordSize = Encoding.UTF8.GetByteCount(normalizedMnemonic);
        int saltBufferSize = Encoding.UTF8.GetByteCount(normalizedSaltedPassword);

        Span<byte> passwordBuffer = passwordSize > 1024
            ? new byte[passwordSize]
            : stackalloc byte[passwordSize];
        Span<byte> saltBuffer = saltBufferSize > 1024
            ? new byte[saltBufferSize]
            : stackalloc byte[saltBufferSize];

        _ = Encoding.UTF8.GetBytes(normalizedMnemonic, passwordBuffer);
        _ = Encoding.UTF8.GetBytes(normalizedSaltedPassword, saltBuffer);

        Rfc2898DeriveBytes.Pbkdf2(passwordBuffer, saltBuffer, destination, 2048, HashAlgorithmName.SHA512);
        return true;
    }
}
