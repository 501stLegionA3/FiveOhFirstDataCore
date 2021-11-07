using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Result;

public class ActionResult
{
    protected bool Success { get; init; }
    protected List<string>? Errors { get; init; }

    public ActionResult(bool success, List<string>? errors = null)
        => (Success, Errors) = (success, errors);

    public bool GetResult([NotNullWhen(false)] out List<string>? errors)
    {
        if (Success)
            errors = null;
        else
            errors = Errors ?? new();

        return Success;
    }
}

public class ActionResult<T> : ActionResult
{
    public T? Result { get; init; }

    public ActionResult(bool success, List<string>? errors = null, T? result = default)
        : base(success, errors)
        => Result = result;

    public bool GetResult([NotNullWhen(true)] out T? result, 
        [NotNullWhen(false)] out List<string>? errors)
    {
        if (Success)
        {
            errors = null;
            result = Result ?? default;
        }
        else
        {
            errors = Errors ?? new();
            result = default;
        }

        return Success;
    }
}
