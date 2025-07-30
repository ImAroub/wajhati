using Microsoft.AspNetCore.Mvc;
using finalproject.Data;
using finalproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace finalproject.Namespace
{
    [Authorize]
    public class DashBoardController : Controller
    {
        // GET: DashBoardController

        private readonly AppDbContext _context;

        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            //ViewBag.username = User.Identity.IsAuthenticated ? User.Identity.Name : " ";
            return View();
        }

        public IActionResult Place()
        {
            ViewBag.PlacesList = _context.places.ToList();
            return View(new Place());

        }

        [HttpPost]
        public async Task<IActionResult> AddPlace(Place place)
        {
            if (ModelState.IsValid)
            {
                _context.places.Add(place);
                await _context.SaveChangesAsync();
                return RedirectToAction("Place");
            }
            if (!ModelState.IsValid)
{
    foreach (var error in ModelState)
    {
        Console.WriteLine($"[Model Error] {error.Key}:");

        foreach (var err in error.Value.Errors)
        {
            Console.WriteLine($"   - {err.ErrorMessage}");
        }
    }
}


            ViewBag.PlacesList = _context.places.ToList();
            return View("Place", place); // ✅ مش قائمة

        }

        public async Task<IActionResult> DeletePlace(int id)
        {
            var place = await _context.places.FindAsync(id);
            if (place != null)
            {
                _context.places.Remove(place);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Place");
        }

        public IActionResult EditPlace(int id)
        {
            var place = _context.places.Find(id);
            return View(place);
        }


        public async Task<IActionResult> UpdatePlace(Place place)
        {
            if (ModelState.IsValid)
            {
                _context.places.Update(place);
                await _context.SaveChangesAsync();
                return RedirectToAction("Place");
            }

            return View("EditPlace", place);
        }


        public IActionResult PlaceDetail()
        {

            var place = _context.places.ToList();
            ViewBag.placs = place;
            var placesdetails = _context.placesDetails.ToList();

            return View(placesdetails);
        }

        public async Task<IActionResult> AddPlaceDetail(PlaceDetail placesDetails,IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqeFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                var filePath = Path.Combine(uploadsFolder, uniqeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                placesDetails.ImageUrl = "/Uploads/" + uniqeFileName;
            }
            
                _context.placesDetails.Add(placesDetails);
                await _context.SaveChangesAsync();
            

            return RedirectToAction("PlaceDetail");
        }

        public IActionResult EditPlaceDetail(int id)
        {
            var place = _context.places.ToList();
            ViewBag.placs = place;

            var detail = _context.placesDetails.Find(id);
           if (detail == null)
                return NotFound();

            return View("EditPlaceDetail", detail);
        }

        [HttpPost]

        public async Task<IActionResult> UpdatePlaceDetail(PlaceDetail detail, IFormFile ImageFile)
        {
            if (ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqeFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqeFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                detail.ImageUrl = "/Uploads/" + uniqeFileName;
            }

            if (ModelState.IsValid)
            {
                _context.placesDetails.Update(detail);
                await _context.SaveChangesAsync();
                return RedirectToAction("PlaceDetail");
            }

            var place = _context.places.ToList();
            ViewBag.placs = place;
            return View("EditPlaceDetail", detail);
        }



        public async Task<IActionResult> DeletePlaceDetail(int id)
        {
            var detail = await _context.placesDetails.FindAsync(id);
            if (detail != null)
            {
                _context.placesDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("PlaceDetail");
        }




    }
}
