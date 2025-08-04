namespace LeoBanking.Shared.Wrappers;

public interface IConfigurationWrapper
{
    string GetValue(string sectionName);
}