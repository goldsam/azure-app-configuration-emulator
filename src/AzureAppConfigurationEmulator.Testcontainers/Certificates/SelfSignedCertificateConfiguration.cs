using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates;

/// <summary>
/// Configuration used when generating a self-signed certificate.
/// </summary>
public class SelfSignedCertificateConfiguration : ResourceConfiguration<CertificateRequest>, ISelfSignedCertificateConfiguration
{
    private static readonly Action<SubjectAlternativeNameBuilder>[] EmptySubjectAlternativeNameActions = new Action<SubjectAlternativeNameBuilder>[0];

    private readonly Action<SubjectAlternativeNameBuilder>[]? _subjectAlternativeNameActions;

    /// <inheritdoc />
    public string? ContainerPath { get; }

    /// <inheritdoc />
    public X500DistinguishedName? DistinguishedName { get; }

    /// <inheritdoc />
    public int? KeySize { get; }

    /// <inheritdoc />
    public UnixFileModes? UnixFileModes { get; }

    /// <inheritdoc />
    public HashAlgorithmName? HashAlgorithm { get; }

    /// <inheritdoc />
    public string? Password { get; }

    /// <inheritdoc />
    public X509ContentType? ContentType { get; }

    /// <inheritdoc />
    public IEnumerable<Action<SubjectAlternativeNameBuilder>> SubjectAlternativeNameActions =>
        _subjectAlternativeNameActions ?? EmptySubjectAlternativeNameActions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfSignedCertificateConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The self-signed certificate resource configuration.</param>
    public SelfSignedCertificateConfiguration(ISelfSignedCertificateConfiguration resourceConfiguration)
        : this(new SelfSignedCertificateConfiguration(), resourceConfiguration)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfSignedCertificateConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The self-signed certificate resource configuration.</param>
    public SelfSignedCertificateConfiguration(IResourceConfiguration<CertificateRequest> resourceConfiguration)
        : base(resourceConfiguration)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfSignedCertificateConfiguration" /> class.
    /// </summary>
    /// <param name="containerPath">Path to mount the self-signed certificate to inside the container.</param>
    /// <param name="distinguishedName">X.500 Distinguished Name of the self-signed certificate.</param>
    /// <param name="keySize">Key size of the self-signed certificate.</param>
    /// <param name="unixFileModes">Unix file modes of the mounted self-signed certificate file.</param>
    /// <param name="hashAlgorithm">Hash algorithm used when creating the self-signed certificate.</param>
    /// <param name="password">Password of the self-signed certificate.</param>
    /// <param name="contentType">Content type of the mounted self-signed certificate file.</param>
    /// <param name="subjectAlternativeNameAction">Subject alternative name builder action.</param>
    public SelfSignedCertificateConfiguration(
        string? containerPath = null,
        X500DistinguishedName? distinguishedName = null,
        int? keySize = null,
        UnixFileModes? unixFileModes = null,
        HashAlgorithmName? hashAlgorithm = null,
        string? password = null,
        X509ContentType? contentType = null,
        Action<SubjectAlternativeNameBuilder>? subjectAlternativeNameAction = null)
    {
        ContainerPath = containerPath;
        DistinguishedName = distinguishedName;
        KeySize = keySize;
        UnixFileModes = unixFileModes;
        HashAlgorithm = hashAlgorithm;
        Password = password;
        ContentType = contentType;
        _subjectAlternativeNameActions = subjectAlternativeNameAction != null
            ? new Action<SubjectAlternativeNameBuilder>[] { subjectAlternativeNameAction } : null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelfSignedCertificateConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old self-signed certificate resource configuration.</param>
    /// <param name="newValue">The new self-signed certificate resource configuration.</param>
    public SelfSignedCertificateConfiguration(ISelfSignedCertificateConfiguration oldValue, ISelfSignedCertificateConfiguration newValue)
      : base(oldValue, newValue)
    {
        ContainerPath = newValue.ContainerPath ?? oldValue.ContainerPath;
        DistinguishedName = newValue.DistinguishedName ?? oldValue.DistinguishedName;
        KeySize = newValue.KeySize ?? oldValue.KeySize;
        UnixFileModes = newValue.UnixFileModes ?? oldValue.UnixFileModes;
        HashAlgorithm = newValue.HashAlgorithm ?? newValue.HashAlgorithm;
        Password = newValue.Password ?? oldValue.Password;
        ContentType = newValue.ContentType ?? oldValue.ContentType;

        if (!newValue.SubjectAlternativeNameActions.Any())
            _subjectAlternativeNameActions = AsArray(oldValue.SubjectAlternativeNameActions);
        else if (!oldValue.SubjectAlternativeNameActions.Any())
            _subjectAlternativeNameActions = AsArray(newValue.SubjectAlternativeNameActions);
        else
        {
            // Concatenate actions from both old and new configurations.
            var oldActions = AsArray(oldValue.SubjectAlternativeNameActions);
            var newActions = AsArray(newValue.SubjectAlternativeNameActions);
            _subjectAlternativeNameActions = new Action<SubjectAlternativeNameBuilder>[oldActions.Length + newActions.Length];
            Array.Copy(oldActions, 0, _subjectAlternativeNameActions, 0, oldActions.Length);
            Array.Copy(newActions, 0, _subjectAlternativeNameActions, oldActions.Length, newActions.Length);
        }
    }

    /// <inheritdoc />
    public ICertificateConfiguration AsContentType(X509ContentType contentType) =>
        new SelfSignedCertificateConfiguration(this, new SelfSignedCertificateConfiguration(contentType: contentType));

    private static T[] AsArray<T>(IEnumerable<T> values) where T : class => values as T[] ?? values.ToArray();
}
