namespace AzureAppConfigurationEmulator.ConfigurationSettings;

public interface IConfigurationSettingFactory
{
    public ConfigurationSetting Create(
        string key,
        string? label = null,
        string? contentType = null,
        string? value = null,
        IDictionary<string, string>? tags = null);

    public ConfigurationSetting Create(
        string etag,
        string key,
        DateTimeOffset lastModified,
        bool locked,
        string? label = null,
        string? contentType = null,
        string? value = null,
        IDictionary<string, string>? tags = null);
}
