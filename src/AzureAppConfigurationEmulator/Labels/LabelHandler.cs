using System.Text.RegularExpressions;
using AzureAppConfigurationEmulator.Common;
using AzureAppConfigurationEmulator.ConfigurationSettings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureAppConfigurationEmulator.Labels;

public class LabelHandler
{
    public static async Task<Results<LabelsResult, InvalidCharacterResult, TooManyValuesResult>> List(
        [FromServices] IConfigurationSettingRepository repository,
        [FromQuery] string name = LabelFilter.Any,
        [FromQuery(Name = "$select")] string? select = default,
        [FromHeader(Name = "Accept-Datetime")] DateTimeOffset? acceptDatetime = default,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity($"{nameof(LabelHandler)}.{nameof(List)}");
        activity?.SetTag(Telemetry.QueryName, name);
        activity?.SetTag(Telemetry.HeaderAcceptDatetime, acceptDatetime);

        if (name != LabelFilter.Any)
        {
            if (new Regex(@"(?=.*(?<!\\),)(?=.*\*)").IsMatch(name))
            {
                return new InvalidCharacterResult(nameof(name));
            }

            if (new Regex(@"(?:.*(?<!\\),){5,}").IsMatch(name))
            {
                return new TooManyValuesResult(nameof(name));
            }
        }

        var labels = await repository.Get(label: name, moment: acceptDatetime, cancellationToken: cancellationToken)
            .Select(setting => setting.Label)
            .Distinct()
            .ToListAsync(cancellationToken);

        return new LabelsResult(labels, acceptDatetime, select);
    }
}
