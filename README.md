[Read this page in English](https://github.com/alexfariakof/EasyCryptoSalt/blob/main/README-en.md)

# EasyCryptoSalt

O EasyCryptoSalt é uma biblioteca simples e eficiente para operações criptográficas em .NET. Ele oferece funcionalidades para hashing seguro usando o algoritmo SHA-256, juntamente com a capacidade de comparar hashes utilizando uma chave e um salt.

## Principais Recursos

* Esta versão utiliza .NET Standard 2.0, tornando-o reutilizável em várias plataformas .NET.
* Hashing Seguro: Gere hashes seguros usando o algoritmo de hash SHA-256.
* Comparação de Hashes: Verifique se o texto simples corresponde ao hash fornecido com facilidade.
* Segurança Adicional com Salt: Utilize um salt adicional para aumentar a segurança dos hashes gerados.

## Instalação

Para instalar o pacote NuGet `EasyCryptoSalt`, execute o seguinte comando no console do Gerenciador de Pacotes NuGet:

```powershell
  dotnet add package EasyCryptoSalt --version 1.0.2
```

## Configuração

   Certifique-se de que o arquivo appsettings.json contém a seção CryptoConfigurations com as chaves necessárias:

```json
{
   "CryptoConfigurations": {
        "Key": "Exemplo de Chave `^AOUWNW16h*634+=tq51#2fa8091$2jnsais71298>shsady|==",
        "AuthSalt": "Exemplo de Auth salt ``àadskldjlskjdlk\gwt257__1816!?}[oap725-1%"
   }
}
```

## Exemplo de Uso Modo 1

```csharp  
using EasyCryptoSalt;

// Criar uma instância de Crypto
var crypto = Crypto.Instance;

// Gerar um hash seguro
string hashedText = crypto.Encrypt("Texto a ser hashado");

// Verificar se um texto simples corresponde a um hash
bool isMatch = crypto.Verify("Texto a ser verificado", hashedText);
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
    // Gerar um hash seguro
    string hashedText = crypto.Encrypt("Texto a ser hashado");

    // Verificar se um texto simples corresponde a um hash
    bool isMatch = crypto.Verify("Texto a ser verificado", hashedText);
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

## Classe Crypto

### Descrição

A classe Crypto é responsável por realizar operações criptográficas, incluindo a geração de hashes com SHA-256 e a comparação de hashes utilizando uma chave e um salt.

### Propriedades

* Instance: Propriedade estática que retorna uma instância singleton da classe Crypto.

### Construtores

* Crypto(): Construtor privado que inicializa a chave e o salt a partir do arquivo de configuração appsettings.json.
* Crypto(IOptions<CryptoOptions> options): Construtor público que inicializa a chave e o salt a partir das opções fornecidas no arquivo de configuração appsettings.json.

### Métodos

```csharp
public async Task<string> Encrypt(string input)
```

> * Descrição: Gera um hash com salt para o input fornecido.
> * Parâmetros:
> input (string): Texto a ser hashado.
> * Retorno:
> string: Hash com salt em formato Base64.

```csharp
public async Task<bool> Verify(string plainText, string hash)
```

> * Descrição: Verifica se o texto simples fornecido corresponde ao hash fornecido.
> * Parâmetros:
      plainText (string): Texto simples a ser verificado.
      hash (string): Hash para comparação.
> * Retorno:
      bool: Retorna true se o texto simples gerar o mesmo hash; caso contrário, false.

```csharp
private byte[] GenerateSalt()
```

> * Descrição: Gera um salt aleatório baseado na chave Auth Salt definida em appsettings.json.
> * Retorno:
       byte[]: Salt aleatório.

## Observações:

* Certifique-se de configurar corretamente a chave e o salt no arquivo de configuração appsettings.json para garantir a segurança adequada dos hashes gerados.

    ```json
    {
      "CryptoConfigurations": {
        "Key": "exemplo de Chave ",
        "AuthSalt": "exemplo de Auth salt"
      }
    }
    ```

* Esta biblioteca é projetada para ser fácil de usar e oferecer segurança robusta para suas necessidades criptográficas em .NET.
