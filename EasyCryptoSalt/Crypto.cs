﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

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
        var key = options.Value.Key ?? throw new ArgumentNullException("Key Auth not defined.");
        var keyByte = Encoding.UTF8.GetBytes(key);
        this._key = keyByte;
        var authSalt = options.Value.AuthSalt ?? throw new ArgumentNullException("Key Auth Salt not defined.");
        this._authSalt = Encoding.UTF8.GetBytes(authSalt);
    }

    /// <summary>
    /// Cria a chave de hash a partir do arquivo de configuração appsettings.json.
    /// </summary>
    /// <returns>Chave de hash como string Base64.</returns>
    private static string GetHashKey()
    {
        var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (File.Exists(jsonFilePath))
        {
            var jsonContent = File.ReadAllText(jsonFilePath);
            var config = JObject.Parse(jsonContent);
            var cryptoKey = config["CryptoConfigurations"]?["Key"]?.ToString() ?? throw new ArgumentNullException("Key Auth not defined.");
            return cryptoKey;
        }
        throw new ArgumentException("File appsettings.json not found.");
    }

    /// <summary>
    /// Cria o salt a partir do arquivo de configuração appsettings.json.
    /// </summary>
    /// <returns>Salt como string.</returns>
    private static string GetAuthSalt()
    {
        var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (File.Exists(jsonFilePath))
        {
            var jsonContent = File.ReadAllText(jsonFilePath);
            var config = JObject.Parse(jsonContent);
            var authSalt = config["CryptoConfigurations"]?["AuthSalt"]?.ToString() ?? throw new ArgumentNullException("Key Auth Salt not defined.");
            return authSalt;
        }
        throw new ArgumentException("File appsettings.json not found.");
    }

    /// <summary>
    /// Gera um hash com salt para o input fornecido.
    /// </summary>
    /// <param name="input">Texto a ser hashado.</param>
    /// <returns>Hash com salt em formato Base64.</returns>
    public string Encrypt(string input)
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
    }

    /// <summary>
    /// Verifica se o texto simples fornecido corresponde ao hash fornecido.
    /// </summary
    /// <param name="plainText">Texto simples a ser verificado.</param>
    /// <param name="hash">Hash para comparação.</param>
    /// <returns>True se o texto simples gerar o mesmo hash; caso contrário, false.</returns>
    public bool Verify(string plainText, string hash)
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