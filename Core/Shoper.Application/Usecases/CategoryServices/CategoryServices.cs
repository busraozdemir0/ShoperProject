using Shoper.Application.Dtos.CategoryDtos;
using Shoper.Application.Interfaces;
using Shoper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Shoper.Application.Usecases.CategoryServices
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IRepository<Category> _repository;

        public CategoryServices(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public async Task CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            await _repository.CreateAsync(new Category 
            { 
                CategoryName=createCategoryDto.CategoryName,
            });
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            await _repository.DeleteAsync(category);
        }

        public async Task<List<ResultCategoryDto>> GetAllCategoryAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(x => new ResultCategoryDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
            }).ToList();
        }

        public async Task<GetByIdCategoryDto> GetByIdCategoryAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return new GetByIdCategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var category = await _repository.GetByIdAsync(updateCategoryDto.CategoryId);
            category.CategoryName = updateCategoryDto.CategoryName;
            await _repository.UpdateAsync(category);
        }
    }
}
