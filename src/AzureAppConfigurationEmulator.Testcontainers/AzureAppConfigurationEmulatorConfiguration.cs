using AzureAppConfigurationEmulator.Testcontainers.Certificates;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers;

/// <inheritdoc cref="ContainerConfiguration" />
public sealed class AzureAppConfigurationEmulatorConfiguration : ContainerConfiguration
{
    public ISelfSignedCertificateConfiguration? CertificateConfiguration { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorConfiguration" /> class.
    /// </summary>
    public AzureAppConfigurationEmulatorConfiguration()
        : this(new SelfSignedCertificateConfiguration())
    {
    }

    public AzureAppConfigurationEmulatorConfiguration(ISelfSignedCertificateConfiguration? certificateConfiguration)
    {
        CertificateConfiguration = certificateConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="containerConfiguration">The Docker Container configuration.</param>
    public AzureAppConfigurationEmulatorConfiguration(IContainerConfiguration containerConfiguration)
        : base(containerConfiguration)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzureAppConfigurationEmulatorConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {}

    // <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzureAppConfigurationEmulatorConfiguration(AzureAppConfigurationEmulatorConfiguration resourceConfiguration)
        : this(new AzureAppConfigurationEmulatorConfiguration(), resourceConfiguration)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureAppConfigurationEmulatorConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public AzureAppConfigurationEmulatorConfiguration(AzureAppConfigurationEmulatorConfiguration oldValue, AzureAppConfigurationEmulatorConfiguration newValue)
        : base(oldValue, newValue)
    {
        CertificateConfiguration = newValue.CertificateConfiguration ?? oldValue.CertificateConfiguration;
    }
}
