# UBI - Segurança de Sistemas Informáticos: TUDOIGUAL

Este repositório contém um projeto relacionado à segurança da informação. Esta aplicação CLI fornece funcionalidades para comunicação segura, encriptação e decriptação de ficheiros, assinatura digital e monitorização de diretórios para alterações. Utiliza click para a CLI, watchdog para monitorização de diretórios e cryptography para encriptação e assinaturas digitais.

## Pré-requisitos

- .NET Core 8 LTS
- Certificados X.509 em formato PFX
- Acesso à rede para comunicação cliente-servidor

## Instalação

1. Clone este repositório:

   ```sh
   git clone https://github.com/juscelior/ubi-security-tudoigual.git
   ```

2. Navegue até ao diretório do projeto:

    ```sh
    cd ubi-security-tudoigual
    ```

## Como Usar

1. Criar um utilizador e um certificado digital para esse utilizador no servidor:

   ```sh
   .\TUDOIGUAL.Server.exe -new-user alice -ip 127.0.0.1
   ```
    Nesta fase, é necessário criar um certificado no servidor com o nome de utilizador do cliente. O servidor regista o thumbprint e o nome desse cliente para conseguir validar o certificado posteriormente.

2. Executar o servidor:

   ```sh
   .\TUDOIGUAL.Server.exe -dir C:\git\Server\Upload -ip 127.0.0.1 -port 6667
   ```

2. Executar o cliente:

   ```sh
   .\TUDOIGUAL.Client.exe -user alice -ip localhost -port 6667  -thumbprint 4E4DC099E2E7DDCB5C9AEE0F4587B9C9A62C95F5  -dir C:\ubi2\TUDOIGUAL\Client\Files
   ```

2. Autenticar no servidor e decriptar o ficheiro local:

   ```sh
   .\TUDOIGUAL.Client.exe -user juscelio -filename "C:\ubi2\Client\Files\Demo - Copia.txt.enc" -out C:\ubi2\
   ```

## Comunicação Segura

Incluímos uma comunicação criptografada entre o cliente e o servidor usando TLS 1.2. Para isso, é necessário:

- Criar um certificado no servidor com o nome de utilizador do cliente.
- O servidor regista o thumbprint e o nome do cliente para conseguir validar o certificado posteriormente.
- O cliente precisa do argumento -thumbprint do servidor, pois o mesmo faz a validação do certificado do servidor.

A fase de criação de um utilizador é feita antes de instalar o cliente. Após este momento, a verificação e a criptografia da comunicação entre as partes são sempre realizadas.


## Funcionalidades

- Geração e gestão de chaves RSA.
- Cifragem e decifragem de ficheiros utilizando AES.
- Assinatura digital de ficheiros.
- Monitorização de diretórios de sincronização para novas adições de ficheiros.
- Comunicação segura utilizando certificados X.509.
- Atualização automática de um ficheiro de clientes com certificados importados.

## Funcionalidades Básicas

Em termos de funcionalidades básicas:

- [x] Na primeira utilização, a aplicação cliente gera um par de chaves para criptografia de chave pública (e.g., RSA). A chave privada desse par deve ser guardada apenas no cliente;
- [x] Na primeira utilização, a aplicação cliente permite o registo de um novo utilizador junto do servidor;
- [x] O registo de um novo utilizador é feito de uma forma segura (e.g., as comunicações são cifradas, é trocada uma chave efémera Diffie-Hellman ou usadas chaves RSA geradas aquando da instalação do servidor);
- [x] Quando um ficheiro é colocado na diretoria de sincronização, é gerada uma chave de cifra e o ficheiro é cifrado com uma cifra de chave simétrica por blocos do estado da arte num modo adequado;
- [x] A chave de cifra simétrica gerada no ponto anterior é cifrada com a chave RSA mencionada no primeiro ponto;
- [x] Ao retirar um ficheiro da diretoria, deve ser feita autenticação do utilizador no servidor e decifrada a chave de cifra que permite decifrar o ficheiro;
- [x] Ao colocar um ficheiro na diretoria, é calculada a sua assinatura digital com recurso ao servidor.

## Funcionalidades Avançadas

Em termos de funcionalidades avançadas:

- [x] O sistema suporta a sincronização, via servidor, entre duas aplicações em dois computadores distintos;
- [ ] Ao transmitir um ficheiro entre dois computadores, este é partido em vários bocados, transmitidos individualmente com códigos MAC, que são verificados à chegada;
- [ ] O sistema é implementado com criptografia sobre curvas elípticas em vez de RSA;
- [x] Outras funcionalidades relevantes no contexto da segurança do sistema e que o favoreçam na nota.