using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazorSqlite.Components.Account.Shared;

/// <content>   A grid. </content>
[CascadingTypeParameter(nameof(TItem))]
public partial class Grid<TItem>
{
    /// <summary>   (Immutable) name of the component. </summary>
    public const string ComponentName = "grid-with-sidecar-component";

    /// <summary>   (Immutable) the data tag rendered. </summary>
    public const string DataTagRendered = "rendered";

    /// <summary>   Values that represent search box location enums. </summary>
    public enum SearchBoxLocationEnum
    {
        /// <summary>   An enum constant representing the start option. </summary>
        Start,

        /// <summary>   An enum constant representing the end option. </summary>
        End
    }

    /// <summary>   Gets or sets the additional CSS. </summary>
    /// <value> The additional CSS. </value>
    [Parameter]
    public string AdditionalCss { get; set; } = string.Empty;

    /// <summary>   Gets or sets a value indicating whether the automatic generate columns. </summary>
    /// <value> True if automatic generate columns, false if not. </value>
    [Parameter]
    public bool AutoGenerateColumns { get; set; } = false;

    /// <summary>   Gets the number of badges. </summary>
    /// <value> The number of badges. </value>
    public string BadgeCount => $"{Items.Count} {(itemsNotFiltered.Any() ? $"of {itemsNotFiltered.Count()}" : "")}";

    /// <summary>   Gets or sets the columns. </summary>
    /// <value> The columns. </value>
    [Parameter]
    public RenderFragment? Columns { get; set; }

    /// <summary>   Gets or sets a value indicating whether the column selection is enabled. </summary>
    /// <value> True if column selection enabled, false if not. </value>
    [Parameter]
    public bool ColumnSelectionEnabled { get; set; } = false;

    /// <summary>   Gets or sets the commands template. </summary>
    /// <value> The commands template. </value>
    [Parameter]
    public RenderFragment? CommandsTemplate { get; set; }

    /// <summary>   Gets or sets the commands template start. </summary>
    /// <value> The commands template start. </value>
    [Parameter]
    public RenderFragment? CommandsTemplateStart { get; set; }

    /// <summary>   Gets or sets a value indicating whether the text wrapping is disabled. </summary>
    /// <value> True if disable text wrapping, false if not. </value>
    [Parameter]
    public bool DisableTextWrapping { get; set; } = false;

    /// <summary>   Gets or sets the ellipse template. </summary>
    /// <value> The ellipse template. </value>
    [Parameter]
    public RenderFragment? EllipseTemplate { get; set; }

    /// <summary>   Gets or sets the empty data template. </summary>
    /// <value> The empty data template. </value>
    [Parameter]
    public RenderFragment? EmptyDataTemplate { get; set; }

    /// <summary>   Gets or sets a value indicating whether the filter is enabled. </summary>
    /// <value> True if filter enabled, false if not. </value>
    [Parameter]
    public bool FilterEnabled { get; set; } = true;

    /// <summary>   Gets or sets the filter item predicate. </summary>
    /// <value> The filter item predicate. </value>
    [Parameter]
    public Func<TItem, string, bool>? FilterItemPredicate { get; set; }

    /// <summary>   Gets or sets the footer model. </summary>
    /// <value> The footer model. </value>
    [Parameter]
    public object? FooterModel { get; set; }

    /// <summary>   Gets or sets the get items. </summary>
    /// <value> The get items. </value>
    [Parameter]
    public Func<Task<List<TItem>>>? GetItems { get; set; }

    /// <summary>   Gets or sets the grid content navigation mode. </summary>
    /// <value> The grid content navigation mode. </value>
    [Parameter]
    public GridContentNavigationMode GridContentNavigationMode { get; set; } = GridContentNavigationMode.Pagination;

    /// <summary>   Gets or sets the initial render complete. </summary>
    /// <value> The initial render complete. </value>
    [Parameter]
    public EventCallback InitialRenderComplete { get; set; }

    /// <summary>   Gets or sets the item count changed. </summary>
    /// <value> The item count changed. </value>
    [Parameter]
    public EventCallback<int> ItemCountChanged { get; set; }

