namespace EasyCryptoSalt
{
    /// <summary>
    /// Opções de configuração para a classe Crypto.
    /// </summary>
    public class CryptoOptions
    {
        /// <summary>
        /// Chave de criptografia.
        /// </summary>
        public string Key { get; set; } // SECURE_AUTH_KEY
        public string AuthSalt { get; set; } // SECURE_AUTH_SALT   
    }
}