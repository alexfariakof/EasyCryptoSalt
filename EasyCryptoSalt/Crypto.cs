using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EasyCryptoSalt;

/// <summary>
/// Classe responsável por operações criptográficas, incluindo hashing com SHA-256 e comparação de hashes utilizando uma chave e um salt.
/// </summary>
public sealed class Crypto : ICrypto
{
    private readonly byte[] _key; // SECURE_AUTH_KEY
    private readonly byte[] _authSalt; // SECURE_AUTH_SALT
    private static readonly object LockObject = new object();
    private static ICrypto? _crypto;

    /// <summary>
    /// Instância singleton da classe Crypto.
    /// </summary>
    public static ICrypto Instance
    {
        get
        {
            lock (LockObject)
            {
                if (_crypto == null)
                {
                    _crypto = new Crypto();
                }

                return _crypto;
            }
        }
    }

    /// <summary>
    /// Construtor privado que inicializa a chave e o salt a partir do arquivo de configuração appsettings.json.
    /// </summary>
    private Crypto()
    {
        var key = GetHashKey();
        var keyByte = Encoding.UTF8.GetBytes(key);
        this._key = keyByte;
        var authSalt = GetAuthSalt();
        this._authSalt = Encoding.UTF8.GetBytes(authSalt);
    }

    /// <summary>
    /// Construtor público que inicializa a chave e o salt a partir das opções fornecidas no arquivo de configuração appsettings.json.
    /// </summary>
    /// <param name="options">Opções de configuração para Crypto.</param>
    public Crypto(IOptions<CryptoOptions> options)
    {
        var key = options.Value.Key ?? throw new ArgumentNullException("Key not defined.");
        var keyByte = Encoding.UTF8.GetBytes(key);
        this._key = keyByte;
        var authSalt = options.Value.AuthSalt ?? throw new ArgumentNullException("Auth Salt not defined.");
        this._authSalt = Encoding.UTF8.GetBytes(authSalt);
    }

    /// <summary>
    /// Cria a chave de hash a partir do arquivo de configuração appsettings.json.
    /// </summary>
    /// <returns>Chave de hash como string Base64.</returns>
    private static string GetHashKey()
    {
        try
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var jsonContent = File.ReadAllText(jsonFilePath);
            var config = JsonDocument.Parse(jsonContent);
            config.RootElement.TryGetProperty("CryptoConfigurations", out var cryptoConfigurations);
            cryptoConfigurations.TryGetProperty("Key", out var key);
            return key.GetString();
        }
        catch
        {
             throw new ArgumentException("Key not defined.");
        }
    }

    /// <summary>
    /// Cria o salt a partir do arquivo de configuração appsettings.json.
    /// </summary>
    /// <returns>Salt como string.</returns>
    private static string GetAuthSalt()
    {
        try
        {
            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var jsonContent = File.ReadAllText(jsonFilePath);
            var config = JsonDocument.Parse(jsonContent);
            config.RootElement.TryGetProperty("CryptoConfigurations", out var cryptoConfigurations);
            cryptoConfigurations.TryGetProperty("AuthSalt", out var authSalt);
            return authSalt.GetString();
        }
        catch
        {
             throw new ArgumentException("Auth Salt not defined.");
        }        
    }

    /// <summary>
    /// Gera um hash com salt para o input fornecido.
    /// </summary>
    /// <param name="input">Texto a ser hashado.</param>
    /// <returns>Hash com salt em formato Base64.</returns>
    public async Task<string> Encrypt(string input)
    {
        return await Task.Run(() =>
        {
            byte[] salt = GenerateSalt();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] inputWithSaltBytes = new byte[inputBytes.Length + salt.Length];
            Buffer.BlockCopy(salt, 0, inputWithSaltBytes, 0, salt.Length);
            Buffer.BlockCopy(inputBytes, 0, inputWithSaltBytes, salt.Length, inputBytes.Length);
            byte[] keyWithSaltBytes = new byte[_key.Length + _authSalt.Length];
            Buffer.BlockCopy(_key, 0, keyWithSaltBytes, 0, _key.Length);
            Buffer.BlockCopy(_authSalt, 0, keyWithSaltBytes, _key.Length, _authSalt.Length);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputWithSaltBytes);
                byte[] hashWithSaltBytes = new byte[salt.Length + hashBytes.Length];
                Buffer.BlockCopy(salt, 0, hashWithSaltBytes, 0, salt.Length);
                Buffer.BlockCopy(hashBytes, 0, hashWithSaltBytes, salt.Length, hashBytes.Length);

                return Convert.ToBase64String(hashWithSaltBytes);
            }
        });
    }

    /// <summary>
    /// Verifica se o texto simples fornecido corresponde ao hash fornecido.
    /// </summary>
    /// <param name="plainText">Texto simples a ser verificado.</param>
    /// <param name="hash">Hash para comparação.</param>
    /// <returns>True se o texto simples gerar o mesmo hash; caso contrário, false.</returns>
    public async Task<bool> Verify(string plainText, string hash)
    {
        return await Task.Run(() =>
        {
            byte[] hashWithSaltBytes = Convert.FromBase64String(hash);
            byte[] salt = new byte[_authSalt.Length];
            byte[] storedHashBytes = new byte[hashWithSaltBytes.Length - salt.Length];
            Buffer.BlockCopy(hashWithSaltBytes, 0, salt, 0, salt.Length);
            Buffer.BlockCopy(hashWithSaltBytes, salt.Length, storedHashBytes, 0, storedHashBytes.Length);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] inputWithSaltBytes = new byte[inputBytes.Length + salt.Length];
            Buffer.BlockCopy(salt, 0, inputWithSaltBytes, 0, salt.Length);
            Buffer.BlockCopy(inputBytes, 0, inputWithSaltBytes, salt.Length, inputBytes.Length);
            byte[] keyWithSaltBytes = new byte[_key.Length + _authSalt.Length];
            Buffer.BlockCopy(_key, 0, keyWithSaltBytes, 0, _key.Length);
            Buffer.BlockCopy(_authSalt, 0, keyWithSaltBytes, _key.Length, _authSalt.Length);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] computedHashBytes = sha256.ComputeHash(inputWithSaltBytes);
                return computedHashBytes.SequenceEqual(storedHashBytes);
            }
        });
    }

    /// <summary>
    /// Gera um salt aleatório baseado na chave Auth Salt definida em appsettings.json.
    /// </summary>
    /// <returns>Salt aleatório.</returns>
    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[_authSalt.Length];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }
}
