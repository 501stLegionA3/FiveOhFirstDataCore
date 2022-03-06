using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System.Collections.Concurrent;

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
}

public class RosterImportConditional
{
    // Unqiue Value, Next Conditional, (start, no, or end parens)
    // parens values: -1 start, 0 middle, 1 end
    public List<(string, PreviousConditional?, int)> Checks { get; set; } = new();
    public Guid RosterTree { get; set; }
    public Range SlotRange { get; set; }

    public bool Resolve(List<string> values)
    {
        Stack<bool> checkStack = new();
        checkStack.Push(true);

        foreach (var check in Checks)
        {
            var prevValue = checkStack.Pop();

            var curValue = values.Contains(check.Item1);

            if (check.Item3 == 1)
            {
                var beforeValue = checkStack.Pop();
                checkStack.Push(Compare(beforeValue, check.Item2, prevValue));
            }
            else if (check.Item3 == -1)
            {
                checkStack.Push(prevValue);
                checkStack.Push(curValue);
            }
            else
            {

                checkStack.Push(prevValue);
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

public enum PreviousConditional
{
    AND,
    OR
}
