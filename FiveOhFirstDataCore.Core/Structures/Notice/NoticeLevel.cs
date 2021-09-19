using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Structures.Notice
{
    public enum NoticeLevel
    {
        [Description("Primary")]
        PrimaryOutline,
        [Description("Secondary")]
        SecondaryOutline,
        [Description("Success")]
        SuccessOutline,
        [Description("Info")]
        InfoOutline,
        [Description("Warning")]
        WarningOutline,
        [Description("Danger")]
        DangerOutline,
        [Description("Light")]
        LightOutline,
        [Description("Dark")]
        DarkOutline,
    }

    public enum AlertLevel
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
    }

    public static class NoticeLevelExtensions
    {
        public static string GetNoticeClasses(this NoticeLevel level)
            => level switch
            {
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

        public static string GetAlertClasses(this AlertLevel level)
            => level switch
            {
                AlertLevel.Primary => "alert alert-primary",
                AlertLevel.Secondary => "alert alert-secondary",
                AlertLevel.Success => "alert alert-success",
                AlertLevel.Info => "alert alert-info",
                AlertLevel.Warning => "alert alert-warning",
                AlertLevel.Danger => "alert alert-danger",
                AlertLevel.Light => "alert alert-light",
                AlertLevel.Dark => "alert alert-dark",
                _ => "",
            };
    }
}
