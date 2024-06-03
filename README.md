# EasyCryptoSalt

O EasyCryptoSalt é uma biblioteca simples e eficiente para operações criptográficas em .NET. Ele oferece funcionalidades para hashing seguro usando o algoritmo SHA-256, juntamente com a capacidade de comparar hashes utilizando uma chave e um salt.

Principais Recursos:
Hashing Seguro: Gere hashes seguros usando o algoritmo de hash SHA-256.
Comparação de Hashes: Verifique se o texto simples corresponde ao hash fornecido com facilidade.
Segurança Adicional com Salt: Utilize um salt adicional para aumentar a segurança dos hashes gerados.

Como Usar:
Instalação:
Instale o pacote NuGet EasyCryptoSalt em seu projeto:
Install-Package EasyCryptoSalt

Exemplo de Uso:

csharp
using EasyCryptoSalt;

// Criar uma instância de Crypto
var crypto = Crypto.Instance;

// Gerar um hash seguro
string hashedText = crypto.Encrypt("Texto a ser hashado");

// Verificar se um texto simples corresponde a um hash
bool isMatch = crypto.IsEquals("Texto a ser verificado", hashedText);

Observações:
Certifique-se de configurar corretamente a chave e o salt no arquivo de configuração appsettings.json para garantir a segurança adequada dos hashes gerados.
Esta biblioteca é projetada para ser fácil de usar e oferecer segurança robusta para suas necessidades criptográficas em .NET.
