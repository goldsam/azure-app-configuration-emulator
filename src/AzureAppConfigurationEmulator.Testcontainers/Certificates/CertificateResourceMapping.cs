using System.Security.Cryptography.X509Certificates;
using System.Text;
using DotNet.Testcontainers;
using DotNet.Testcontainers.Configurations;

namespace AzureAppConfigurationEmulator.Testcontainers.Certificates;

/// <inheritdoc />
public class CertificateResourceMapping : ICertificateResourceMapping
{
    private const string DefaultContainerPathTemplate = "/usr/local/share/ca-certificates/azure-app-config-emulator.{0}";

    private readonly Lazy<X509Certificate2> _certificate;

    /// <inheritdoc/>
    public ICertificateConfiguration Configuration { get; }

    /// <inheritdoc/>
    public X509ContentType ContentType => Configuration.ContentType!.Value;

    /// <inheritdoc/>
    public X509Certificate2 Certificate => _certificate.Value;

    /// <inheritdoc />
    public UnixFileModes FileMode => Configuration.UnixFileModes ?? Unix.FileMode644;

    /// <inheritdoc />
    public MountType Type => MountType.Bind;

    /// <inheritdoc />
    public AccessMode AccessMode => AccessMode.ReadWrite;

    /// <inheritdoc />
    public string Source => string.Empty;

    /// <inheritdoc />
    public string Password => Configuration.Password ?? SelfSignedCertificateBuilder.DefaultCertificatePassword;

    /// <inheritdoc />
    public string Target => Configuration.ContainerPath ?? GetDefaultContainerPath(ContentType);

    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateResourceMapping" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">Self-signed certificate configuration</param>
    public CertificateResourceMapping(ISelfSignedCertificateConfiguration resourceConfiguration)
        : this(new Lazy<X509Certificate2>(() => new SelfSignedCertificateBuilder(resourceConfiguration).CreateSelfSignedCertificate()), resourceConfiguration)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateResourceMapping" /> class.
    /// </summary>
    /// <param name="certificate">The certificate to be mounted.</param>
    /// <param name="resourceConfiguration">Certificate configuration.</param>
    public CertificateResourceMapping(X509Certificate2 certificate, ICertificateConfiguration resourceConfiguration)
        : this(new Lazy<X509Certificate2>(Guard.Argument(certificate, nameof(certificate)).NotNull().Value), resourceConfiguration)
    {}

    private CertificateResourceMapping(Lazy<X509Certificate2> certificate, ICertificateConfiguration resourceConfiguration)
    {
        _certificate = certificate;
        Configuration = Guard.Argument(resourceConfiguration, nameof(resourceConfiguration)).NotNull().Value;
    }

    /// <inheritdoc />
    public Task CreateAsync(CancellationToken _) => Task.CompletedTask;

    /// <inheritdoc />
    public Task DeleteAsync(CancellationToken _) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task<byte[]> GetAllBytesAsync(CancellationToken _) =>
        Task.FromResult(ContentType switch
        {
            X509ContentType.Pfx => Certificate.Export(X509ContentType.Pfx, Password),
            X509ContentType.Cert => Encoding.ASCII.GetBytes(EncodeCertificateToPEM(Certificate)),
            _ => throw new NotSupportedException($"Unsupported content type: {ContentType}")
        });

    /// <inheritdoc/>
    public ICertificateResourceMapping AsContentType(X509ContentType contentType) =>
        new CertificateResourceMapping(_certificate, Configuration.AsContentType(contentType));

    private static string EncodeCertificateToPEM(X509Certificate2 certificate)
    {
        _ = Guard.Argument(certificate, nameof(certificate)).NotNull(); 
        return new StringBuilder()
            .AppendLine("-----BEGIN CERTIFICATE-----")
            .AppendLine(Convert.ToBase64String(certificate.RawData, Base64FormattingOptions.InsertLineBreaks))
            .AppendLine("-----END CERTIFICATE-----")
            .ToString();
    }

    private static string GetDefaultContainerPath(X509ContentType contentType) => contentType switch
    {
        X509ContentType.Pfx => string.Format(DefaultContainerPathTemplate, "pfx"),
        X509ContentType.Cert => string.Format(DefaultContainerPathTemplate, "crt"),
        _ => throw new NotSupportedException($"Unsupported content type: {contentType}")
    };
}
