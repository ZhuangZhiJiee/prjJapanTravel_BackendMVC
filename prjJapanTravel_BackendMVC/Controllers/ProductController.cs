﻿using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;
using NuGet.Protocol.Resources;
using prjJapanTravel_BackendMVC.Models;
using prjJapanTravel_BackendMVC.ViewModels.ProductViewModels;
using System.Collections.Generic;

namespace prjJapanTravel_BackendMVC.Controllers
{
    public class ProductController : Controller
    {
        private JapanTravelContext _JP;
        IWebHostEnvironment _enviroment;
        public ProductController(JapanTravelContext context, IWebHostEnvironment environment)
        {
            _JP = context;
            _enviroment = environment;
        }
        public IActionResult List()
        {
            var datas = _JP.Itineraries.Select(n => new ItineraryViewModel()
            {
                行程系統編號 = n.ItinerarySystemId,
                行程編號 = n.ItineraryId,
                行程名稱 = n.ItineraryName,
                體驗項目 = n.ActivitySystem.ActivityName,
                總團位 = n.Stock,
                價格 = n.Price,
                地區 = n.AreaSystem.AreaName,
                行程日期 = n.ItineraryDates.Where(date => date.ItinerarySystemId == n.ItinerarySystemId).ToList(),
                行程圖片 = n.Images.Where(img => img.ItinerarySystemId == n.ItinerarySystemId).Select(img => new Image
                {
                    ItineraryPicSystemId = img.ItineraryPicSystemId,
                    ItinerarySystemId = img.ItinerarySystemId,
                    ImagePath = img.ImagePath,
                }).ToList(),
                行程詳情 = n.ItineraryDetail,
                行程簡介 = n.ItineraryBrief,
                行程注意事項 = n.ItineraryNotes
            });
            return View(datas);
        }

        public IActionResult ItineraryCreate()
        {
            CreateListViewModel c = new CreateListViewModel();
            c.areaList = _JP.Areas.ToList();
            c.activityList = _JP.Activities.ToList();
            return View(c);
        }

        [HttpPost]
        public IActionResult ItineraryCreate(ItineraryViewModel itimodel)
        {
            Itinerary itinerary = new Itinerary();

            itinerary.ItineraryId = itimodel.行程編號;
            itinerary.ItineraryName = itimodel.行程名稱;
            itinerary.ActivitySystemId = itimodel.ActivitySystem.ActivitySystemId;
            itinerary.Stock = itimodel.總團位;
            itinerary.Price = itimodel.價格;
            //itinerary.ThemeSystemId = itimodel.ThemeSystem.ThemeSystemId;
            itinerary.AreaSystemId = itimodel.AreaSystem.AreaSystemId;
            itinerary.ItineraryDetail = itimodel.行程詳情;
            itinerary.ItineraryBrief = itimodel.行程簡介;
            itinerary.ItineraryNotes = itimodel.行程注意事項;

            _JP.Itineraries.Add(itinerary);
            _JP.SaveChanges();

            if (itimodel.行程日期 != null && itimodel.行程日期.Count > 0)
            {
                foreach (var date in itimodel.行程日期)
                {
                    ItineraryDate itineraryDate = new ItineraryDate
                    {
                        ItinerarySystemId = itinerary.ItinerarySystemId, // 使用新建行程的 ItinerarySystemId
                        DepartureDate = date.DepartureDate // 來自於 ViewModel 的行程日期
                    };
                    _JP.ItineraryDates.Add(itineraryDate); // 將出發日期添加到上下文中
                }
                _JP.SaveChanges(); // 將所有出發日期保存到資料庫
            }

            if (itimodel.imageViewModel != null && itimodel.imageViewModel.Count > 0)
            {
                foreach (var img in itimodel.imageViewModel)
                {
                    if (img.ImageFile != null && img.ImageFile.Length > 0)
                    {
                        // 生成唯一的圖片名稱
                        string photoname = Guid.NewGuid() + ".jpg";

                        // 設置圖片的保存路徑
                        var filePath = _enviroment.WebRootPath + "/images/Product/" + photoname;
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            img.ImageFile.CopyTo(stream);
                        }

                        // 創建 Image 物件並保存到資料庫
                        Image image = new Image
                        {
                            ItinerarySystemId = itinerary.ItinerarySystemId,
                            ImageName = img.ImageName,
                            ImagePath = photoname,  // 保存相對路徑
                            ImageDetail = img.ImageDetail // 圖片描述
                        };

                        _JP.Images.Add(image);
                        _JP.SaveChanges();
                    }
                }
            }

            return RedirectToAction("List");
        }

