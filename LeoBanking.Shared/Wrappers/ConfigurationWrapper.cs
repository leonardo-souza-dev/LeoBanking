using Microsoft.Extensions.Configuration;

namespace LeoBanking.Shared.Wrappers;

public class ConfigurationWrapper(IConfiguration configuration) : IConfigurationWrapper
{
    public string GetValue(string sectionName)
    {
        var configurationSection = configuration.GetSection(sectionName);

        if (configurationSection == null || configurationSection.Value == null)
        {
            throw new ArgumentException($"No section '${sectionName}' found in the configuration file");
        }

        return configurationSection.Value;
    }
}