using System.Data.Common;
using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using AzureAppConfigurationEmulator.Data;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.ConfigurationSettings;

public class ConfigurationSettingRepositoryTests
{
    private IDbCommandFactory CommandFactory { get; set; }

    private IConfigurationSettingFactory ConfigurationSettingFactory { get; set; }

    private IDbConnectionFactory ConnectionFactory { get; set; }

    private ILogger<ConfigurationSettingRepository> Logger { get; set; }

    private IDbParameterFactory ParameterFactory { get; set; }

    private ConfigurationSettingRepository Repository { get; set; }

    [SetUp]
    public void SetUp()
    {
        CommandFactory = Substitute.For<IDbCommandFactory>();
        CommandFactory
            .Create(Arg.Any<DbConnection>(), Arg.Any<string?>(), Arg.Any<IEnumerable<DbParameter>?>())
            .Returns(_ =>
            {
                var command = Substitute.For<DbCommand>();

                command.ExecuteReaderAsync().Returns(_ =>
                {
                    var reader = Substitute.For<DbDataReader>();
                    return reader;
                });

                return command;
            });

        ConfigurationSettingFactory = Substitute.For<IConfigurationSettingFactory>();

        ConnectionFactory = Substitute.For<IDbConnectionFactory>();
        ConnectionFactory
            .Create()
            .Returns(_ =>
            {
                var connection = Substitute.For<DbConnection>();
                return connection;
            });

        Logger = Substitute.For<ILogger<ConfigurationSettingRepository>>();

        ParameterFactory = Substitute.For<IDbParameterFactory>();

        Repository = new ConfigurationSettingRepository(CommandFactory, ConfigurationSettingFactory, ConnectionFactory, Logger, ParameterFactory);
    }

    [Test]
    public async Task Add_CommandText_ConfigurationSetting()
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, null, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is("INSERT INTO configuration_settings (etag, key, label, content_type, value, last_modified, locked, tags) VALUES ($etag, $key, $label, $content_type, $value, $last_modified, $locked, $tags)"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    [Test]
    public async Task Add_ContentTypeParameter_ConfigurationSetting()
    {
        // Arrange
        const string contentType = "TestContentType";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, contentType, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$content_type"),
                Arg.Is(contentType));
    }

