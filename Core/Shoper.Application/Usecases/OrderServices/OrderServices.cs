using Shoper.Application.Dtos.OrderDtos;
using Shoper.Application.Dtos.OrderItemDtos;
using Shoper.Application.Interfaces;
using Shoper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shoper.Application.Usecases.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly IRepository<Order> _repository;
        private readonly IRepository<OrderItem> _repositoryOrderItem;

        public OrderServices(IRepository<Order> repository, IRepository<OrderItem> repositoryOrderItem = null)
        {
            _repository = repository;
            _repositoryOrderItem = repositoryOrderItem;
        }

        public async Task CreateOrderAsync(CreateOrderDto model)
        {
            var order = new Order
            {
                OrderDate = model.OrderDate,
                TotalAmount = model.TotalAmount,
                OrderStatus = model.OrderStatus,
                //BillingAddress = model.BillingAddress,
                ShippingAddress = model.ShippingAddress,
                PaymentMethod = model.PaymentMethod,
                CustomerId = model.CustomerId,
            };
            await _repository.CreateAsync(order);

            foreach (var item in model.OrderItems)
            {
                await _repositoryOrderItem.CreateAsync(new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                });
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var values = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(values);
        }

        public async Task<List<ResultOrderDto>> GetAllOrderAsync()
        {
            var values = await _repository.GetAllAsync();
            var orderItem = await _repositoryOrderItem.GetAllAsync();
            return values.Select(x => new ResultOrderDto
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                OrderStatus = x.OrderStatus,
                //BillingAddress = x.BillingAddress,
                ShippingAddress = x.ShippingAddress,
                PaymentMethod = x.PaymentMethod,
                CustomerId = x.CustomerId,
                OrderItems = x.OrderItems.Select(oi => new ResultOrderItemDto
                {
                    OrderId=oi.OrderId,
                    ProductId=oi.ProductId,
                    Quantity = oi.Quantity,
                    TotalPrice=oi.TotalPrice,
                    OrderItemId= oi.OrderItemId,
                }).ToList()
            }).ToList();
        }

        public async Task<GetByIdOrderDto> GetByIdOrderAsync(int id)
        {
            var values = await _repository.GetByIdAsync(id);
            var result = new GetByIdOrderDto
            {
                OrderId = values.OrderId,
                OrderDate = values.OrderDate,
                TotalAmount = values.TotalAmount,
                OrderStatus = values.OrderStatus,
                //BillingAddress = values.BillingAddress,
                ShippingAddress = values.ShippingAddress,
                PaymentMethod = values.PaymentMethod,
                CustomerId = values.CustomerId,
            };
            return result;
        }

        public async Task UpdateOrderAsync(UpdateOrderDto model)
        {
            var values = await _repository.GetByIdAsync(model.OrderId);
            values.OrderStatus = model.OrderStatus;
            values.OrderDate = model.OrderDate;
            values.TotalAmount = model.TotalAmount;
            //values.BillingAddress = model.BillingAddress;
            values.ShippingAddress = model.ShippingAddress;
            values.PaymentMethod = model.PaymentMethod;
            values.CustomerId = model.CustomerId;
            await _repository.UpdateAsync(values);
        }
    }
}
