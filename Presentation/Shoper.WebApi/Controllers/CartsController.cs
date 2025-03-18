using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shoper.Application.Dtos.CartDtos;
using Shoper.Application.Usecases.CartServices;

namespace Shoper.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCart()
        {
            var values = await _cartService.GetAllCartAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdCart(int id)
        {
            var value = await _cartService.GetByIdCartAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart(CreateCartDto dto)
        {
            await _cartService.CreateCartAsync(dto);
            return Ok("Cart'ınız başarılı bir şekilde oluşturuldu.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart(UpdateCartDto dto)
        {
            await _cartService.UpdateCartAsync(dto);
            return Ok("Cart'ınız başarılı bir şekilde güncellendi.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart(int id)
        {
            await _cartService.DeleteCartAsync(id);
            return Ok("Cart'ınız başarılı bir şekilde silindi.");
        }
    }
}
