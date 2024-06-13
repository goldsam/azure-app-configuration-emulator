using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates;

/// <summary>
/// A fluent builder for a self-signed certificate resource.
/// </summary>
public interface ISelfSignedCertificateBuilder<out TBuilderEntity>
{
    /// <summary>
    /// Sets the distinguished name of the self-signed certificate.
    /// </summary>
    /// <param name="distinguishedName">Certificate distinguished name</param>
    /// <returns></returns>
    TBuilderEntity WithDistinguishedName(X500DistinguishedName distinguishedName);

    /// <summary>
    /// Sets the key size of the self-signed certificate.
    /// </summary>
    /// <param name="keySize">Key size</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    TBuilderEntity WithKeySize(int keySize);

    /// <summary>
    /// Sets the Unix file modes of the mounted self-signed certificate file.
    /// </summary>
    /// <param name="unixFileModes">Unix file modes</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    TBuilderEntity WithUnixFileModes(UnixFileModes unixFileModes);

    /// <summary>
    /// Sets the hash algorithm used to sign the self-signed certificate.
    /// </summary>
    /// <param name="hashAlgorithm">Certificate signing hash algorithm.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    TBuilderEntity WithHashAlgorithm(HashAlgorithmName hashAlgorithm);

    /// <summary>
    /// Sets the password of the self-signed certificate.
    /// </summary>
    /// <param name="password">Certificate password.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    TBuilderEntity WithPassword(string password);

    /// <summary>
    /// Sets the content type of the mounted self-signed certificate file.
    /// </summary>
    /// <param name="subjectAlternativeNameAction">Certificate content type.</param>
    /// <returns>A configured instance of <typeparamref name="TBuilderEntity" />.</returns>
    TBuilderEntity AndSubjectAlternativeName(Action<SubjectAlternativeNameBuilder> subjectAlternativeNameAction);
}
