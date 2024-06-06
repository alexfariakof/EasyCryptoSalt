[Readme em português](https://github.com/alexfariakof/EasyCryptoSalt/blob/main/README.md)

# EasyCryptoSalt

EasyCryptoSalt is a simple and efficient library for cryptographic operations in .NET. It provides functionalities for secure hashing using the SHA-256 hashing algorithm, along with the ability to compare hashes using a key and a salt.

## Key Features

* Secure Hashing: Generate secure hashes using the SHA-256 hash algorithm.
* Hash Comparison: Easily verify if plain text matches the provided hash.
* Additional Security with Salt: Use an additional salt to enhance the security of generated hashes.
* Encryption and verification methods are asynchronous, providing scalability and responsiveness to the system, especially in applications performing I/O operations or other potentially blocking tasks.

## Installation

To install the NuGet package `EasyCryptoSalt`, execute the following command in the NuGet Package Manager Console:

```powershell
Install-Package EasyCryptoSalt
```

## Configuration

Ensure that the appsettings.json file contains the CryptoConfigurations section with the necessary keys:

```json
{
   "CryptoConfigurations": {
        "Key": "Exemplo de Chave `^AOUWNW16h*634+=tq51#2fa8091$2jnsais71298>shsady|==",
        "AuthSalt": "Exemplo de Auth salt ``àadskldjlskjdlk\gwt257__1816!?}[oap725-1%"
   }
}
```

## Example Usage Mode 2

```csharp  
using EasyCryptoSalt;

// Create an instance of Crypto
var crypto = Crypto.Instance;

// Generate a secure hash
string hashedText = crypto.Encrypt("Texto a ser hashado").Result;

// Verify if a plain text matches a hash
bool isMatch = crypto.Verify("Texto a ser verificado", hashedText).Result;
```

## Exemplo de Uso Modo 2

```csharp
#program.cs
using EasyCryptoSalt;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<CryptoOptions>(configuration.GetSection("CryptoConfigurations"));
builder.Services.AddSingleton<ICrypto, Crypto>();

var app = builder.Build();      
app.Run();

#ExampleClass.cs 
using EasyCryptoSalt;

public class ExampleClass
{
  private readonly ICrypto _crypto;
  public ExampleClass(ICrypto crypto)
  {
    _crypto = crypto;
  }

  public void UseCrypto()
  {
    // Generate a secure hash
    string hashedText = crypto.Encrypt("Texto a ser hashado").Result;

    // Verify if a plain text matches a hash
    bool isMatch = crypto.Verify("Texto a ser verificado", hashedText).Result;
  }
}        
```

## Classe CryptoOptions

```csharp
public class CryptoOptions
{
    public string Key { get; set; }
    public string AuthSalt { get; set; }
}
```

## Crypto Class

### Description

The Crypto class is responsible for performing cryptographic operations, including generating hashes with SHA-256 and comparing hashes using a key and a salt.


### Properties

* Instance: A static property that returns a singleton instance of the Crypto class.


### Constructors

* Crypto(): Private constructor that initializes the key and salt from the appsettings.json configuration file.
* Crypto(IOptions<CryptoOptions> options): Public constructor that initializes the key and salt from the provided options in the appsettings.json configuration file.

### Methods

```csharp
public async Task<string> Encrypt(string input)
```
> * Description: Generates a hash with salt for the provided input.
> * Parameters:
> input (string): Text to be hashed.
> * Returns:
> string: Hash with salt in Base64 format.

```csharp
public async Task<bool> Verify(string plainText, string hash)
```
> * Description: Verifies if the provided plain text matches the provided hash.
> * Parameters:
     plainText (string): Plain text to be verified.
     hash (string): Hash for comparison.
> * Returns:
    bool: Returns true if the plain text generates the same hash; otherwise, false.

```csharp
private byte[] GenerateSalt()
```

> * Description: Generates a random salt based on the Auth Salt key defined in appsettings.json.
> * Returns:
    byte[]: Random salt.

## Notes:

* Ensure to properly configure the key and salt in the appsettings.json file to ensure the proper security of generated hashes.

    ```json
    {
      "CryptoConfigurations": {
        "Key": "exemplo de Chave ",
        "AuthSalt": "exemplo de Auth salt"
      }
    }
    ```

* This library is designed to be easy to use and offer robust security for your cryptographic needs in .NET.
