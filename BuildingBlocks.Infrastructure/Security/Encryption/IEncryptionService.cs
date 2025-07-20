namespace BuildingBlocks.Infrastructure.Security.Encryption;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Encrypt(string plainText, string key);
    string Decrypt(string cipherText);
    string Decrypt(string cipherText, string key);
    byte[] Encrypt(byte[] data);
    byte[] Encrypt(byte[] data, byte[] key);
    byte[] Decrypt(byte[] encryptedData);
    byte[] Decrypt(byte[] encryptedData, byte[] key);
    string GenerateKey();
    bool ValidateKey(string key);
}