    /// <summary>   Gets or sets the item row CSS class selector. </summary>
    /// <value> A function delegate that yields a string. </value>
    [Parameter]
    public Func<TItem, string> ItemRowCssClassSelector { get; set; } = (item) => string.Empty;

    /// <summary>   Gets the items. </summary>
    /// <value> The items. </value>
    public List<TItem> Items { get; protected set; } = new();

    /// <summary>   Gets or sets the load more template. </summary>
    /// <value> The load more template. </value>
    [Parameter]
    public RenderFragment<GridLoadMoreTemplateContext>? LoadMoreTemplate { get; set; }

    /// <summary>   Gets or sets a value indicating whether the multi selection is enabled. </summary>
    /// <value> True if multi selection enabled, false if not. </value>
    [Parameter]
    public bool MultiSelectionEnabled { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the only show load more at bottom of
    ///             scroll. </summary>
    /// <value> True if only show load more at bottom of scroll, false if not. </value>
    [Parameter]
    public bool OnlyShowLoadMoreAtBottomOfScroll { get; set; } = false;

    /// <summary>   Gets or sets a value indicating whether the pad side car. </summary>
    /// <value> True if pad side car, false if not. </value>
    [Parameter]
    public bool PadSideCar { get; set; } = true;

    /// <summary>   Gets or sets the page size. </summary>
    /// <value> The size of the page. </value>
    [Parameter]
    public int PageSize { get; set; } = 0;

    /// <summary>   Gets or sets the size of the search box input. </summary>
    /// <value> The size of the search box input. </value>
    [Parameter]
    public InputSize SearchBoxInputSize { get; set; } = InputSize.Regular;

    /// <summary>   Gets or sets the search box location. </summary>
    /// <value> The search box location. </value>
    [Parameter]
    public SearchBoxLocationEnum SearchBoxLocation { get; set; } = SearchBoxLocationEnum.Start;

    /// <summary>   Gets or sets a value indicating whether the search is enabled. </summary>
    /// <value> True if search enabled, false if not. </value>
    [Parameter]
    public bool SearchEnabled { get; set; } = true;

    /// <summary>   Gets or sets the search text. </summary>
    /// <value> The search text. </value>
    public string SearchText
    {
        get
        {
            return searchText;
        }
        set
        {
            //TODO: this should be imporved upon to not use this pattern of property setting, but an async pattern of on changed
            searchText = value;
            SearchSync(value);
        }
    }

    /// <summary>   Gets or sets the selected item. </summary>
    /// <value> The selected item. </value>
    public TItem? SelectedItem { get; set; } = default;

    /// <summary>   Gets or sets the selected item changed. </summary>
    /// <value> The selected item changed. </value>
    [Parameter]
    public EventCallback<TItem?> SelectedItemChanged { get; set; }

    /// <summary>   Gets or sets the selected items. </summary>
    /// <value> The selected items. </value>
    public HashSet<TItem>? SelectedItems { get; set; } = new();

    /// <summary>   Gets or sets the selected items changed. </summary>
    /// <value> The selected items changed. </value>
    [Parameter]
    public EventCallback<HashSet<TItem>?> SelectedItemsChanged { get; set; }

    /// <summary>   Gets or sets a value indicating whether the selection is enabled. </summary>
    /// <value> True if selection enabled, false if not. </value>
    [Parameter]
    public bool SelectionEnabled { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the badge count is shown. </summary>
    /// <value> True if show badge count, false if not. </value>
    [Parameter]
    public bool ShowBadgeCount { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the border is shown. </summary>
    /// <value> True if show border, false if not. </value>
    [Parameter]
    public bool ShowBorder { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the card header is shown. </summary>
    /// <value> True if show card header, false if not. </value>
    [Parameter]
    public bool ShowCardHeader { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the grid title is shown. </summary>
    /// <value> True if show grid title, false if not. </value>
    [Parameter]
    public bool ShowGridTitle { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the header is shown. </summary>
    /// <value> True if show header, false if not. </value>
    [Parameter]
    public bool ShowHeader { get; set; } = true;

    /// <summary>   Gets or sets a value indicating whether the refresh button is shown. </summary>
    /// <value> True if show refresh button, false if not. </value>
    [Parameter]
    public bool ShowRefreshButton { get; set; } = true;

    /// <summary>   Gets or sets the side car detail. </summary>
    /// <value> The side car detail. </value>
    [Parameter]
    public RenderFragment? SideCarDetail { get; set; }

    /// <summary>   Gets or sets a value indicating whether the side car is visible. </summary>
    /// <value> True if side car visible, false if not. </value>
    [Parameter]
    public bool SideCarEnabled { get; set; } = true;

    /// <summary>   Gets or sets the side car header. </summary>
    /// <value> The side car header. </value>
    [Parameter]
    public RenderFragment? SideCarHeader { get; set; }

    /// <summary>   Gets or sets the title. </summary>
    /// <value> The title. </value>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>   Gets or sets the grid. </summary>
    /// <value> The grid. </value>
    protected HxGrid<TItem>? _grid { get; set; }

    /// <summary>   Gets or sets the items not filtered. </summary>
    /// <value> The items not filtered. </value>
    protected List<TItem> itemsNotFiltered { get; set; } = new();

    /// <summary>   Gets or sets the number of previous items. </summary>
    /// <value> The number of previous items. </value>
    private int _prevItemCount { get; set; } = 0;

    /// <summary>   Gets the button size. </summary>
    /// <value> The size of the button. </value>
    private ButtonSize buttonSize
    {
        get
        {
            return SearchBoxInputSize switch
            {
                InputSize.Large => ButtonSize.Large,
                InputSize.Regular => ButtonSize.Regular,
                InputSize.Small => ButtonSize.Small,
                _ => ButtonSize.Regular
            };
        }
    }

    /// <summary>   Gets or sets the chips. </summary>
    /// <value> The chips. </value>
    private List<FilterChip> chips { get; set; } = new();

    /// <summary>   Gets or sets the div container attributes. </summary>
    /// <value> The div container attributes. </value>
    private Dictionary<string, object> divContainerAttributes { get; set; } = new();

    /// <summary>   Gets or sets a context for the filter. </summary>
    /// <value> The filter context. </value>
    private TItem? FilterContext { get; set; }

    /// <summary>   Gets or sets the filters. </summary>
    /// <value> The filters. </value>
    private List<FilterRecord> filters { get; set; } = new();

    /// <summary>   Gets or sets the offcanvas column select. </summary>
    /// <value> The offcanvas column select. </value>
    private HxOffcanvas? offcanvasColumnSelect { get; set; }

    /// <summary>   Gets or sets the offcanvas filter. </summary>
    /// <value> The offcanvas filter. </value>
    private HxOffcanvas? offcanvasFilter { get; set; }

    /// <summary>   Gets or sets the search text. </summary>
    /// <value> The search text. </value>
    private string searchText { get; set; } = string.Empty;


    private TItem? InitFilterContext()
    {
        return typeof(TItem).GetConstructor(Type.EmptyTypes) != null ? (TItem?)Activator.CreateInstance(typeof(TItem)) : default;
    }


    /// <summary>   Clears the selected items. </summary>
    public void ClearSelectedItems()
    {
        SelectedItem = default;
        SelectedItems = null;
    }

    /// <summary>   Hides the filter. </summary>
    /// <returns>   A Task. </returns>
    public Task HideFilter()
    {
        return offcanvasFilter.HideNullSafe();
    }

    /// <summary>   Gets the refresh. </summary>
    /// <returns>   A Task. </returns>
    public async Task Refresh()
    {
        itemsNotFiltered.Clear(); //clear any cached / filtered items
        await GetItemsBase();
        FilterItemsBasedOn(searchText);
        await _grid.RefreshGridNullSafe();
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>   Searches for the first match for the given string. </summary>
    /// <param name="value">    The value. </param>
    /// <returns>   A Task. </returns>
    public Task Search(string value)
    {
        FilterItemsBasedOn(value);
        return _grid.RefreshGridNullSafe();
    }

    /// <summary>   Select first. </summary>
    /// <returns>   A Task. </returns>
    public Task SelectFirst()
    {
        return HandleSelectedDataItemChanged(Items.FirstOrDefault());
    }

    /// <summary>   Select item. </summary>
    /// <param name="itemSelector"> The item selector. </param>
    /// <returns>   A Task. </returns>
    public Task SelectItem(Func<TItem, bool> itemSelector)
    {
        return HandleSelectedDataItemChanged(Items.FirstOrDefault(itemSelector));
    }

    /// <summary>   Select items. </summary>
    /// <param name="itemSelector"> The item selector. </param>
    /// <returns>   A Task. </returns>
    public Task SelectItems(Func<TItem, bool> itemSelector)
    {
        return HandleSelectedDataItemsChanged(Items.Where(itemSelector).ToHashSet());
    }

    /// <summary>   Sets the items. </summary>
    /// <param name="items">    The items. </param>
    /// <returns>   A Task. </returns>
    public async Task SetItems(List<TItem> items)
    {
        Items = items;

        await _grid.RefreshGridNullSafe();

        await InvokeAsync(StateHasChanged);
    }

    /// <summary>   Shows the filter. </summary>
    /// <returns>   A Task. </returns>
    public Task ShowFilter()
    {
        FilterContext = InitFilterContext();
        return offcanvasFilter.ShowNullSafe();
    }

    /// <summary>   Filter items based on. </summary>
    /// <param name="value">    The value. </param>
    protected void FilterItemsBasedOn(string value)
    {
        RestoreUnfilteredItems();

        // If there is a new search value, then filter the list.
        if (!string.IsNullOrEmpty(value))
        {
            itemsNotFiltered = new List<TItem>(Items);

            // Filter the Items based on the search text.
            Items = FilterItems(value);
        }
    }

    /// <summary>   Gets grid data. </summary>
    /// <param name="request">  The request. </param>
    /// <returns>   The grid data. </returns>
    protected async Task<GridDataProviderResult<TItem>> GetGridData(GridDataProviderRequest<TItem> request)
    {
        //update selecteditem
        SelectedItem = Items.FirstOrDefault(_ => SelectedItem?.Equals(_) ?? false);

        //update selecteditems
        SelectedItems = SelectedItems?
            .Where(item => Items.Any(_ => item?.Equals(_) ?? false))
            .Select(item => Items.First(_ => item?.Equals(_) ?? false))
            .ToHashSet();

        //since we are not doing server side paging/sorting and we have the entire result set here we can you request.applyTo to
        // have the grid auto page / sort
        return await Task.FromResult(request.ApplyTo(Items));
    }

    /// <summary>   Gets items base. </summary>
    /// <returns>   A Task. </returns>
    protected async Task GetItemsBase()
    {
        //reset the Items collection, can't simply clear because it is possibel its bound to a parent collection
        Items = new();

        if (GetItems != null)
            Items = await GetItems();

        if (_prevItemCount != Items.Count())
        {
            _prevItemCount = Items.Count();
            await ItemCountChanged.InvokeAsync(_prevItemCount);
        }
    }

    /// <summary>   Handles the selected data item changed described by item. </summary>
    /// <param name="item"> The item. </param>
    /// <returns>   A Task. </returns>
    protected async Task HandleSelectedDataItemChanged(TItem? item)
    {
        // if the selected item is null the grid could have been refreshed, but we want
        // to keep the previously selected item so just return
        if (item == null) return;

        //this selects the check box for the row selected
        SelectedItems = new() { item };

        SelectedItem = item;

        await InvokeAsync(StateHasChanged);
        await SelectedItemChanged.InvokeAsync(SelectedItem);
    }

    /// <summary>   Handles the selected data items changed described by items. </summary>
    /// <param name="items">    The items. </param>
    /// <returns>   A Task. </returns>
    protected async Task HandleSelectedDataItemsChanged(HashSet<TItem>? items)
    {
        // if the selected item is null the grid could have been refreshed, but we want
        // to keep the previously selected item so just return
        if (items == null) return;

        SelectedItems = items;

        if (SelectedItems.Count == 0 || SelectedItems.Count > 1)
        {
            //this undoes the selected connection if the user has selected more then one checkbox
            SelectedItem = default;
        }
        else if (SelectedItems.Count == 1)
        {
            SelectedItem = SelectedItems.First();
        }
        await SelectedItemsChanged.InvokeAsync(SelectedItems);
        await SelectedItemChanged.InvokeAsync(SelectedItem);
    }

    /// <summary>   Method invoked after each time the component has been rendered. Note that the
    ///             component does not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />,
    ///             because that would cause an infinite render loop. </summary>
    /// <remarks>   The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" />
    ///             and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" />
    ///             lifecycle methods are useful for performing interop, or interacting with values
    ///             received from <c>@ref</c>. Use the <paramref name="firstRender" /> parameter to
    ///             ensure that initialization work is only performed once. </remarks>
    /// <param name="firstRender">  Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" />
    ///                             has been invoked on this component instance; otherwise <c>false</c>.</param>
    /// <returns>   A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous
    ///             operation. </returns>

    /// <summary>   Gets chip render fragment. </summary>
    /// <param name="label">    The label. </param>
    /// <param name="value">    The value. </param>
    /// <returns>   The chip render fragment. </returns>
    private static RenderFragment GetChipRenderFragment(string label, string value) => builder =>
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "hx-chip-list-label");
        builder.AddContent(2, label);
        builder.AddContent(3, ": ");
        builder.CloseElement();
        builder.AddContent(4, value);
    };

    /// <summary>   Any string property contains. </summary>
    /// <param name="obj">      The object. </param>
    /// <param name="value">    The value. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    private bool AnyStringPropertyContains(TItem obj, string value)
    {
        if (obj is Dictionary<string, string> dictionary)
        {
            foreach (var v in dictionary.Values)
            {
                if (v.Contains(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (PropertyInfo property in typeof(TItem).GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    var propertyValue = property.GetValue(obj) as string;
                    if (propertyValue != null && propertyValue.Contains(value, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>   Filter items. </summary>
    /// <param name="value">    The value. </param>
    /// <returns>   A List&lt;TItem&gt; </returns>
    private List<TItem> FilterItems(string value)
    {
        //TODO: we can make this smarter to look at property decorations to determine which properties to search
        // this would require auto generating the columns and then looking at the attributes on the properties
        // and taking into account what columns are visible

        if (FilterItemPredicate != null)
        {
            return Items.Where(item => FilterItemPredicate.Invoke(item, value)).ToList();
        }

        return Items.Where(item =>
            item != null &&
            AnyStringPropertyContains(item, value)
        ).ToList();
    }

    /// <summary>   Handles the remove chip described by chipToRemove. </summary>
    /// <param name="chipToRemove"> The chip to remove. </param>
    /// <returns>   A Task. </returns>
    private async Task HandleRemoveChip(ChipItem chipToRemove)
    {
        if (chipToRemove is FilterChip)
        {
            chips.Remove((FilterChip)chipToRemove);
            chipToRemove.RemoveAction((FilterChip)chipToRemove);

            await Refresh();
        }
    }

    /// <summary>   Removes the filter described by filterToRemove. </summary>
    /// <param name="filterToRemove">   The filter to remove. </param>
    private void RemoveFilter(object filterToRemove)
    {
        if (filterToRemove != null && filterToRemove is FilterChip)
        {
            filters.RemoveAll(_ => _.Id == ((FilterChip)filterToRemove).Id);
        }
    }

    /// <summary>   Restore unfiltered items. </summary>
    private void RestoreUnfilteredItems()
    {
        if (itemsNotFiltered.Any())
        {
            Items = new List<TItem>(itemsNotFiltered);
            itemsNotFiltered.Clear();
        }
    }

    /// <summary>   Searches for the first synchronize. </summary>
    /// <param name="value">    The value. </param>
    private void SearchSync(string value)
    {
        FilterItemsBasedOn(value);
        _ = _grid.RefreshGridNullSafe(); // Fire-and-forget
    }

    /// <summary>   Encapsulates the result of a difference. </summary>
    public class DifferenceResult
    {
        /// <summary>   Gets or sets the name of the display. </summary>
        /// <value> The name of the display. </value>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>   Gets or sets the object 1 value. </summary>
        /// <value> The object 1 value. </value>
        public object? Object1Value { get; set; }

        /// <summary>   Gets or sets the object 2 value. </summary>
        /// <value> The object 2 value. </value>
        public object? Object2Value { get; set; }

        /// <summary>   Gets or sets the name of the property. </summary>
        /// <value> The name of the property. </value>
        public string PropertyName { get; set; } = string.Empty;
    }

    /// <summary>   A filter chip. </summary>
    private class FilterChip : ChipItem
    {
        /// <summary>   The identifier. </summary>
        public Guid Id = Guid.NewGuid();
    }

    /// <summary>   Information about the filter. </summary>
    private class FilterRecord
    {
        /// <summary>   Initializes a new instance of the <see cref="FilterRecord"/> class. </summary>
        /// <param name="id">       The identifier. </param>
        /// <param name="prop">     The property. </param>
        /// <param name="value">    The value. </param>
        public FilterRecord(Guid id, string prop, object value)
        {
            Id = id;
            this.prop = prop;
            this.value = value;
        }

        /// <summary>   Gets or sets the identifier. </summary>
        /// <value> The identifier. </value>
        public Guid Id { get; set; }

        /// <summary>   Gets or sets the property. </summary>
        /// <value> The property. </value>
        public string prop { get; set; }

        /// <summary>   Gets or sets the value. </summary>
        /// <value> The value. </value>
        public object value { get; set; }
    }

    public class GridPropertyColumn
    {
        /// <summary>   Gets or sets a value indicating whether this object is visible. </summary>
        /// <value> True if this object is visible, false if not. </value>
        public bool IsVisible { get; set; }

        /// <summary>   Gets or sets the property. </summary>
        /// <value> The property. </value>
        public PropertyInfo? Property { get; set; }
    }



    protected record FilterModelDto { }
}
public static class NullHelperExtensions
{
    /// <summary>   A HxModal? extension method that hides the modal null safe. </summary>
    /// <param name="hxModal">  The hxModal to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task HideModalNullSafe(this HxModal? hxModal)
    {
        return (hxModal?.HideAsync() ?? Task.CompletedTask);
    }

    /// <summary>   A HxCollapse? extension method that hides the null safe. </summary>
    /// <param name="hxCollapse">   The hxCollapse to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task HideNullSafe(this HxCollapse? hxCollapse)
    {
        return (hxCollapse?.HideAsync() ?? Task.CompletedTask);
    }

    /// <summary>   A HxCollapse? extension method that hides the null safe. </summary>
    /// <param name="hxOffcanvas">  The hxOffcanvas to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task HideNullSafe(this HxOffcanvas? hxOffcanvas)
    {
        return (hxOffcanvas?.HideAsync() ?? Task.CompletedTask);
    }

    /// <summary>   A HxGrid&lt;TReturn&gt;? extension method that refresh grid null safe. </summary>
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="hxGrid">   The hxGrid to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task RefreshGridNullSafe<T>(this HxGrid<T>? hxGrid)
    {
        return (hxGrid?.RefreshDataAsync() ?? Task.CompletedTask);
    }

    /// <summary>   A HxModal? extension method that shows the modal null safe. </summary>
    /// <param name="hxModal">  The hxModal to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task ShowModalNullSafe(this HxModal? hxModal)
    {
        return (hxModal?.ShowAsync() ?? Task.CompletedTask);
    }

    /// <summary>   A HxCollapse? extension method that shows the null safe. </summary>
    /// <param name="hxCollapse">   The hxCollapse to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task ShowNullSafe(this HxCollapse? hxCollapse)
    {
        return (hxCollapse?.ShowAsync() ?? Task.CompletedTask);
    }

    /// <summary>   A HxCollapse? extension method that shows the null safe. </summary>
    /// <param name="hxOffcanvas">  The hxOffcanvas to act on. </param>
    /// <returns>   A Task. </returns>
    public static Task ShowNullSafe(this HxOffcanvas? hxOffcanvas)
    {
        return (hxOffcanvas?.ShowAsync() ?? Task.CompletedTask);
    }
}