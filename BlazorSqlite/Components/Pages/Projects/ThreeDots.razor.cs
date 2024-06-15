using Microsoft.AspNetCore.Components;

namespace BlazorSqlite.Components.Pages.Projects;

public partial class ThreeDots
{
	[Parameter]
	public RenderFragment? MenuItems { get; set; }
	[Parameter]
	public string? CssClass { get; set; }
}
