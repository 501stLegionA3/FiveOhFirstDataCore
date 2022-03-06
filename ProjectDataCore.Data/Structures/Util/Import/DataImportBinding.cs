using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System.Collections.Concurrent;
using System.ComponentModel;

namespace ProjectDataCore.Data.Structures.Util.Import;

public class DataImportBinding
{
    public bool IsStatic { get; set; } = false;
    public string PropertyName { get; set; } = "";
    public ConcurrentDictionary<string, object> DataValues { get; set; } = new();
    public ConcurrentDictionary<string, (dynamic, List<(string, dynamic)>)> DataValueModels { get; set; } = new();
    public List<RosterImportConditional> RosterImportConditionals { get; set; } = new();
    public bool AutoConvert { get; set; } = false;

    public bool IsUsernameIdentifier { get; set; } = false;
    public bool IsUserIdIdentifier { get; set; } = false;

    public bool PasswordIdentifier { get; set; } = false;
    public bool EmailIdentifier { get; set; } = false;
    public bool RosterIdentifier { get; set; } = false;

    public bool ValueInRosterImports(string value)
    {
        foreach(var condition in RosterImportConditionals)
        {
            if (condition.Checks.ToList(x => x.Value).Contains(value))
                return true;
        }

        return false;
    }
}

public class RosterImportConditional
{
    // Unqiue Value, Next Conditional, (start, no, or end parens)
    // parens values: -1 start, 0 middle, 1 end
    private readonly List<RosterImportConditionalStatement> _checks = new();
    public IReadOnlyList<RosterImportConditionalStatement> Checks { get
        {
            return _checks;
        }
    }

    public Guid RosterTree { get; set; }
    public List<Range> SlotRange { get; set; } = new();

    public int AddRangeStart { get; set; } = 0;
    public int AddRangeEnd { get; set; } = 0;

    public void AddCheck(string value, PreviousConditional? condition = null, int parenValue = 0)
        => _checks.Add(new()
        {
            Value = value,
            PreviousConditional = condition,
            ParenValue = parenValue,
        });

    public void DeleteCheck(int index)
        => _checks.RemoveAt(index);

    public void MoveCheck(int index, int newIndex)
    {
        var value = _checks[index];
        _checks.RemoveAt(index);
        _checks.Insert(newIndex, value);
    }

    public bool Validate()
    {
        Stack<int> parens = new();
        foreach(var check in _checks)
        {
            if (check.ParenValue == -1)
                parens.Push(check.ParenValue);
            else if (check.ParenValue == 1)
                if (parens.TryPeek(out var last) && last == -1)
                    parens.Pop();
                else // unbalanced (two close, one open, etc)
                    return false;
        }

        // Unbalanced. too many opens not enough closes
        if (parens.Count > 0)
            return false;
        // Balanced
        return true;
    }

    public bool Resolve(List<string> values)
    {
        Stack<bool> checkStack = new();
        Stack<PreviousConditional?> beforeCompareStack = new();
        checkStack.Push(true);

        foreach (var check in _checks)
        {
            var prevValue = checkStack.Pop();

            var curValue = values.Contains(check.Value);

            if (check.ParenValue == 1)
            {
                curValue = Compare(prevValue, check.PreviousConditional, curValue);
                var beforeValue = checkStack.Pop();
                checkStack.Push(Compare(beforeValue, beforeCompareStack.Pop(), curValue));
            }
            else if (check.ParenValue == -1)
            {
                checkStack.Push(prevValue);
                checkStack.Push(curValue);
                beforeCompareStack.Push(check.PreviousConditional);
            }
            else
            {
                curValue = Compare(prevValue, check.PreviousConditional, curValue);
                checkStack.Push(curValue);
            }
        }

        return checkStack.Pop();
    }

    private static bool Compare(bool prev, PreviousConditional? condition, bool next) 
        => condition switch
        {
            PreviousConditional.AND => prev && next,
            PreviousConditional.OR => prev || next,
            _ => prev && next,
        };
}

public class RosterImportConditionalStatement
{
    public string Value { get; set; }
    public PreviousConditional? PreviousConditional { get; set; }
    public int ParenValue { get; set; }
}

public enum PreviousConditional
{
    [Description("AND")]
    AND,
    [Description("OR")]
    OR
}
