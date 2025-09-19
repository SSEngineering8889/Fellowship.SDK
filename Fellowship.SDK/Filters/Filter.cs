using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Fellowship.SDK.Filters;

public class Filter<TModel>
{
    public string Field { get; }
    public FilterOperator Operator { get; }
    public string? Value { get; }

    public Filter(Expression<Func<TModel, object>> fieldSelector, FilterOperator op, string? value = null)
    {
        Field = GetJsonPropertyName(fieldSelector)
                ?? throw new ArgumentException("Invalid field selector");
        Operator = op;

        if ((op == FilterOperator.Regex || op == FilterOperator.NotRegex) && value != null)
        {
            ValidateRegex(value);
        }

        Value = value;
    }

    private static void ValidateRegex(string regexString)
    {
        try
        {
            // Strip leading/trailing slashes if user passed `/pattern/i`
            var pattern = regexString.Trim('/');

            // Check if it has flags (like /pattern/i)
            var hasIgnoreCase = regexString.EndsWith("i");
            var options = hasIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            _ = new Regex(pattern, options); // Will throw if invalid
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid regex pattern: {regexString}", ex);
        }
    }

    public override string ToString()
    {
        return Operator switch
        {
            FilterOperator.Match => $"{Field}={Value}",
            FilterOperator.NotMatch => $"{Field}!={Value}",
            FilterOperator.In => $"{Field}={Value}",
            FilterOperator.NotIn => $"{Field}!={Value}",
            FilterOperator.Exists => $"{Field}",
            FilterOperator.NotExists => $"!{Field}",
            FilterOperator.Regex => $"{Field}={Value}",
            FilterOperator.NotRegex => $"{Field}!={Value}",
            FilterOperator.GreaterThan => $"{Field}>{Value}",
            FilterOperator.GreaterThanOrEqual => $"{Field}>={Value}",
            FilterOperator.LessThan => $"{Field}<{Value}",
            FilterOperator.LessThanOrEqual => $"{Field}<={Value}",
            _ => throw new NotSupportedException($"Unsupported operator {Operator}")
        };
    }

    private static string? GetJsonPropertyName(Expression<Func<TModel, object>> expr)
    {
        MemberExpression? member = expr.Body switch
        {
            MemberExpression m => m,
            UnaryExpression u when u.Operand is MemberExpression m => m,
            _ => null
        };

        if (member?.Member is not PropertyInfo prop)
            return null;

        var attr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
        return attr?.Name ?? prop.Name;
    }
}
