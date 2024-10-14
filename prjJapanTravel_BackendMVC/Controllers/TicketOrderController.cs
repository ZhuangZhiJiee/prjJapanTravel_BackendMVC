﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjJapanTravel_BackendMVC.Models;
using prjJapanTravel_BackendMVC.ViewModels.OrderViewModels;

namespace prjJapanTravel_BackendMVC.Controllers
{
    public class TicketOrderController : Controller
    {
        public JapanTravelContext _context;
        public TicketOrderController(JapanTravelContext context)
        {
            _context = context;
        }
        public IActionResult List()
        {
            var datas = _context.TicketOrders
                .Include(o => o.Member)
                .Include(o => o.PaymentMethod)
                .Include(o => o.PaymentStatus)
                .Include(o => o.Coupon)
                .Select(m => new TicketOrderListViewModel()
            {
                船票訂單編號 = m.TicketOrderId,
                訂單編號 = m.TicketOrderNumber,
                會員 = m.Member.MemberName,
                下單時間 = m.OrderTime,
                付款方式 = m.PaymentMethod.PaymentMethod1,
                付款狀態 = m.PaymentStatus.PaymentStatus1,
                訂單狀態 = m.OrderStatus.OrderStatus1,
                優惠券 = m.Coupon.CouponName,
                總金額 = m.TotalAmount


                });
            return View(datas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TicketOrder to)
        {
            _context.TicketOrders.Add(to);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        public IActionResult Edit(int? id)
        {
            var data = _context.TicketOrders
                .Include(io => io.Member)
                .FirstOrDefault(io => io.TicketOrderId== id);
            if (data == null)
                return RedirectToAction("List");

            //ViewBag.MemberName = 

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Cancel(int? id)
        {
            var data = _context.TicketOrders.FirstOrDefault(io => io.TicketOrderId== id);
            if (data != null)
            {
                data.OrderStatusId = 3;
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}
