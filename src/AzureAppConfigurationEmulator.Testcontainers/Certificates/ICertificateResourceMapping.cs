using System.Security.Cryptography.X509Certificates;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates;

/// <summary>
/// Certificate resource configuration.
/// </summary>
public interface ICertificateResourceMapping : IResourceMapping
{
    /// <summary>
    /// Gets the certificate configuration.
    /// </summary>
    ICertificateConfiguration Configuration { get; }

    /// <summary>
    /// Gets the content type of the certificate file.
    /// </summary>
    X509ContentType ContentType => Configuration.ContentType!.Value;

    /// <summary>
    /// Gets the certificate to be mounted.
    /// </summary>
    X509Certificate2 Certificate { get; }

    /// <summary>
    /// Password of the self-signed certificate.
    /// </summary>
    string Password { get; }

    /// <summary>
    /// Creates a representation of this certificate resource 
    /// mapping with the specified <paramref name="contentType"/>.
    /// </summary>
    /// <param name="contentType">Content type of the certificate resource mapping.</param>
    /// <returns>Certificate resource mapping with the specified content type.</returns>
    ICertificateResourceMapping AsContentType(X509ContentType contentType);
}
