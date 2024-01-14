using AzureAppConfigurationEmulator.Entities;
using AzureAppConfigurationEmulator.Handlers;
using AzureAppConfigurationEmulator.Repositories;
using AzureAppConfigurationEmulator.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NUnit.Framework;

namespace AzureAppConfigurationEmulator.Tests.Handlers;

public class LockHandlerTests
{
    [Test]
    public async Task Lock_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Lock_KeyValueResult_MatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Lock_KeyValueResult_NonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Lock_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Lock_PreconditionFailedResult_MatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Lock_PreconditionFailedResult_NonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Lock(repository, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Unlock_KeyValueResult_ExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Unlock_KeyValueResult_MatchingIfMatch(string ifMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "TestKey", ifMatch: ifMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Unlock_KeyValueResult_NonMatchingIfNoneMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "TestKey", ifNoneMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<KeyValueResult>());
    }

    [Test]
    public async Task Unlock_NotFoundResult_NonExistingConfigurationSetting()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = Enumerable.Empty<ConfigurationSetting>();
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "TestKey");

        // Assert
        Assert.That(results.Result, Is.TypeOf<NotFound>());
    }

    [TestCase("TestEtag")]
    [TestCase("*")]
    public async Task Unlock_PreconditionFailedResult_MatchingIfNoneMatch(string ifNoneMatch)
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "TestKey", ifNoneMatch: ifNoneMatch);

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }

    [Test]
    public async Task Unlock_PreconditionFailedResult_NonMatchingIfMatch()
    {
        // Arrange
        var repository = Substitute.For<IConfigurationSettingRepository>();
        var settings = new List<ConfigurationSetting>
        {
            new("TestEtag", "TestKey", null, null, null, DateTimeOffset.UtcNow, false, null)
        };
        repository.Get(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTimeOffset?>(), Arg.Any<CancellationToken>()).Returns(settings.ToAsyncEnumerable());

        // Act
        var results = await LockHandler.Unlock(repository, "TestKey", ifMatch: "abc");

        // Assert
        Assert.That(results.Result, Is.TypeOf<PreconditionFailedResult>());
    }
}
