using Microsoft.AspNetCore.Components.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Events.Parameters;
public class OnPressEventArgs : EventArgs
{
    public string Key
    {
        get
        {
            return key.ToString();
        }

        set
        {
            key = value.FirstOrDefault();
            lowerKeyChar = value.ToLower().FirstOrDefault();
        }
    }

    public bool ShiftKey
    {
        get
        {
            return Convert.ToBoolean(shift);
        }

        set
        {
            shift = value.ToInt32();
        }
    }

    public bool CtrlKey
    {
        get
        {
            return Convert.ToBoolean(ctrl);
        }

        set
        {
            ctrl = value.ToInt32();
        }
    }

    public bool AltKey 
    {
        get
        {
            return Convert.ToBoolean(alt);
        }

        set
        {
            alt = value.ToInt32();
        }
    }

    public bool MetaKey 
    { 
        get
        {
            return Convert.ToBoolean(meta);
        }

        set
        {
            meta = value.ToInt32();
        }
    }

    private char key;
    private char lowerKeyChar;
    private int shift;
    private int ctrl;
    private int alt;
    private int meta;

    public OnPressEventArgs()
        : this("", false, false, false, false) { }

    public OnPressEventArgs(KeyboardEventArgs args)
        : this(args.Code, args.ShiftKey, args.CtrlKey, args.AltKey, args.MetaKey) { }

    public OnPressEventArgs(string code, bool shiftKey, bool ctrlKey, bool altKey, bool metaKey)
    {
        Key = code;
        ShiftKey = shiftKey;
        CtrlKey = ctrlKey;
        AltKey = altKey;
        MetaKey = metaKey;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is OnPressEventArgs args)
            return GetHashCode().Equals(args.GetHashCode());

        return false;
    }

    public override int GetHashCode()
        => lowerKeyChar.ShiftAndWrap(2) ^ shift.ShiftAndWrap(2) ^ ctrl.ShiftAndWrap(2)
            ^ alt.ShiftAndWrap(2) ^ meta.ShiftAndWrap(2);
}
