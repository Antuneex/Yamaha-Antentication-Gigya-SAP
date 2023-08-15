# Introduction 
Api para integração com serviços externos de comunicação, como Microsoft Teams ou Zoom   

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
- Visal Studio 
    ctrl+b + f5

- CLI
    dotnet restore 
    dotnet build 
    dotnet run 

# Run on Docker

Adicionar um arquivo chamado Nuget.cofig com o seguinte conteúdo 
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageRestore>
    <add key="enabled" value="True" />
    <add key="automatic" value="True" />
  </packageRestore>
  <packageSources>
    <add key="DotNetCoreFXLab" value="https://dotnet.myget.org/F/dotnet-corefxlab/" />
    <add key="nuget.org" value="https://www.nuget.org/api/v2/" />
    <add key="AtenaPackages" value="https://pkgs.dev.azure.com/uoledtech/_packaging/AtenaPackages/nuget/v3/index.json" />
  </packageSources>
  <activePackageSource>
    <add key="AtenaPackages" value="https://pkgs.dev.azure.com/uoledtech/_packaging/AtenaPackages/nuget/v3/index.json" />
  </activePackageSource>
  <packageSourceCredentials>
    <AtenaPackages>
      <add key="Username" value="PAT"/>
      <add key="ClearTextPassword" value="<<AZURE KEY>>" />
    </AtenaPackages>
  </packageSourceCredentials>
  <bindingRedirects>
    <add key="skip" value="False" />
  </bindingRedirects>
  <packageManagement>
    <add key="format" value="0" />
    <add key="disabled" value="False" />
  </packageManagement>
  <disabledPackageSources>
    <add key="DotNetCoreFXLab" value="true" />
  </disabledPackageSources>
</configuration>  
```


Alterar o azure key para a sua key, caso ainda não tenha, seguit o [guideline](https://docs.microsoft.com/pt-br/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page)

Após isso, rode os seguintes comandos 

``` docker build . 
    docker run -p 4000:44326 -d <nome do container>
```

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)