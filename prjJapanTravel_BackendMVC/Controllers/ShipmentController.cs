﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjJapanTravel_BackendMVC.Models;
using prjJapanTravel_BackendMVC.ViewModels;
using prjJapanTravel_BackendMVC.ViewModels.ShipmentViewModels;

namespace prjJapanTravel_BackendMVC.Controllers
{
    public class ShipmentController : Controller
    {
        private readonly JapanTravelContext _context;

        public ShipmentController(JapanTravelContext context)
        {
            _context = context;
        }

        //---------------------首頁------------------------------------------
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewData["CurrentFilter"] = searchTerm;

            var query = _context.Routes
                .Include(r => r.OriginPort)
                .Include(r => r.DestinationPort)
                .Select(r => new ShipmentListViewModel
                {
                    RouteId = r.RouteId,
                    OriginPortName = r.OriginPort.PortName,
                    DestinationPortName = r.DestinationPort.PortName,
                    Price = r.Price,
                    RouteDescription = r.RouteDescription
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.OriginPortName.Contains(searchTerm) || r.DestinationPortName.Contains(searchTerm));
            }

            var shipments = await query.ToListAsync();

            return View(shipments);
        }


        //---------------------Create------------------------------------------
        public async Task<IActionResult> Create()
        {
            var ports = await _context.Ports.ToListAsync(); // 取得所有港口以填充下拉選單
            ViewBag.Ports = ports;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Route route)
        {
            if (ModelState.IsValid)
            {
                _context.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var ports = await _context.Ports.ToListAsync(); // 取得所有港口以填充下拉選單
            ViewBag.Ports = ports;
            return View(route);
        }

        //---------------------Edit------------------------------------------
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var route = await _context.Routes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            var ports = await _context.Ports.ToListAsync(); // 取得所有港口以填充下拉選單
            ViewBag.Ports = ports;
            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Route route)
        {
            if (id != route.RouteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(route);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteExists(route.RouteId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var ports = await _context.Ports.ToListAsync(); // 取得所有港口以填充下拉選單
            ViewBag.Ports = ports;
            return View(route);
        }

        private bool RouteExists(int id)
        {
            return _context.Routes.Any(e => e.RouteId == id);
        }

        //---------------------Delete------------------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var route = await _context.Routes.FindAsync(id);
            if (route != null)
            {
                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //---------------------Detail------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            ViewBag.RouteId = id;

            var route = await _context.Routes
                .Include(r => r.OriginPort)
                .Include(r => r.DestinationPort)
                .Include(r => r.Schedules) // Include Schedule data
                .Include(r => r.RouteImages) // Include RouteImage data
                .FirstOrDefaultAsync(m => m.RouteId == id);

            if (route == null)
            {
                return NotFound();
            }

            var model = new RouteDetailViewModel
            {
                RouteId = route.RouteId,
                OriginPortName = route.OriginPort.PortName,
                DestinationPortName = route.DestinationPort.PortName,
                RouteDescription = route.RouteDescription,
                Price = route.Price,
                Schedules = route.Schedules.ToList(),
                RouteImages = route.RouteImages.ToList()
            };

            return View(model);
        }


        //-----------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult CreateRouteImage(int routeId)
        {
            ViewBag.RouteId = routeId;
            return View();
        }

        // 提交新圖片 (Create - POST)
        [HttpPost]
        public IActionResult CreateRouteImage(RouteImage routeImage, IFormFile RouteImageUrl)
        {
            // 檢查 RouteId 是否正確
            if (routeImage.RouteId <= 0)
            {
                ModelState.AddModelError("RouteId", "RouteId is required.");
            }

            // 檢查上傳的圖片是否存在且有內容
            if (RouteImageUrl != null && RouteImageUrl.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    RouteImageUrl.CopyTo(memoryStream);
                    routeImage.RouteImageUrl = memoryStream.ToArray(); // 將圖片轉換為 byte array
                }
            }
            else
            {
                ModelState.AddModelError("RouteImageUrl", "Image is required.");
            }

            // 如果所有資料都正確，將資料存入資料庫
            if (ModelState.IsValid)
            {
                _context.RouteImages.Add(routeImage);
                _context.SaveChanges();

                // 重定向到詳細頁面，並將 RouteId 傳回去
                return RedirectToAction("Details", new { id = routeImage.RouteId });
            }

            // 如果有錯誤，返回表單頁面並顯示錯誤訊息
            return View(routeImage);
        }


        public ActionResult EditRouteImage(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var routeImage = _context.RouteImages.FirstOrDefault(ri => ri.RouteImageId == id);
            if (routeImage == null)
            {
                return RedirectToAction("Index");
            }

            return View(routeImage);
        }

        [HttpPost]
        public ActionResult EditRouteImage(RouteImage routeImage, IFormFile RouteImageUrl)
        {
            var dbRouteImage = _context.RouteImages.FirstOrDefault(ri => ri.RouteImageId == routeImage.RouteImageId);
            if (dbRouteImage == null)
            {
                return RedirectToAction("Index");
            }

            // 檢查是否有上傳新圖片
            if (RouteImageUrl != null && RouteImageUrl.Length > 0)
            {
                // 使用 MemoryStream 轉換上傳的圖片為 byte array
                using (var memoryStream = new MemoryStream())
                {
                    RouteImageUrl.CopyTo(memoryStream);
                    dbRouteImage.RouteImageUrl = memoryStream.ToArray(); // 將圖片更新為新圖片
                }
            }

            // 更新圖片描述
            dbRouteImage.RouteImageDescription = routeImage.RouteImageDescription;

            // 保存更改到資料庫
            _context.SaveChanges();

            // 重定向到詳細頁面
            return RedirectToAction("Details", new { id = dbRouteImage.RouteId });
        }




        public ActionResult DeleteRouteImage(int? id)
        {
            if (id == null)
            {
                return NotFound(); // 如果 id 為 null，返回 404
            }

            var routeImage = _context.RouteImages.FirstOrDefault(ri => ri.RouteImageId == id);
            if (routeImage != null)
            {
                _context.RouteImages.Remove(routeImage); // 從資料庫中移除該圖片
                _context.SaveChanges(); // 保存更改
            }

            // 返回到詳細頁面
            return RedirectToAction("Details", new { id = routeImage?.RouteId });
        }
        /// <summary>
        //-----------------------------------------------------------------------------------------------
        public ActionResult CreateSchedule(int routeId)
        {
            var newSchedule = new Schedule { RouteId = routeId };
            return View(newSchedule);
        }

        [HttpPost]
        public ActionResult CreateSchedule(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Schedules.Add(schedule);
                _context.SaveChanges();

                // 重定向到詳細頁面
                return RedirectToAction("Details", new { id = schedule.RouteId });
            }

            return View(schedule);
        }

