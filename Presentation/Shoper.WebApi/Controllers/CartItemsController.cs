using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shoper.Application.Dtos.CartDtos;
using Shoper.Application.Dtos.CartItemDtos;
using Shoper.Application.Usecases.CartItemServices;

namespace Shoper.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemsController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartItem()
        {
            var values = await _cartItemService.GetAllCartItemAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdCartItem(int id)
        {
            var value = await _cartItemService.GetByIdCartItemAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCartItem(CreateCartItemDto dto)
        {
            await _cartItemService.CreateCartItemAsync(dto);
            return Ok("Cart Item'ınız başarılı bir şekilde oluşturuldu.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItem(UpdateCartItemDto dto)
        {
            await _cartItemService.UpdateCartItemAsync(dto);
            return Ok("Cart Item'ınız başarılı bir şekilde güncellendi.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            await _cartItemService.DeleteCartItemAsync(id);
            return Ok("Cart Item'ınız başarılı bir şekilde silindi.");
        }
    }
}
