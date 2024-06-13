using System.Security.Cryptography.X509Certificates;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates
{
    /// <summary>
    /// Certificate configuration.
    /// </summary>
    public interface ICertificateConfiguration : IResourceConfiguration<CertificateRequest>
    {
        /// <summary>
        /// Path to mount the self-signed certificate to inside the container.
        /// </summary>
        string? ContainerPath { get; }

        /// <summary>
        /// Unix file modes of the mounted self-signed certificate file.
        /// </summary>
        UnixFileModes? UnixFileModes { get; }

        /// <summary>
        /// Password of the self-signed certificate.
        /// </summary>
        string? Password { get; }

        /// <summary>
        /// Content type of the mounted self-signed certificate file.
        /// </summary>
        X509ContentType? ContentType { get; }

        /// <summary>
        /// Creates a certificate configuration with the specified <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        ICertificateConfiguration AsContentType(X509ContentType contentType);
    }
}
