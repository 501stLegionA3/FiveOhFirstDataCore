using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Notice
{
    public enum NoticeLevel
    {
        [Description("Primary")]
        Primary,
        [Description("Secondary")]
        Secondary,
        [Description("Success")]
        Success,
        [Description("Info")]
        Info,
        [Description("Warning")]
        Warning,
        [Description("Danger")]
        Danger,
        [Description("Light")]
        Light,
        [Description("Dark")]
        Dark,
        [Description("Primary Outline")]
        PrimaryOutline,
        [Description("Secondary Outline")]
        SecondaryOutline,
        [Description("Success Outline")]
        SuccessOutline,
        [Description("Info Outline")]
        InfoOutline,
        [Description("Warning Outline")]
        WarningOutline,
        [Description("Danger Outline")]
        DangerOutline,
        [Description("Light Outline")]
        LightOutline,
        [Description("Dark Outline")]
        DarkOutline,
    }

    public static class NoticeLevelExtensions
    {
        public static string GetAlertClasses(this NoticeLevel level)
            => level switch
            {
                NoticeLevel.Primary => "alert alert-primary p-2",
                NoticeLevel.Secondary => "alert alert-secondary p-2",
                NoticeLevel.Success => "alert alert-success p-2",
                NoticeLevel.Info => "alert alert-info p-2",
                NoticeLevel.Warning => "alert alert-warning p-2",
                NoticeLevel.Danger => "alert alert-danger p-2",
                NoticeLevel.Light => "alert alert-light p-2",
                NoticeLevel.Dark => "alert alert-dark p-2",
                NoticeLevel.PrimaryOutline => "border border-primary p-2",
                NoticeLevel.SecondaryOutline => "border border-secondary p-2",
                NoticeLevel.SuccessOutline => "border border-success p-2",
                NoticeLevel.InfoOutline => "border border-info p-2",
                NoticeLevel.WarningOutline => "border border-warning p-2",
                NoticeLevel.DangerOutline => "border border-danger p-2",
                NoticeLevel.LightOutline => "border border-light p-2",
                NoticeLevel.DarkOutline => "border border-dark p-2",
                _ => "",
            };
    }
}
