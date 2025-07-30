using Microsoft.AspNetCore.Mvc;
using finalproject.Data;
using finalproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace finalproject.Namespace
{
    [Authorize]
    public class TripController : Controller
    {
        // GET: TripController

        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TripController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var cities = _context.places
                .Where(p => !string.IsNullOrEmpty(p.City))
                .Select(p => p.City)
                .Distinct()
                .ToList();

            ViewBag.Cities = cities;

            return View();
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

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of earth in km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlan(string city, int days)
        {
            var userId = _userManager.GetUserId(User);
            var random = new Random();

            // النسخ الأصلية الثابتة
            var allMainPlaces = _context.places
                .Where(p => p.City == city &&
                    (p.Type == "Entertainment" || p.Type == "Mall" || p.Type == "Historical" || p.Type == "Workshop"))
                .ToList();

            var allRestaurants = _context.places
                .Where(p => p.City == city && p.Type == "Restaurant")
                .ToList();

            var allCafes = _context.places
                .Where(p => p.City == city && p.Type == "Cafe")
                .ToList();

            // النسخ المتغيرة اللي راح نستهلك منها
            var mainPlaces = new List<Place>(allMainPlaces);
            var restaurants = new List<Place>(allRestaurants);
            var cafes = new List<Place>(allCafes);

            var tripPlan = new TripPlan
            {
                City = city,
                NumberOfDays = days,
                CreatedAt = DateTime.Now,
                UserId = userId,
                DailySchedules = new List<DaySchedule>()
            };

            for (int i = 1; i <= days; i++)
            {
                // إذا خلصت، نعيد تعبئة النسخ المتغيرة من الأصل
                if (mainPlaces.Count == 0) mainPlaces = new List<Place>(allMainPlaces);
                if (restaurants.Count == 0) restaurants = new List<Place>(allRestaurants);
                if (cafes.Count == 0) cafes = new List<Place>(allCafes);

                var mainPlace = mainPlaces.OrderBy(x => random.Next()).FirstOrDefault();
                var restaurant = restaurants.OrderBy(x => random.Next()).FirstOrDefault();
                var cafe = cafes.OrderBy(x => random.Next()).FirstOrDefault();

                // حذفهم من النسخ المتغيرة عشان ما يتكرروا خلال الدورة
                mainPlaces.Remove(mainPlace);
                restaurants.Remove(restaurant);
                cafes.Remove(cafe);

                var daySchedule = new DaySchedule
                {
                    DayNumber = i,
                    DaySchedulePlaces = new List<DaySchedulePlace>()
                };

                // 🟢 رئيسي
                if (mainPlace != null)
                {
                    _context.daySchedulePlaces.Add(new DaySchedulePlace
                    {
                        PlaceId = mainPlace.Id,
                        IsMainPlace = true,
                        OrderInDay = 1,
                        DaySchedule = daySchedule
                    });
                }

                int order = 2;

                // 🟠 مطعم
                if (restaurant != null)
                {
                    _context.daySchedulePlaces.Add(new DaySchedulePlace
                    {
                        PlaceId = restaurant.Id,
                        IsMainPlace = false,
                        OrderInDay = order++,
                        DaySchedule = daySchedule
                    });
                }

                // 🔵 كافيه
                if (cafe != null)
                {
                    _context.daySchedulePlaces.Add(new DaySchedulePlace
                    {
                        PlaceId = cafe.Id,
                        IsMainPlace = false,
                        OrderInDay = order++,
                        DaySchedule = daySchedule
                    });
                }

                tripPlan.DailySchedules.Add(daySchedule);
            }

            _context.tripPlans.Add(tripPlan);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewPlan", new { id = tripPlan.Id });
        }


        [Authorize]
        public IActionResult ViewPlan(int id)
        {
            var tripPlan = _context.tripPlans
             .Include(t => t.DailySchedules)
                 .ThenInclude(ds => ds.DaySchedulePlaces)
                     .ThenInclude(dp => dp.Place)
             .FirstOrDefault(t => t.Id == id);


            if (tripPlan == null)
                return NotFound();

            return View(tripPlan);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNewDay(int tripId)
        {
            var trip = _context.tripPlans
                .Include(t => t.DailySchedules)
                    .ThenInclude(ds => ds.DaySchedulePlaces)
                .FirstOrDefault(t => t.Id == tripId);

            if (trip == null) return NotFound();

            var city = trip.City;
            var random = new Random();

            // جميع الأماكن الأصلية في المدينة حسب النوع
            var allMainPlaces = _context.places
                .Where(p => p.City == city &&
                    (p.Type == "Entertainment" || p.Type == "Mall" || p.Type == "Historical" || p.Type == "Workshop"))
                .ToList();

            var allRestaurants = _context.places
                .Where(p => p.City == city && p.Type == "Restaurant").ToList();

            var allCafes = _context.places
                .Where(p => p.City == city && p.Type == "Cafe").ToList();

            // استخراج كل الأماكن المستخدمة سابقًا في الرحلة
            var usedPlaceIds = trip.DailySchedules
                .SelectMany(ds => ds.DaySchedulePlaces)
                .Select(p => p.PlaceId)
                .ToList();

            // إعداد النسخ المتاحة بعد إزالة الأماكن المستخدمة
            var availableMainPlaces = allMainPlaces.Where(p => !usedPlaceIds.Contains(p.Id)).ToList();
            var availableRestaurants = allRestaurants.Where(p => !usedPlaceIds.Contains(p.Id)).ToList();
            var availableCafes = allCafes.Where(p => !usedPlaceIds.Contains(p.Id)).ToList();

            // إذا ما فيه متاح، نرجع نكرر كامل القائمة (loop)
            if (!availableMainPlaces.Any()) availableMainPlaces = new List<Place>(allMainPlaces);
            if (!availableRestaurants.Any()) availableRestaurants = new List<Place>(allRestaurants);
            if (!availableCafes.Any()) availableCafes = new List<Place>(allCafes);

            // اختيار عشوائي
            var mainPlace = availableMainPlaces.OrderBy(x => random.Next()).FirstOrDefault();
            var restaurant = availableRestaurants.OrderBy(x => random.Next()).FirstOrDefault();
            var cafe = availableCafes.OrderBy(x => random.Next()).FirstOrDefault();

            var newDay = new DaySchedule
            {
                DayNumber = trip.DailySchedules.Count + 1,
                TripPlanId = tripId,
                DaySchedulePlaces = new List<DaySchedulePlace>()
            };

            if (mainPlace != null)
            {
                newDay.DaySchedulePlaces.Add(new DaySchedulePlace
                {
                    PlaceId = mainPlace.Id,
                    IsMainPlace = true,
                    OrderInDay = 1
                });
            }

            int order = 2;

            if (restaurant != null && restaurant.Id != mainPlace?.Id)
            {
                newDay.DaySchedulePlaces.Add(new DaySchedulePlace
                {
                    PlaceId = restaurant.Id,
                    IsMainPlace = false,
                    OrderInDay = order++
                });
            }

            if (cafe != null && cafe.Id != mainPlace?.Id && cafe.Id != restaurant?.Id)
            {
                newDay.DaySchedulePlaces.Add(new DaySchedulePlace
                {
                    PlaceId = cafe.Id,
                    IsMainPlace = false,
                    OrderInDay = order++
                });
            }

            _context.daySchedules.Add(newDay);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewPlan", new { id = tripId });
        }

        [Authorize]
        public async Task<IActionResult> DeleteDay(int id)
        {
            var day = await _context.daySchedules
                .Include(d => d.DaySchedulePlaces)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (day == null)
                return NotFound();

            // حذف الأماكن المرتبطة باليوم أولًا
            _context.daySchedulePlaces.RemoveRange(day.DaySchedulePlaces);
            _context.daySchedules.Remove(day);

            await _context.SaveChangesAsync();

            return RedirectToAction("ViewPlan", new { id = day.TripPlanId });
        }


        [HttpGet]
        [Authorize]
        public IActionResult EditDayContent(int id)
        {
            var day = _context.daySchedules
                .Include(d => d.DaySchedulePlaces)
                    .ThenInclude(dp => dp.Place)
                .Include(d => d.TripPlan)
                .FirstOrDefault(d => d.Id == id);

            if (day == null) return NotFound();

            var city = day.TripPlan.City;

            var availablePlaces = _context.places
                .Where(p => p.City == city)
                .ToList();

            var addedPlaceIds = day.DaySchedulePlaces.Select(dp => dp.PlaceId).ToList();

            var notAddedPlaces = availablePlaces
                .Where(p => !addedPlaceIds.Contains(p.Id))
                .ToList();

            ViewBag.AvailablePlaces = notAddedPlaces;

            return View(day);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPlaceToDay(int dayId, int placeId)
        {
            var day = _context.daySchedules
                .Include(d => d.DaySchedulePlaces)
                .FirstOrDefault(d => d.Id == dayId);

            if (day == null) return NotFound();

            var place = _context.places.FirstOrDefault(p => p.Id == placeId);
            if (place == null) return NotFound();

            // تصنيف تلقائي: فقط "مطعم" و"كافيه" تعتبر فرعية
            bool isMain = !(place.Type == "Restaurant" || place.Type == "Cafe");

            int order;

            if (isMain)
            {
                // نحسب آخر ترتيب للمكان الرئيسي
                var lastMain = day.DaySchedulePlaces
                    .Where(p => p.IsMainPlace)
                    .OrderByDescending(p => p.OrderInDay)
                    .FirstOrDefault();

                order = (lastMain?.OrderInDay ?? 0) + 1;
            }
            else
            {
                // نحسب آخر ترتيب للمكان الفرعي (بعد كل الرئيسي)
                var lastSub = day.DaySchedulePlaces
                    .Where(p => !p.IsMainPlace)
                    .OrderByDescending(p => p.OrderInDay)
                    .FirstOrDefault();

                order = (lastSub?.OrderInDay ?? day.DaySchedulePlaces.Count + 1);
            }

            var newEntry = new DaySchedulePlace
            {
                DayScheduleId = dayId,
                PlaceId = placeId,
                IsMainPlace = isMain,
                OrderInDay = order
            };

            _context.daySchedulePlaces.Add(newEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditDayContent", new { id = dayId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemovePlaceFromDay(int daySchedulePlaceId)
        {
            var placeEntry = await _context.daySchedulePlaces.FindAsync(daySchedulePlaceId);
            if (placeEntry == null) return NotFound();

            int dayId = placeEntry.DayScheduleId;

            _context.daySchedulePlaces.Remove(placeEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditDayContent", new { id = dayId });
        }

        [Authorize]
        public IActionResult MyTrips(string city)
        {
            var userId = _userManager.GetUserId(User);

            var trips = _context.tripPlans
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(city))
            {
                trips = trips.Where(t => t.City == city);
            }

            return View(trips.ToList());
        }


        [Authorize]
        public IActionResult TripDetails(int id)
        {
            var userId = _userManager.GetUserId(User);

            var trip = _context.tripPlans
                .Include(t => t.DailySchedules)
                    .ThenInclude(ds => ds.DaySchedulePlaces)
                        .ThenInclude(dp => dp.Place)
                .FirstOrDefault(t => t.Id == id && t.UserId == userId);

            if (trip == null)
                return NotFound();

            return View(trip);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SaveTrip(int tripId)
        {
            // مستقبلاً ممكن نضيف منطق هنا إذا فيه شيء إضافي للحفظ

            return RedirectToAction("MyTrips");
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var trip = await _context.tripPlans
                .Include(t => t.DailySchedules)
                    .ThenInclude(ds => ds.DaySchedulePlaces)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound();

            // حذف جميع الأماكن
            var allPlaces = trip.DailySchedules.SelectMany(d => d.DaySchedulePlaces).ToList();
            _context.daySchedulePlaces.RemoveRange(allPlaces);

            // حذف جميع الأيام
            _context.daySchedules.RemoveRange(trip.DailySchedules);

            // حذف الخطة
            _context.tripPlans.Remove(trip);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyTrips");
        }

    }
}