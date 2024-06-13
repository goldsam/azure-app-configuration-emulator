using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates
{
    /// <summary>
    /// Self-signed certificate configuration.
    /// </summary>
    public interface ISelfSignedCertificateConfiguration : ICertificateConfiguration
    {
        /// <summary>
        /// X.500 Distinguished Name of the self-signed certificate.
        /// </summary>
        public X500DistinguishedName? DistinguishedName { get; }

        /// <summary>
        /// Key size of the self-signed certificate.
        /// </summary>
        public int? KeySize { get; }

        /// <summary>
        /// Hash algorithm used when creating the self-signed certificate.
        /// </summary>
        public HashAlgorithmName? HashAlgorithm { get; }

        /// <summary>
        /// Gets the actions to customize the Subject Alternative Name.
        /// </summary>
        public IEnumerable<Action<SubjectAlternativeNameBuilder>> SubjectAlternativeNameActions { get; }
    }
}
