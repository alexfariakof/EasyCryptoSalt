# EasyCryptoSalt

O EasyCryptoSalt é uma biblioteca simples e eficiente para operações criptográficas em .NET. Ele oferece funcionalidades para hashing seguro usando o algoritmo SHA-256, juntamente com a capacidade de comparar hashes utilizando uma chave e um salt.

## Principais Recursos:
  * Hashing Seguro: Gere hashes seguros usando o algoritmo de hash SHA-256.
  * Comparação de Hashes: Verifique se o texto simples corresponde ao hash fornecido com facilidade.
  * Segurança Adicional com Salt: Utilize um salt adicional para aumentar a segurança dos hashes gerados.

## Como Usar:
  1. Instalação:

     Instale o pacote NuGet EasyCryptoSalt em seu projeto:
      ```powershell
      Install-Package EasyCryptoSalt
      ```

  2. Exemplo de Uso Modo 1:

      ```csharp  
        using EasyCryptoSalt;
        
        // Criar uma instância de Crypto
        var crypto = Crypto.Instance;
        
        // Gerar um hash seguro
        string hashedText = crypto.Encrypt("Texto a ser hashado");
        
        // Verificar se um texto simples corresponde a um hash
        bool isMatch = crypto.Verify("Texto a ser verificado", hashedText);
      ```

  3. Exemplo de Uso Modo 2:

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


## Observações:
  * Certifique-se de configurar corretamente a chave e o salt no arquivo de configuração appsettings.json para garantir a segurança adequada dos hashes gerados.
    ```json
     "CryptoConfigurations": {
       "Key": "exemplo de Chave ",
       "AuthSalt": "exemplo de Auth salt"
     }
    ```
  * Esta biblioteca é projetada para ser fácil de usar e oferecer segurança robusta para suas necessidades criptográficas em .NET.
