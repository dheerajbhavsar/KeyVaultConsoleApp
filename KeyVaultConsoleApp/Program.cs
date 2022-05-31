using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace KeyVaultConsoleApp;

public class Program
{
    private static readonly IConfiguration config;
    static Program()
    {
        var configBuilder = new ConfigurationBuilder();
        config = configBuilder
            .AddJsonFile("application.json")
            .Build();
    }

    static async Task Main(string[] args)
    {

        var clientId = GetConfig("client_id");
        var clientSecret = GetConfig("client_secret");
        var vaultName = GetConfig("keyvault");

        string VAULT_URL = $"https://{vaultName}.vault.azure.net";

        Console.WriteLine("getting secret from key vault...");

        var kvClient = KvClient(clientId, clientSecret);
        var secret = await kvClient.GetSecretAsync(VAULT_URL, "secret");

        Console.WriteLine($"Secret: {secret.Value}");
    }

    private static KeyVaultClient KvClient(string clientId, string clientSecret)
    {
        return new KeyVaultClient(async (authority, resource, code) =>
        {
            var context = new AuthenticationContext(authority);
            var credential = new ClientCredential(clientId, clientSecret);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, credential);
            return result.AccessToken;
        });
    }

    private static string GetConfig(string key) => config[key];
}
