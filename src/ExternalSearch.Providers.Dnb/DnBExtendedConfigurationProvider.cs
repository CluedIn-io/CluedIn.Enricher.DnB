using System;
using System.Linq;
using System.Threading.Tasks;

using CluedIn.Core;
using CluedIn.Core.Providers.ExtendedConfiguration;

namespace CluedIn.ExternalSearch.Providers.DnB;

internal class DnBExtendedConfigurationProvider : IExtendedConfigurationProvider
{
    internal const string SourceName = "DnBExtendedConfigurationProvider";
    private const int DefaultPageSize = 20;

    private static readonly Option[] SupportedIdentityResolutionAPIsOptions = DnBConstants.SupportedIdentityResolutionAPIs.Values
        .Select(api => new Option(
            api.Value.ToLowerInvariant(),
            api.Label,
            Description: api.Description
        )).ToArray();

    public Task<CanHandleResponse> CanHandle(ExecutionContext context, ExtendedConfigurationRequest request)
    {
        return Task.FromResult(new CanHandleResponse
        {
            CanHandle = request.Source == SourceName
        });
    }

    public Task<ResolveOptionByValueResponse> ResolveOptionByValue(ExecutionContext context, ResolveOptionByValueRequest request)
    {
        var found = request.Key switch
        {
            DnBConstants.KeyName.IdentityResolutionApi => HandleIdentityResolutionAPIs().Data.SingleOrDefault(item => item.Value.Equals(request.Value, StringComparison.OrdinalIgnoreCase)),
            _ => null,
        };

        return Task.FromResult(new ResolveOptionByValueResponse
        {
            Option = found,
        });
    }

    public Task<ResolveOptionsResponse> ResolveOptions(ExecutionContext context, ResolveOptionsRequest request)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);

        return Task.FromResult(request.Key switch
        {
            DnBConstants.KeyName.IdentityResolutionApi => HandleIdentityResolutionAPIs(),

            _ => ResolveOptionsResponse.Empty,
        });
    }

    private static ResolveOptionsResponse HandleIdentityResolutionAPIs()
    {
        return new ResolveOptionsResponse
        {
            Data = SupportedIdentityResolutionAPIsOptions,
            Total = SupportedIdentityResolutionAPIsOptions.Length,
            Page = 0,
            Take = DefaultPageSize,
        };
    }
}


