using CAMPUS.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CAMPUS.PL.Models;

// Base class for the New request and Edit pages: the form and its two dropdowns.
public abstract class RequestFormPageModel : PageModel
{
    private readonly ILookupService _lookups;

    protected RequestFormPageModel(ILookupService lookups)
    {
        _lookups = lookups;
    }

    // The form fields. BindProperty fills it from the posted form.
    [BindProperty]
    public RequestInputModel Input { get; set; } = new();

    public List<SelectListItem> Categories { get; private set; } = new();

    public List<SelectListItem> Campuses { get; private set; } = new();

    // Fills both dropdowns. Call it before every return Page(), or they're empty after an error.
    protected async Task LoadListsAsync()
    {
        var categories = await _lookups.GetCategoriesAsync();
        Categories = categories
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();

        var campuses = await _lookups.GetCampusesAsync();
        Campuses = campuses
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
    }
}