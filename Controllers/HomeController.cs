using Microsoft.AspNetCore.Mvc;
using finalproject.Data;
using finalproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    public HomeController(ILogger<HomeController> logger,AppDbContext context)
    {
        _logger = logger;
         _context = context;
    }
    

    public IActionResult Index()
    {
        var places = _context.places
            .Include(p => p.Detail)
            .Where(p => p.Detail != null && !string.IsNullOrEmpty(p.Detail.ImageUrl))
            .Take(8)
            .ToList();

        return View(places);
    }

    public IActionResult Places(string search, string city, string type)
    {
        var placesQuery = _context.places.Include(p => p.Detail).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            placesQuery = placesQuery.Where(p => p.Name.Contains(search));
            ViewBag.SearchTerm = search;
        }

        if (!string.IsNullOrEmpty(city))
        {
            placesQuery = placesQuery.Where(p => p.City == city);
            ViewBag.SelectedCity = city;
        }

        if (!string.IsNullOrEmpty(type))
        {
            placesQuery = placesQuery.Where(p => p.Type == type);
            ViewBag.SelectedType = type;
        }

        var filteredPlaces = placesQuery.ToList();

        var cities = _context.places
            .Where(p => !string.IsNullOrEmpty(p.City))
            .Select(p => p.City)
            .Distinct()
            .ToList();

        var types = _context.places
            .Where(p => !string.IsNullOrEmpty(p.Type))
            .Select(p => p.Type)
            .Distinct()
            .ToList();

        ViewBag.Cities = cities;
        ViewBag.Types = types;

        return View(filteredPlaces);
    }



    public IActionResult PlaceDetail(int id)
    {
        var place = _context.places
            .Include(p => p.Detail)
            .FirstOrDefault(p => p.Id == id);

        if (place == null)
            return NotFound();

        return View(place);
    }
    

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
