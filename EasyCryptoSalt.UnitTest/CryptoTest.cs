using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using __mock__;

namespace EasyCryptoSalt.UnitTest;
public sealed class CryptoTest
{
    private readonly IOptions<CryptoOptions?> _cryptoOptions;

    public CryptoTest()
    {
        var configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                       .Build();

        if (configuration is null) throw new ArgumentNullException("Arquivo appsettings.json não encontrado.");
        _cryptoOptions = Options.Create(configuration.GetSection("CryptoConfigurations").Get<CryptoOptions>()) ?? throw new ArgumentNullException("Configurações appsettings.json não encontradas.");
    }

    [Fact]
    public async Task Encrypt_And_Decrypt_Should_Work_With_Instance()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = Crypto.Instance;

        // Act
        string encryptedText = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedText);

        // Assert
        Assert.NotEqual(originalText, encryptedText);
        Assert.True(isEquals);
    }

    [Fact]
    public async Task Encrypt_And_Decrypt_Should_Work_With_Options_Instance()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = new Crypto(_cryptoOptions);

        // Act
        string encryptedText = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedText);

        // Assert
        Assert.NotEqual(originalText, encryptedText);
        Assert.True(isEquals);
    }

    [Fact]
    public async Task Encrypt_Should_Produce_Different_Output_For_Same_Input()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = Crypto.Instance;

        // Act
        string encryptedText1 = await crypto.Encrypt(originalText);
        string encryptedText2 = await crypto.Encrypt(originalText);

        // Assert
        Assert.NotEqual(encryptedText1, encryptedText2);
    }

    [Fact]
    public async Task Encrypt_Should_Produce_Different_Output_For_Same_Input_With_Options_Instance()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = new Crypto(_cryptoOptions);

        // Act
        string encryptedText1 = await crypto.Encrypt(originalText);
        string encryptedText2 = await crypto.Encrypt(originalText);

        // Assert
        Assert.NotEqual(encryptedText1, encryptedText2);
    }

    [Fact]
    public async Task Encrypt_Should_Produce_Valid_Hash_With_Salt()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = Crypto.Instance;

        // Act
        string encryptedText = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedText);

        // Assert
        Assert.True(isEquals);
    }

    [Fact]
    public async Task Encrypt_Should_Produce_Valid_Hash_With_Salt_With_Options_Instance()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = new Crypto(_cryptoOptions);

        // Act
        string encryptedText = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedText);

        // Assert
        Assert.True(isEquals);
    }

    [Fact]
    public async Task Encrypt_Should_Produce_Valid_Hash_With_Salt_With_Instance_and_Options_Instance()
    {
        // Arrange
        string originalText = MockCrypto.Instance.GetNewPlainText();
        ICrypto crypto = Crypto.Instance;
        ICrypto cryptoOptions = new Crypto(_cryptoOptions);

        // Act
        string encryptedTextInstance = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedTextInstance);
        var isEqualsOptions = await cryptoOptions.Verify(originalText, encryptedTextInstance);

        // Assert
        Assert.True(isEquals);
        Assert.True(isEqualsOptions);
    }

    [Fact]
    public async Task Encrypt_And_IsEquals_Should_Handle_Empty_Input()
    {
        // Arrange
        string originalText = "";
        ICrypto crypto = Crypto.Instance;

        // Act
        string encryptedText = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedText);

        // Assert
        Assert.NotEqual(originalText, encryptedText);
        Assert.True(isEquals);
    }

    [Fact]
    public async Task Encrypt_And_IsEquals_Should_Handle_Empty_Input_With_Options_Instance()
    {
        // Arrange
        string originalText = "";
        ICrypto crypto = new Crypto(_cryptoOptions);

        // Act
        string encryptedText = await crypto.Encrypt(originalText);
        var isEquals = await crypto.Verify(originalText, encryptedText);

        // Assert
        Assert.NotEqual(originalText, encryptedText);
        Assert.True(isEquals);
    }

    [Theory]
    [InlineData("Ajmnolj1(&1jxçdsu9IQJAÇp)_62LA", "}46aSb$]R|jjTtKGY`", true)]  // Chave válida
    [InlineData(null, "}46aSb$]R|jjTtKGY`", false)]  // Chave inválida
    [InlineData("Ajmnolj1(&1jxçdsu9IQJAÇp)_62LA", null, false)]  // Chave inválida
    public void ValidateKey_Should_Throw_Exception_For_Invalid_Keys(string key, string authSalt, bool isValid)
    {
        // Arrange
        var options = Options.Create(new CryptoOptions() { Key = key, AuthSalt = authSalt });

        // Act & Assert
        if (isValid)
        {
            Assert.IsType<Crypto>(new Crypto(options));
        }
        else
        {
            Assert.Throws<ArgumentNullException>(() => new Crypto(options));
        }
    }
}