    [Test]
    public async Task Add_EtagParameter_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        var setting = new ConfigurationSetting(etag, "TestKey", DateTimeOffset.UtcNow, false, null, null, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$etag"),
                Arg.Is(etag));
    }

    [Test]
    public async Task Add_KeyParameter_ConfigurationSetting()
    {
        // Arrange
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, DateTimeOffset.UtcNow, false, null, null, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$key"),
                Arg.Is(key));
    }

    [Test]
    public async Task Add_LabelParameter_ConfigurationSetting()
    {
        // Arrange
        const string label = "TestLabel";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, label, null, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$label"),
                Arg.Is(label));
    }

    [Test]
    public async Task Add_LastModifiedParameter_ConfigurationSetting()
    {
        // Arrange
        var lastModified = DateTimeOffset.UtcNow;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", lastModified, false, null, null, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$last_modified"),
                Arg.Is(lastModified));
    }

    [Test]
    public async Task Add_LockedParameter_ConfigurationSetting()
    {
        // Arrange
        const bool locked = false;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, locked, null, null, null, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$locked"),
                Arg.Is(locked));
    }

    [Test]
    public async Task Add_TagsParameter_ConfigurationSetting()
    {
        // Arrange
        var tags = new Dictionary<string, string> { { "TestKey", "TestValue" } };
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, null, null, tags);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$tags"),
                Arg.Is(tags));
    }

    [Test]
    public async Task Add_ValueParameter_ConfigurationSetting()
    {
        // Arrange
        const string value = "TestValue";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, null, value, null);

        // Act
        await Repository.Add(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$value"),
                Arg.Is(value));
    }

    [TestCaseSource(nameof(Get_CommandText_KeyAndLabelAndMoment_TestCases))]
    public async Task Get_CommandText_KeyAndLabelAndMoment(string key, string label, DateTimeOffset? moment, string expected)
    {
        // Act
        await Repository.Get(key, label, moment).ToListAsync();

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is($"SELECT etag, key, label, content_type, value, last_modified, locked, tags{expected}"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    // ReSharper disable once InconsistentNaming
    private static object[] Get_CommandText_KeyAndLabelAndMoment_TestCases =
    [
        new object?[]
        {
            KeyFilter.Any, LabelFilter.Any, null,
            " FROM configuration_settings"
        },
        new object?[]
        {
            "TestKey", LabelFilter.Any, null,
            " FROM configuration_settings WHERE (key = $key0)"
        },
        new object?[]
        {
            "TestKey*", LabelFilter.Any, null,
            " FROM configuration_settings WHERE (key LIKE $key0)"
        },
        new object?[]
        {
            "TestKey,TestKey*", LabelFilter.Any, null,
            " FROM configuration_settings WHERE (key = $key0 OR key LIKE $key1)"
        },
        new object?[]
        {
            KeyFilter.Any, LabelFilter.Null, null,
            " FROM configuration_settings WHERE (label IS NULL)"
        },
        new object?[]
        {
            KeyFilter.Any, "TestLabel", null,
            " FROM configuration_settings WHERE (label = $label0)"
        },
        new object?[]
        {
            KeyFilter.Any, "TestLabel*", null,
            " FROM configuration_settings WHERE (label LIKE $label0)"
        },
        new object?[]
        {
            KeyFilter.Any, $"{LabelFilter.Null},TestLabel,TestLabel*", null,
            " FROM configuration_settings WHERE (label IS NULL OR label = $label1 OR label LIKE $label2)"
        },
        new object?[]
        {
            KeyFilter.Any, LabelFilter.Any, DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
            " FROM configuration_settings_history WHERE (valid_from <= $moment AND valid_to > $moment)"
        },
        new object?[]
        {
            "TestKey", LabelFilter.Null, DateTimeOffset.Parse("2023-10-01T00:00:00+00:00"),
            " FROM configuration_settings_history WHERE (key = $key0) AND (label IS NULL) AND (valid_from <= $moment AND valid_to > $moment)"
        }
    ];

    [TestCase("TestKey", "TestLabel", " WHERE key = $key AND label = $label")]
    [TestCase("TestKey", null, " WHERE key = $key AND label IS NULL")]
    public async Task Remove_CommandText_ConfigurationSetting(string key, string? label, string expected)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", key, DateTimeOffset.UtcNow, false, label, null, null, null);

        // Act
        await Repository.Remove(setting);

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is($"DELETE FROM configuration_settings{expected}"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    [Test]
    public async Task Remove_KeyParameter_ConfigurationSetting()
    {
        // Arrange
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, DateTimeOffset.UtcNow, false, null, null, null, null);

        // Act
        await Repository.Remove(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$key"),
                Arg.Is(key));
    }

    [TestCase("TestLabel")]
    [TestCase(null)]
    public async Task Remove_LabelParameter_ConfigurationSetting(string? label)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, label, null, null, null);

        // Act
        await Repository.Remove(setting);

        // Assert
        if (label is not null)
        {
            ParameterFactory
                .Received()
                .Create(
                    Arg.Is("$label"),
                    Arg.Is(label));
        }
        else
        {
            ParameterFactory
                .DidNotReceive()
                .Create(
                    Arg.Is("$label"),
                    Arg.Any<object?>());
        }
    }

    [TestCase("TestKey", "TestLabel", " WHERE key = $key AND label = $label")]
    [TestCase("TestKey", null, " WHERE key = $key AND label IS NULL")]
    public async Task Update_CommandText_ConfigurationSetting(string key, string? label, string expected)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", key, DateTimeOffset.UtcNow, false, label, null, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        CommandFactory
            .Received()
            .Create(
                Arg.Any<DbConnection>(),
                Arg.Is($"UPDATE configuration_settings SET etag = $etag, content_type = $content_type, value = $value, last_modified = $last_modified, locked = $locked, tags = $tags{expected}"),
                Arg.Any<IEnumerable<DbParameter>?>());
    }

    [Test]
    public async Task Update_ContentTypeParameter_ConfigurationSetting()
    {
        // Arrange
        const string contentType = "TestContentType";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, contentType, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$content_type"),
                Arg.Is(contentType));
    }

    [Test]
    public async Task Update_EtagParameter_ConfigurationSetting()
    {
        // Arrange
        const string etag = "TestEtag";
        var setting = new ConfigurationSetting(etag, "TestKey", DateTimeOffset.UtcNow, false, null, null, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$etag"),
                Arg.Is(etag));
    }

    [Test]
    public async Task Update_KeyParameter_ConfigurationSetting()
    {
        // Arrange
        const string key = "TestKey";
        var setting = new ConfigurationSetting("TestEtag", key, DateTimeOffset.UtcNow, false, null, null, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$key"),
                Arg.Is(key));
    }

    [TestCase("TestLabel")]
    [TestCase(null)]
    public async Task Update_LabelParameter_ConfigurationSetting(string? label)
    {
        // Arrange
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, label, null, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        if (label is not null)
        {
            ParameterFactory
                .Received()
                .Create(
                    Arg.Is("$label"),
                    Arg.Is(label));
        }
        else
        {
            ParameterFactory
                .DidNotReceive()
                .Create(
                    Arg.Is("$label"),
                    Arg.Any<object?>());
        }
    }

    [Test]
    public async Task Update_LastModifiedParameter_ConfigurationSetting()
    {
        // Arrange
        var lastModified = DateTimeOffset.UtcNow;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", lastModified, false, null, null, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$last_modified"),
                Arg.Is(lastModified));
    }

    [Test]
    public async Task Update_LockedParameter_ConfigurationSetting()
    {
        // Arrange
        const bool locked = false;
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, locked, null, null, null, null);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$locked"),
                Arg.Is(locked));
    }

    [Test]
    public async Task Update_TagsParameter_ConfigurationSetting()
    {
        // Arrange
        var tags = new Dictionary<string, string> { { "TestKey", "TestValue" } };
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, null, null, tags);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$tags"),
                Arg.Is(tags));
    }

    [Test]
    public async Task Update_ValueParameter_ConfigurationSetting()
    {
        // Arrange
        const string value = "TestValue";
        var setting = new ConfigurationSetting("TestEtag", "TestKey", DateTimeOffset.UtcNow, false, null, null, value, null);

        // Act
        await Repository.Update(setting);

        // Assert
        ParameterFactory
            .Received()
            .Create(
                Arg.Is("$value"),
                Arg.Is(value));
    }
}