        public IActionResult ItineraryDelete(int? id)
        {
            Itinerary iti = _JP.Itineraries.FirstOrDefault(n => n.ItinerarySystemId == id);
            if (iti != null)
            {
                var itineraryTimes = _JP.ItineraryDates.Where(n => n.ItinerarySystemId == id).ToList();
                var itineraryImages = _JP.Images.Where(n => n.ItinerarySystemId == id).ToList();

                _JP.ItineraryDates.RemoveRange(itineraryTimes);
                _JP.Images.RemoveRange(itineraryImages);
                _JP.Itineraries.Remove(iti);

                _JP.SaveChanges();
            }
            return RedirectToAction("List");
        }
        public IActionResult ItineraryEdit(int? id)
        {

            var data = _JP.Itineraries.Where(n => n.ItinerarySystemId == id).Select(n => new ItineraryViewModel()
            {
                行程系統編號 = n.ItinerarySystemId,
                行程編號 = n.ItineraryId,
                行程名稱 = n.ItineraryName,
                體驗項目編號 = n.ActivitySystemId,
                體驗項目 = n.ActivitySystem.ActivityName,
                總團位 = n.Stock,
                價格 = n.Price,
                體驗主題編號 = n.ThemeSystemId,
                地區編號 = n.AreaSystemId,
                地區 = n.AreaSystem.AreaName,
                行程圖片 = n.Images.Where(pic => pic.ItinerarySystemId == n.ItinerarySystemId).ToList(),
                行程詳情 = n.ItineraryDetail,
                行程簡介 = n.ItineraryBrief,
                行程注意事項 = n.ItineraryNotes
            }).FirstOrDefault();

            // 查詢地區和體驗項目選單資料
            data.地區選項 = new SelectList(_JP.Areas, "AreaSystemId", "AreaName", data.地區編號);
            data.體驗項目選項 = new SelectList(_JP.Activities, "ActivitySystemId", "ActivityName", data.體驗項目編號);

            return View(data);

        }
        [HttpPost]
        public IActionResult ItineraryEdit(ItineraryViewModel itiModel)
        {
            var itinerary = _JP.Itineraries.FirstOrDefault(n => n.ItinerarySystemId == itiModel.行程系統編號);
            if (itinerary == null)
            {
                return RedirectToAction("List");
            }

            itinerary.ItineraryId = itiModel.行程編號;
            itinerary.ItineraryName = itiModel.行程名稱;
            itinerary.ActivitySystemId = itiModel.體驗項目編號;
            itinerary.Stock = itiModel.總團位;
            itinerary.Price = itiModel.價格;
            itinerary.ThemeSystemId = itiModel.體驗主題編號;
            itinerary.AreaSystemId = itiModel.地區編號;
            itinerary.ItineraryDetail = itiModel.行程詳情;
            itinerary.ItineraryBrief = itiModel.行程簡介;
            itinerary.ItineraryNotes = itiModel.行程注意事項;

            if (itiModel.imageViewModel != null && itiModel.imageViewModel.Count > 0 && itiModel.imageViewModel[0].ImageFile != null && itiModel.imageViewModel[0].ImageFile.Length > 0)
            {
                var oldImage = _JP.Images.FirstOrDefault(i => i.ItinerarySystemId == itinerary.ItinerarySystemId);

                // 如果已經有舊圖片，先刪除舊的圖片
                if (oldImage != null && !string.IsNullOrEmpty(oldImage.ImagePath))
                {
                    var oldImagePath = Path.Combine(_enviroment.WebRootPath, oldImage.ImagePath);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);  // 刪除舊圖片
                    }

                    // 刪除資料庫中的舊圖片紀錄
                    _JP.Images.Remove(oldImage);
                }

                string photoname = Guid.NewGuid() + ".jpg";
                var filePath = Path.Combine(_enviroment.WebRootPath, "images/Product", photoname);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    itiModel.imageViewModel[0].ImageFile.CopyTo(stream);
                }

                // 新建圖片紀錄並更新路徑到資料庫
                Image newImage = new Image
                {
                    ItinerarySystemId = itinerary.ItinerarySystemId,
                    ImagePath = photoname,  // 保存相對路徑
                };

                _JP.Images.Add(newImage);
            }
            else
            {
                // 如果沒有新圖片，那就保持舊圖片不變
                var oldImage = _JP.Images.FirstOrDefault(i => i.ItinerarySystemId == itinerary.ItinerarySystemId);
                if (oldImage != null)
                {
                    // 保留現有圖片的紀錄，不做更動
                    itinerary.Images.Add(oldImage);
                }
            }

            _JP.SaveChanges();
            return RedirectToAction("List");

        }
    }
}
