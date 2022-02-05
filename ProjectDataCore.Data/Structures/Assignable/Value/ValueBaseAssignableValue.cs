﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public class ValueBaseAssignableValue<T> : BaseAssignableValue, IAssignableValue<T>
{
    /// <summary>
    /// The currently set value.
    /// </summary>
    public List<T> SetValue { get; set; } = new();

    public override object? GetValue()
    {
        return SetValue.FirstOrDefault();
    }

    public override List<object?> GetValues()
    {
        return SetValue.ToList(x => (object?)x);
    }

    public override void ReplaceValue(object value, int? index = null)
    {
        if(index is null)
        {
            // equivalent of the set value return.
            SetValue = ((List<object>)value).ToList(x => (T)x);
        }
        else
        {
            SetValue[index.Value] = (T)value;
        }
    }

    public override void AddValue(object value)
    {
        SetValue.Add((T)value);
    }
}