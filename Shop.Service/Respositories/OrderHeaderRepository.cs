using Microsoft.EntityFrameworkCore;
using Shop.DataAccess.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Repository.Respositories
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
/*
        public void PaymentStatus(int Id, string SessionId, string PaymentIntentId)
        {
            var order = _context.OrderHeaders.FirstOrDefault(o => o.Id == Id);
            order.SessionId = SessionId;
            order.PaymentIntentId = PaymentIntentId;
            order.DateofPayment = DateTime.Now;
        }*/

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int Id, string orderStatus, string? paymentStatus = null)
        {
            var order = _context.OrderHeaders.FirstOrDefault(o => o.Id == Id);
            if (order != null)
            {
                order.OrderStatus = orderStatus;                
            }
/*            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
            }*/
        }
    }
}
