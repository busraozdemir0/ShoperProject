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
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Product> _productRepository;

        public OrderServices(IRepository<Order> repository, IRepository<OrderItem> repositoryOrderItem = null, IRepository<Customer> customerRepository = null, IRepository<Product> productRepository = null)
        {
            _repository = repository;
            _repositoryOrderItem = repositoryOrderItem;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task CreateOrderAsync(CreateOrderDto model)
        {
            decimal sum = 0;
            var order = new Order
            {
                OrderDate = model.OrderDate,
                TotalAmount = sum,
                OrderStatus = model.OrderStatus,
                //BillingAddress = model.BillingAddress,
                ShippingAddress = model.ShippingAddress,
                //PaymentMethod = model.PaymentMethod,
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
                sum = sum + item.TotalPrice;
            }

            order.TotalAmount = sum;
            await _repository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var values = await _repository.GetByIdAsync(id);
            var orderItems = await _repositoryOrderItem.GetAllAsync(); // Bu metod EF Core'dan geliyor ve order.OrderItems bir navigation property old için (yani ilişkili tabloya ait verileri içerdiği için), EF Core o listeyi otomatik olarak doldurmaz. Sadece proxy objesi olarak bırakır. Bu sebeple hepsini belleğe alıyoruz
            foreach (var item in values.OrderItems) // Once siparis icerisindeki orderItem'lar siliniyor
            {
                var orderItem = await _repositoryOrderItem.GetByIdAsync(item.OrderItemId);
                await _repositoryOrderItem.DeleteAsync(orderItem);
            }
            await _repository.DeleteAsync(values);
        }

        public async Task<List<ResultOrderDto>> GetAllOrderAsync()
        {
            var values = await _repository.GetAllAsync();
            var orderItems = await _repositoryOrderItem.GetAllAsync();
            var result = new List<ResultOrderDto>();
            foreach (var item in values)
            {
                var orderCustomer = await _customerRepository.GetByIdAsync(item.CustomerId);
                var orderDto = new ResultOrderDto
                {
                    OrderId = item.OrderId,
                    OrderDate = item.OrderDate,
                    TotalAmount = item.TotalAmount,
                    OrderStatus = item.OrderStatus,
                    //BillingAddress = item.BillingAddress,
                    ShippingAddress = item.ShippingAddress,
                    //PaymentMethod = item.PaymentMethod,
                    CustomerId = item.CustomerId,
                    Customer = orderCustomer,
                    OrderItems = new List<ResultOrderItemDto>(),
                };

                foreach (var orderItem in item.OrderItems)
                {
                    var orderItemProduct = await _productRepository.GetByIdAsync(orderItem.ProductId);
                    var orderItemDto = new ResultOrderItemDto
                    {
                        OrderId = orderItem.OrderId,
                        ProductId = orderItem.ProductId,
                        Quantity = orderItem.Quantity,
                        TotalPrice = orderItem.TotalPrice,
                        OrderItemId = orderItem.OrderItemId,
                        Product = orderItemProduct,
                    };
                    orderDto.OrderItems.Add(orderItemDto);
                }
                result.Add(orderDto);
            }

            return result;
        }

        public async Task<GetByIdOrderDto> GetByIdOrderAsync(int id)
        {
            var values = await _repository.GetByIdAsync(id);
            var orderItems = await _repositoryOrderItem.GetAllAsync();
            var orderCustomer = await _customerRepository.GetByIdAsync(values.CustomerId);
            var result = new GetByIdOrderDto
            {
                OrderId = values.OrderId,
                OrderDate = values.OrderDate,
                TotalAmount = values.TotalAmount,
                OrderStatus = values.OrderStatus,
                //BillingAddress = values.BillingAddress,
                ShippingAddress = values.ShippingAddress,
                //PaymentMethod = values.PaymentMethod,
                CustomerId = values.CustomerId,
                Customer = orderCustomer,
                OrderItems = new List<ResultOrderItemDto>(),
            };
            foreach (var item in values.OrderItems)
            {
                var orderItemProduct = await _productRepository.GetByIdAsync(item.ProductId);
                var orderItemDto = new ResultOrderItemDto
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    OrderItemId = item.OrderItemId,
                    Product = orderItemProduct,
                };
                result.OrderItems.Add(orderItemDto);
            }
            return result;
        }

        public async Task UpdateOrderAsync(UpdateOrderDto model)
        {
            var values = await _repository.GetByIdAsync(model.OrderId);
            var orderItems = await _repositoryOrderItem.GetAllAsync();
            values.OrderStatus = model.OrderStatus;

            decimal sum = 0;
            foreach(var itemModel in model.OrderItems)
            {
                foreach(var item in values.OrderItems)
                {
                    var orderItemDto = await _repositoryOrderItem.GetByIdAsync(item.OrderItemId);
                    if(itemModel.OrderItemId == item.OrderItemId)
                    {
                        orderItemDto.Quantity = itemModel.Quantity;
                        orderItemDto.TotalPrice = itemModel.TotalPrice;
                    }
                    sum += item.TotalPrice;
                }
            }
            values.TotalAmount = sum;

            await _repository.UpdateAsync(values);
        }
    }
}
