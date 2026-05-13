namespace MaphunziroBlackboard.Web.Models;

/// <summary>
/// Used by placeholder module pages until full CRUD is wired for each area.
/// </summary>
public class ModuleWorkspaceViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "fa-layer-group";
    public IReadOnlyList<string> Highlights { get; set; } = Array.Empty<string>();

    public static ModuleWorkspaceViewModel For(string title, string description, string icon = "fa-folder-open",
        params string[] highlights) =>
        new()
        {
            Title = title,
            Description = description,
            Icon = icon,
            Highlights = highlights.Length > 0 ? highlights : Array.Empty<string>()
        };
}
