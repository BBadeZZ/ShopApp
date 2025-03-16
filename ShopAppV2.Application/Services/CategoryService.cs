// using AutoMapper;
// using ShopAppV2.Infrastructure.Data;
// using ShopAppV2.Infrastructure.Dtos;
// using ShopAppV2.Infrastructure.Entities;
// using ShopAppV2.Infrastructure.Models;
//
// namespace ShopAppV2.Infrastructure.Services;
//
// public class CategoryService : ICategoryService
// {
//     private readonly AdoDbContext _adoDbContext;
//
//     private readonly IMapper _mapper;
//
//     public CategoryService(AdoDbContext adoDbContext, IMapper mapper)
//     {
//         _adoDbContext = adoDbContext;
//         _mapper = mapper;
//     }
//
//     public async Task<Response<List<Category>>> GetAllCategoriesAsync()
//     {
//         const string query = "SELECT * FROM public.\"Categories\"";
//
//         var categories = await _adoDbContext.ExecuteQueryAsync(query, reader => new Category
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name"))
//         });
//
//         return categories.Count != 0
//             ? Response<List<Category>>.Success("Ok", categories)
//             : Response
//                 <List<Category>>.Fail("No categories found!");
//     }
//
//     public async Task<Response<Category>> GetCategoryByIdAsync(int id)
//     {
//         var query = $"SELECT * FROM public.\"Categories\" WHERE id = {id}";
//         var category = await _adoDbContext.ExecuteSingleQueryAsync(query, reader => new Category
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name"))
//         });
//
//         return category != null
//             ? Response<Category>.Success("Ok", category)
//             : Response<Category>.Fail("Category not found!");
//     }
//
//     public async Task<Response<Category>> CreateCategoryAsync(CategoryDto categoryDto)
//     {
//         var query = @$"
//     INSERT INTO public.""Categories""(
// 	name)
// 	VALUES ('{categoryDto.Name}') RETURNING id, name;
// ";
//         var createdCategory = await _adoDbContext.ExecuteSingleQueryAsync<Category>(query,
//             command => { command.Parameters.AddWithValue("@name", categoryDto.Name); }, reader => new Category
//             {
//                 Id = reader.GetInt32(reader.GetOrdinal("id")),
//                 Name = reader.GetString(reader.GetOrdinal("name"))
//             });
//
//         return createdCategory != null
//             ? Response<Category>.Success("Ok", createdCategory)
//             : Response<Category>.Fail("Category not created!");
//     }
//
//
//     public async Task<Response<Category>> UpdateCategoryAsync(int id, CategoryDto categoryDto)
//     {
//         // Check that the category exists
//         var resp = await GetCategoryByIdAsync(id);
//         if (resp.Status == "fail")
//             return Response<Category>.Fail("Failed to update category! Category does not exist!");
//
//         var query = @"UPDATE public.""Categories""
//                      SET ""name"" = @name
//                      WHERE ""id"" = @id RETURNING id, name";
//
//         var updatedCategory = await _adoDbContext.ExecuteSingleQueryAsync<Category>(query, command =>
//         {
//             command.Parameters.AddWithValue("@id", id);
//             command.Parameters.AddWithValue("@name", categoryDto.Name);
//         }, reader => new Category
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name"))
//         });
//
//         return updatedCategory != null
//             ? Response<Category>.Success("Ok", updatedCategory)
//             : Response<Category>.Fail("Category not updated!");
//     }
//
//     public async Task<Response> DeleteCategoryAsync(int id)
//     {
//         // Check that the category exists
//         var resp = await GetCategoryByIdAsync(id);
//         if (resp.Status == "fail") return Response.Fail("Failed to delete category! Category does not exist!");
//
//         var query = @"DELETE FROM public.""Categories"" WHERE ""id"" = @id";
//         var rowsAffected =
//             await _adoDbContext.ExecuteNonQueryAsync(query, command => { command.Parameters.AddWithValue("@id", id); });
//
//         return rowsAffected > 0 ? Response.Success("Ok") : Response.Fail("Failed to delete category!");
//     }
// }

