using AzureAppConfigurationEmulator.Testcontainers.Certificates;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace AzureAppConfigurationEmulator.Testcontainers;

public class AzureAppConfigurationEmulatorContainer : DockerContainer
{
    public ICertificateResourceMapping? Certificate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulator" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">Container logger</param>
    public AzureAppConfigurationEmulatorContainer(AzureAppConfigurationEmulatorConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        if (configuration.CertificateConfiguration != null)
            Certificate = new SelfSignedCertificateBuilder(configuration.CertificateConfiguration).Build();
    }

    /// <summary>
    /// Gets the AzureAppConfigurationEmulator service endpoint.
    /// </summary>
    /// <returns>The AzureAppConfigurationEmulator service endpoint.</returns>
    public Uri GetEndpoint() =>
        new UriBuilder(
            Uri.UriSchemeHttps, 
            Hostname, 
            GetMappedPublicPort(AzureAppConfigurationEmulatorBuilder.AzureAppConfigurationEmulatorServerPort)).Uri;
}
