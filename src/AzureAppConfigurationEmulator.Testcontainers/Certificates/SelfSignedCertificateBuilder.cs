using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Builders;
using System.Net;
using DotNet.Testcontainers;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates;

/// <inheritdoc />
public class SelfSignedCertificateBuilder

    : AbstractBuilder<SelfSignedCertificateBuilder, ICertificateResourceMapping, CertificateRequest, ISelfSignedCertificateConfiguration>
    , ISelfSignedCertificateBuilder<SelfSignedCertificateBuilder>
{
    /// <summary>
    /// Default certificate password.
    /// </summary>
    public static string DefaultCertificatePassword { get; } = "MyS3cur3P@ssw0rd";

    /// <summary>
    /// Default certificate signing hash algorithm.
    /// </summary>
    public static HashAlgorithmName DefaultHashAlgorithm { get; } = HashAlgorithmName.SHA256;

    /// <summary>
    /// Default distinguished name of the self-signed certificate.
    /// </summary>
    public static X500DistinguishedName DefaultDistinguishedName => new($"CN=MyAppCert");

    /// <summary>
    /// Default content type of the mounted self-signed certificate file.
    /// </summary>
    public static X509ContentType DefaultContentType { get; } = X509ContentType.Pfx;

    /// <summary>
    /// Gets the self-signed certificate configuration.
    /// </summary>
    internal ISelfSignedCertificateConfiguration Configuration =>
        DockerResourceConfiguration;

    /// <inheritdoc />
    protected override ISelfSignedCertificateConfiguration DockerResourceConfiguration { get; }

    public SelfSignedCertificateBuilder()
        : this(new SelfSignedCertificateConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    public SelfSignedCertificateBuilder(ISelfSignedCertificateConfiguration configuration)
        : base(configuration)
    {
        DockerResourceConfiguration = configuration;
    }

    /// <inheritdoc />
    public SelfSignedCertificateBuilder WithDistinguishedName(X500DistinguishedName distinguishedName) =>
        Merge(new SelfSignedCertificateConfiguration(distinguishedName: distinguishedName));

    /// <inheritdoc />
    public SelfSignedCertificateBuilder WithKeySize(int keySize) =>
        Merge(new SelfSignedCertificateConfiguration(keySize: keySize));

    /// <inheritdoc />
    public SelfSignedCertificateBuilder WithUnixFileModes(UnixFileModes unixFileModes) =>
        Merge(new SelfSignedCertificateConfiguration(unixFileModes: unixFileModes));

    /// <inheritdoc />
    public SelfSignedCertificateBuilder WithHashAlgorithm(HashAlgorithmName hashAlgorithm) =>
        Merge(new SelfSignedCertificateConfiguration(hashAlgorithm: hashAlgorithm));

    /// <inheritdoc />
    public SelfSignedCertificateBuilder WithPassword(string password) =>
        Merge(new SelfSignedCertificateConfiguration(password: password));

    /// <inheritdoc />
    public SelfSignedCertificateBuilder AndSubjectAlternativeName(Action<SubjectAlternativeNameBuilder> subjectAlternativeNameAction)
    {
        _ = Guard.Argument(subjectAlternativeNameAction, nameof(subjectAlternativeNameAction)).NotNull();
        var newValue = new SelfSignedCertificateConfiguration(subjectAlternativeNameAction: subjectAlternativeNameAction);
        return Merge(newValue);
    }

    /// <inheritdoc />
    public override ICertificateResourceMapping Build() => new CertificateResourceMapping(DockerResourceConfiguration);

    /// <inheritdoc />
    protected override SelfSignedCertificateBuilder Init() =>
        base.Init()
            .AndSubjectAlternativeName(san =>
            {
                san.AddIpAddress(IPAddress.Loopback);
                san.AddIpAddress(IPAddress.IPv6Loopback);
                san.AddDnsName("localhost");
                san.AddDnsName(Environment.MachineName);
            });

    /// <inheritdoc />
    protected override SelfSignedCertificateBuilder Merge(ISelfSignedCertificateConfiguration oldValue, ISelfSignedCertificateConfiguration newValue) =>
        new SelfSignedCertificateBuilder(new SelfSignedCertificateConfiguration(oldValue, newValue));

    /// <inheritdoc />
    protected override SelfSignedCertificateBuilder Clone(IResourceConfiguration<CertificateRequest> resourceConfiguration) =>
        Merge(DockerResourceConfiguration, new SelfSignedCertificateConfiguration(resourceConfiguration));

    private SelfSignedCertificateBuilder Merge(SelfSignedCertificateConfiguration newValue) =>
        Merge(DockerResourceConfiguration, newValue);

    internal X509Certificate2 CreateSelfSignedCertificate()
    {
        using (var rsa = DockerResourceConfiguration.KeySize.HasValue ? RSA.Create(DockerResourceConfiguration.KeySize.Value) : RSA.Create())
        {
            var distinguishedName = DockerResourceConfiguration.DistinguishedName ?? DefaultDistinguishedName;
            var hashAlgorithm = DockerResourceConfiguration.HashAlgorithm ?? DefaultHashAlgorithm;

            var request = new CertificateRequest(distinguishedName, rsa, hashAlgorithm, RSASignaturePadding.Pkcs1);

            // Key Usage
            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DataEncipherment,
                    critical: true)
                );

            // Extended Key Usage
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    new OidCollection {
                        new Oid("1.3.6.1.5.5.7.3.1") // serverAuth
                    },
                    critical: true)
                );

            // Basic Constraints
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: true,
                    hasPathLengthConstraint: false,
                    pathLengthConstraint: 0,
                    critical: true)
                );

            // Subject Alternative Name
            request.CertificateExtensions.Add(CreateSubjectAlternativeName());

            // Certificate validity period
            var now = DateTimeOffset.UtcNow;
            var notBefore = now.AddDays(-1);
            var notAfter = now.AddYears(1);

            // Create the self-signed certificate.
            return request.CreateSelfSigned(notBefore, notAfter);
        }
    }

    private X509Extension CreateSubjectAlternativeName()
    {
        var sanBuilderActions = DockerResourceConfiguration.SubjectAlternativeNameActions;
        if (sanBuilderActions?.Any() != true)
            throw new InvalidOperationException("No Subject Alternative Name actions are defined.");

        var sanBuilder = new SubjectAlternativeNameBuilder();
        foreach (var sanBuilderAction in sanBuilderActions)
            sanBuilderAction(sanBuilder);

        return sanBuilder.Build(critical: true);
    }
}