        public async Task<IActionResult> DeleteSchedule(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Route)  // Include the related Route data if needed
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            try
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // 記錄或處理錯誤
                return StatusCode(500, "An error occurred while deleting the schedule.");
            }

            // 如果您想在刪除後返回某個頁面，可以像這樣返回 Route 詳細頁面
            return RedirectToAction(nameof(Details), new { id = schedule.Route.RouteId });
        }

        public ActionResult EditSchedule(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // 查找需要編輯的行程
            var schedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == id);
            if (schedule == null)
            {
                return RedirectToAction("Index");
            }

            // 如果找到了行程，返回該行程到 View
            return View(schedule);
        }
        [HttpPost]
        public ActionResult EditSchedule(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                // 查找數據庫中要更新的 Schedule
                var dbSchedule = _context.Schedules.FirstOrDefault(s => s.ScheduleId == schedule.ScheduleId);
                if (dbSchedule == null)
                {
                    return RedirectToAction("Index");
                }

                // 更新 Schedule 的屬性
                dbSchedule.DepartureTime = schedule.DepartureTime;
                dbSchedule.ArrivalTime = schedule.ArrivalTime;
                dbSchedule.Seats = schedule.Seats;
                dbSchedule.Capacity = schedule.Capacity;

                // 保存更改
                _context.Entry(dbSchedule).State = EntityState.Modified;
                _context.SaveChanges();

                // 重定向到詳細頁面
                return RedirectToAction("Details",  new { id = dbSchedule.RouteId });
            }

            // 如果 ModelState 無效，返回原表單並顯示錯誤訊息
            return View(schedule);
        }




    }
}