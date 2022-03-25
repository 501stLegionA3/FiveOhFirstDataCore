﻿using ProjectDataCore.Data.Structures.Page;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDataCore.Data.Structures.Nav;

/// <summary>
/// Used to represent modules for the top navigation bar
/// </summary>
public class NavModule : DataObject<Guid>
{
    public string DisplayName { get; set; }
    public string Href { get; set; } = "";
    public List<NavModule> SubModules { get; set; } = new();
    public bool HasMainPage { get; set; } = false;
    public NavModule? Parent { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? PageId { get; set; }
    public Guid? AuthKey { get; set; }
    public bool IsAdmin { get; set; } = false;

    public NavModule()
    {
        DisplayName = "";
        SubModules = new List<NavModule>();
    }

    public NavModule(Guid id)
    {
        ParentId = id;
        DisplayName = "";
        SubModules = new List<NavModule>();
    }

    public NavModule(string displayName, string href, bool hasMainPage = false)
    {
        DisplayName = displayName;
        Href = href;
        HasMainPage = hasMainPage;
    }

    public NavModule(string displayName, string href, List<NavModule> subModules, bool hasMainPage = false)
    {
        DisplayName = displayName;
        Href = href;
        SubModules = subModules;
        HasMainPage = hasMainPage;
    }

}