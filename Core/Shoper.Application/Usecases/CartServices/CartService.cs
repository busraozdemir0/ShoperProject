using Shoper.Application.Dtos.CartDtos;
using Shoper.Application.Dtos.CartItemDtos;
using Shoper.Application.Dtos.ProductDtos;
using Shoper.Application.Interfaces;
using Shoper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoper.Application.Usecases.CartServices
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _repository;
        private readonly IRepository<CartItem> _itemRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Product> _productRepository;

        public CartService(IRepository<Cart> repository, IRepository<CartItem> itemRepository, IRepository<Customer> customerRepository, IRepository<Product> productRepository)
        {
            _repository = repository;
            _itemRepository = itemRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task CreateCartAsync(CreateCartDto model)
        {
            var cart = new Cart
            {
                //TotalAmount = model.TotalAmount,
                CreatedDate = DateTime.Now,
                CustomerId = model.CustomerId,
            };
            await _repository.CreateAsync(cart);

            var sum = 0;
            foreach (var item in model.CartItems)
            {
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice, // üründen gidip fiyatı alacak ve adet ile çarpacak
                };
                sum = sum + (item.TotalPrice);
                await _itemRepository.CreateAsync(cartItem);
            }
            cart.TotalAmount = sum;
            await _repository.UpdateAsync(cart);

        }

        public async Task DeleteCartAsync(int id)
        {
            var cart = await _repository.GetByIdAsync(id);
            var cartItems = await _itemRepository.GetAllAsync();
            foreach (var item in cartItems)
            {
                if (item.CartId == id)
                {
                    var cartItem = await _itemRepository.GetByIdAsync(item.CartItemId);
                    await _itemRepository.DeleteAsync(cartItem);
                }
            }
            await _repository.DeleteAsync(cart);
        }

        public async Task<List<ResultCartDto>> GetAllCartAsync()
        {
            var carts = await _repository.GetAllAsync();
            var cartItems = await _itemRepository.GetAllAsync();
            var result = new List<ResultCartDto>();
            foreach (var item in carts)
            {
                var customer = await _customerRepository.GetByFilterAsync(c => c.CustomerId == item.CustomerId);
                var cartDto = new ResultCartDto
                {
                    CartId = item.CartId,
                    CreatedDate = item.CreatedDate,
                    CustomerId = item.CustomerId,
                    Customer = customer,
                    TotalAmount = item.TotalAmount,
                    CartItems = new List<ResultCartItemDto>(),
                };
                foreach (var cartItem in item.CartItems)
                {
                    var product = await _productRepository.GetByFilterAsync(p => p.ProductId == cartItem.ProductId);
                    var cartItemDto = new ResultCartItemDto
                    {
                        CartId = cartItem.CartId,
                        CartItemId = cartItem.CartItemId,
                        ProductId = cartItem.ProductId,
                        Product = product,
                        Quantity = cartItem.Quantity,
                        TotalPrice = cartItem.TotalPrice,
                    };
                    cartDto.CartItems.Add(cartItemDto);
                }
                result.Add(cartDto);
            }
            return result;
        }

        public async Task<GetByIdCartDto> GetByIdCartAsync(int id)
        {
            var cart = await _repository.GetByIdAsync(id);
            var cartItem = await _itemRepository.GetAllAsync();
            var customer = await _customerRepository.GetByIdAsync(id);
            var result = new GetByIdCartDto
            {
                CartId = cart.CartId,
                CartItems = new List<ResultCartItemDto>(),
                CreatedDate = cart.CreatedDate,
                CustomerId = cart.CustomerId,
                Customer = customer,
            };
            foreach (var item in cart.CartItems)
            {
                var product = await _productRepository.GetByFilterAsync(p => p.ProductId == item.ProductId);
                var cartItemDto = new ResultCartItemDto
                {
                    CartId = item.CartId,
                    CartItemId = item.CartItemId,
                    ProductId = item.ProductId,
                    Product = product,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                };
                result.CartItems.Add(cartItemDto);
            }
            return result;
        }

        public async Task UpdateCartAsync(UpdateCartDto model)
        {
            var cart = await _repository.GetByIdAsync(model.CartId);
            var cartItems = await _itemRepository.GetAllAsync();
            var sum = 0;
            foreach (var cartModelItem in model.CartItems)
            {
                foreach (var item in cart.CartItems)
                {
                    var cartItem = await _itemRepository.GetByIdAsync(item.CartItemId);

                    if (item.CartItemId == cartModelItem.CartItemId)
                    {
                        cartItem.Quantity = cartModelItem.Quantity;
                        cartItem.TotalPrice = cartModelItem.TotalPrice;
                    }
                    sum = sum + item.TotalPrice;
                }
            }
            cart.TotalAmount = sum;
            await _repository.UpdateAsync(cart);
        }
    }
}
