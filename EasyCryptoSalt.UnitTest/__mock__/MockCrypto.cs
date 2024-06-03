using Bogus;

namespace __mock__;

/// <summary>
/// Classe mock para geração de textos de entrada aleatórios para testes.
/// </summary>
public sealed class MockCrypto
{
    private static MockCrypto? _instance;
    private static readonly object LockObject = new object();

    /// <summary>
    /// Instância singleton da classe MockCrypto.
    /// </summary>
    public static MockCrypto Instance
    {
        get
        {
            lock (LockObject)
            {
                return _instance ??= new MockCrypto();
            }
        }
    }

    /// <summary>
    /// Gera um novo texto simples aleatório.
    /// </summary>
    /// <returns>Texto simples aleatório.</returns>
    public string GetNewPlainText()
    {
        return new Faker().Internet.Password();
    }
}
