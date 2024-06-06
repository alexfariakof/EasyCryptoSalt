namespace EasyCryptoSalt;

/// <summary>
/// Interface para a classe Crypto.
/// </summary>
public interface ICrypto
{
    /// <summary>
    /// Gera um hash com salt para o input fornecido.
    /// </summary>
    /// <param name="input">Texto a ser hashado.</param>
    /// <returns>Hash com salt em formato Base64.</returns>
    Task<string> Encrypt(string input);

    /// <summary>
    /// Verifica se o texto simples fornecido corresponde ao hash fornecido.
    /// </summary>
    /// <param name="plainText">Texto simples a ser verificado.</param>
    /// <param name="hash">Hash para comparação.</param>
    /// <returns>True se o texto simples gerar o mesmo hash; caso contrário, false.</returns>
    Task<bool> Verify(string plainText, string hash);
    
}