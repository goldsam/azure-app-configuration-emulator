using AzureAppConfigurationEmulator.Testcontainers.Certificates;
using Docker.DotNet.Models;
using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers;

/// <summary>
/// A fluent Docker container builder for azure-app-configuration-emulator.
/// </summary>
public class AzureAppConfigurationEmulatorBuilder
    : ContainerBuilder<AzureAppConfigurationEmulatorBuilder, AzureAppConfigurationEmulatorContainer, AzureAppConfigurationEmulatorConfiguration>
{
    /// <summary>
    /// Default azure-app-configuration-emulator image.
    /// </summary>
    public const string AzureAppConfigurationEmulatorImage = "tnc1997/azure-app-configuration-emulator:latest";

    /// <summary>
    /// Default port azure-app-configuration-emulator server port.
    /// </summary>
    public const ushort AzureAppConfigurationEmulatorServerPort = 4443;

    /// <inheritdoc />
    protected override AzureAppConfigurationEmulatorConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorBuilder" /> class.
    /// </summary>
    public AzureAppConfigurationEmulatorBuilder()
        : this(new AzureAppConfigurationEmulatorConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzureAppConfigurationEmulatorBuilder(AzureAppConfigurationEmulatorConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <summary>
    /// Convenience method to set endpoint URL of the emulated Azure App Configuration instance to `https://{resourceName}.azconfig.io`.
    /// </summary>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public AzureAppConfigurationEmulatorBuilder WithResourceName(string resourceName)
    {
        _ = Guard.Argument(resourceName, nameof(resourceName)).NotNull().NotEmpty();
        return WithAppConfigurationEndpoint(new UriBuilder
        {
            Scheme = Uri.UriSchemeHttps,
            Host = $"{resourceName}.azconfig.io",
            Port = AzureAppConfigurationEmulatorServerPort
        }.Uri);
    }

    /// <summary>
    /// Sets the endpoint URL of the emulated Azure App Configuration instance. For example, "https://contoso.azconfig.io".
    /// </summary>
    /// <param name="endpointUrl">Azure App Configuration endpoint URL</param>
    /// <returns>A configured instance of <see cref="AzureAppConfigurationEmulatorBuilder" />.</returns>
    public AzureAppConfigurationEmulatorBuilder WithAppConfigurationEndpoint(Uri endpointUrl)
    {
        _ = Guard.Argument(endpointUrl, nameof(endpointUrl)).NotNull();
        return WithNetworkAliases(endpointUrl.Host)
            .WithSelfSignedCertificate(certificate => certificate
                .WithDistinguishedName(new System.Security.Cryptography.X509Certificates.X500DistinguishedName($"CN={endpointUrl.Host}"))
                .AndSubjectAlternativeName(san => san.AddDnsName(endpointUrl.Host)));
    }

    /// <summary>
    /// Configures the use of a self-signed certificate.
    /// </summary>
    /// <param name="CertificateResourceMappingBuilderAction">Callback for customizing the self-signed certificate.</param>
    /// <returns>A configured instance of <see cref="AzureAppConfigurationEmulatorBuilder" />.</returns>
    public AzureAppConfigurationEmulatorBuilder WithSelfSignedCertificate(Func<SelfSignedCertificateBuilder, SelfSignedCertificateBuilder>? CertificateResourceMappingBuilderAction = null)
    {
        var certConfiguration = DockerResourceConfiguration.CertificateConfiguration ?? new SelfSignedCertificateConfiguration();
        if (CertificateResourceMappingBuilderAction != null)
            certConfiguration = CertificateResourceMappingBuilderAction(new SelfSignedCertificateBuilder(certConfiguration)).Configuration;

        return new AzureAppConfigurationEmulatorBuilder(
            new AzureAppConfigurationEmulatorConfiguration(DockerResourceConfiguration, 
            new AzureAppConfigurationEmulatorConfiguration(certConfiguration)));
    }

    /// <inheritdoc />
    public override AzureAppConfigurationEmulatorContainer Build()
    {
        Validate();

        var builder = this;

        var certificateConfiguration = DockerResourceConfiguration.CertificateConfiguration;
        if (certificateConfiguration != null)
        {
            var certificate = new SelfSignedCertificateBuilder().Build();
            builder = WithResourceMapping(certificate)
                .WithEnvironment("ASPNETCORE_URLS", "https://+:4443")
                .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", certificate.Target)
                .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", certificate.Password);
        }

        return new AzureAppConfigurationEmulatorContainer(builder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override AzureAppConfigurationEmulatorBuilder Init() => base.Init()
        .WithImage(AzureAppConfigurationEmulatorImage)
        .WithPortBinding(AzureAppConfigurationEmulatorServerPort, true);

    /// <inheritdoc />
    protected override AzureAppConfigurationEmulatorBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration) =>
        Merge(DockerResourceConfiguration, new AzureAppConfigurationEmulatorConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override AzureAppConfigurationEmulatorBuilder Clone(IContainerConfiguration resourceConfiguration) =>
        Merge(DockerResourceConfiguration, new AzureAppConfigurationEmulatorConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override AzureAppConfigurationEmulatorBuilder Merge(AzureAppConfigurationEmulatorConfiguration oldValue, AzureAppConfigurationEmulatorConfiguration newValue) =>
        new AzureAppConfigurationEmulatorBuilder(new AzureAppConfigurationEmulatorConfiguration(oldValue, newValue));
}
