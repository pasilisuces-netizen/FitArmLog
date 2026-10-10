namespace FitArmLog.Validation;

public class ValidationResult
{
    private readonly List<string> _errors = new();

    public IReadOnlyList<string> Errors => _errors;
    public bool IsValid => _errors.Count == 0;
    public string FirstError => _errors.FirstOrDefault() ?? string.Empty;

    public void Add(string error) => _errors.Add(error);
}
