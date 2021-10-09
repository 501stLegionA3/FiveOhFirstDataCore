using FiveOhFirstDataCore.Data.Structures.Transfer;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures;

public class TransferResult : ResultBase
{
    private readonly TransferRequest? _request;

    public TransferResult(bool success, TransferRequest? transfer = null, List<string>? errors = null)
        : base(success, errors)
        => (_request) = (transfer);

    public virtual bool GetResult([NotNullWhen(true)] out TransferRequest? transfer,
        [NotNullWhen(false)] out List<string>? errors)
    {
        if (Success)
        {
            transfer = _request;
            errors = null;
        }
        else
        {
            transfer = null;
            errors = Errors;
        }

        return Success;
    }
}
