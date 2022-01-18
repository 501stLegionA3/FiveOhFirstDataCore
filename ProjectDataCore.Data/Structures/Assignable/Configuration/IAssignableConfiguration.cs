using ProjectDataCore.Data.Structures.Model.Assignable;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

public interface IAssignableConfiguration<T> : IAssignableConfiguration
{
    public List<T> AllowedValues { get; set; }
}

public interface IAssignableConfiguration
{
    public void MoveElement(int currentIndex, int newIndex);
    public void AddElement(object o);
    public void RemoveElement(int index);

    public List<string> GetDisplayValues()
    {
        List<string> itemList = new();
        switch (this)
        {
            case IAssignableConfiguration<DateTime> c:
                foreach (var item in c.AllowedValues)
                    itemList.Add(item.ToShortDateString() + " " + item.ToShortTimeString());
                break;
            case IAssignableConfiguration<DateOnly> c:
                foreach (var item in c.AllowedValues)
                    itemList.Add(item.ToShortDateString());
                break;
            case IAssignableConfiguration<TimeOnly> c:
                foreach (var item in c.AllowedValues)
                    itemList.Add(item.ToString("hh:mm:ss"));
                break;

            case IAssignableConfiguration<int> c:
                foreach (var item in c.AllowedValues)
                    itemList.Add(item.ToString());
                break;
            case IAssignableConfiguration<double> c:
                foreach (var item in c.AllowedValues)
                    itemList.Add(item.ToString());
                break;

            case IAssignableConfiguration<string> c:
                foreach (var item in c.AllowedValues)
                    itemList.Add(item);
                break;
        }

        return itemList;
    }

    public void AddItem(AssignableConfigurationValueEditModel valueEditModel)
	{
        switch (this)
        {
            case IAssignableConfiguration<DateTime> c:
                var dateTime = valueEditModel.DateValue;
                dateTime += valueEditModel.TimeValue;
                c.AddElement(dateTime);
                break;
            case IAssignableConfiguration<DateOnly> c:
                c.AddElement(DateOnly.FromDateTime(valueEditModel.DateValue));
                break;
            case IAssignableConfiguration<TimeOnly> c:
                c.AddElement(TimeOnly.FromTimeSpan(valueEditModel.TimeValue));
                break;

            case IAssignableConfiguration<int> c:
                c.AddElement(valueEditModel.IntValue);
                break;
            case IAssignableConfiguration<double> c:
                c.AddElement(valueEditModel.DoubleValue);
                break;

            case IAssignableConfiguration<string> c:
                c.AddElement(valueEditModel.StringValue);
                break;
        }
    }

    public (string, dynamic)? GetSingleValuePair(int valueIndex)
	{
        switch (this)
        {
            case IAssignableConfiguration<DateTime> c:
                var dateTime = c.AllowedValues[valueIndex];
                return (dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString(), dateTime);

            case IAssignableConfiguration<DateOnly> c:
                var dateItem = c.AllowedValues[valueIndex];
                return (dateItem.ToShortDateString(), dateItem);

            case IAssignableConfiguration<TimeOnly> c:
                var timeItem = c.AllowedValues[valueIndex];
                return (timeItem.ToString("hh:mm:ss"), timeItem);


            case IAssignableConfiguration<int> c:
                var intVal = c.AllowedValues[valueIndex];
                return (intVal.ToString(), intVal);

            case IAssignableConfiguration<double> c:
                var doubleValue = c.AllowedValues[valueIndex];
                return (doubleValue.ToString(), doubleValue);


            case IAssignableConfiguration<string> c:
                var stringVal = c.AllowedValues[valueIndex];
                return (stringVal, stringVal);
        }

        return null;
	}

    public (string, dynamic)? GetSingleValuePair(AssignableConfigurationValueEditModel valueEditModel)
	{
        switch (this)
        {
            case IAssignableConfiguration<DateTime>:
                var dateTime = valueEditModel.DateValue;
                dateTime += valueEditModel.TimeValue;
                return (dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString(), dateTime);

            case IAssignableConfiguration<DateOnly>:
                var dateItem = DateOnly.FromDateTime(valueEditModel.DateValue);
                return (dateItem.ToShortDateString(), dateItem);

            case IAssignableConfiguration<TimeOnly>:
                var timeItem = TimeOnly.FromTimeSpan(valueEditModel.TimeValue);
                return (timeItem.ToString("hh:mm:ss"), timeItem);


            case IAssignableConfiguration<int>:
                return (valueEditModel.IntValue.ToString(), valueEditModel.IntValue);

            case IAssignableConfiguration<double>:
                return (valueEditModel.DoubleValue.ToString(), valueEditModel.DoubleValue);


            case IAssignableConfiguration<string>:
                return (valueEditModel.StringValue, valueEditModel.StringValue);
        }

        return null;
    }
}