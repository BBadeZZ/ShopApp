// using AutoMapper;
// using ShopAppV2.Infrastructure.Data;
// using ShopAppV2.Infrastructure.Dtos;
// using ShopAppV2.Infrastructure.Entities;
// using ShopAppV2.Infrastructure.Models;
//
// namespace ShopAppV2.Infrastructure.Services;
//
// public class ProductService : IProductService
// {
//     private readonly AdoDbContext _adoDbContext;
//     private readonly ICategoryManagerService _categoryManagerService;
//
//     private readonly IImageUploaderService _imageUploaderService;
//
//     private readonly IMapper _mapper;
//
//     public ProductService(AdoDbContext adoDbContext, IMapper mapper, ICategoryManagerService categoryManagerService,
//         IImageUploaderService imageUploaderService)
//     {
//         _adoDbContext = adoDbContext;
//         _mapper = mapper;
//         _categoryManagerService = categoryManagerService;
//         _imageUploaderService = imageUploaderService;
//     }
//
//     public async Task<Response<List<ProductDto>>> GetAllProductsAsync()
//     {
//         const string query =
//             @"SELECT ""id"", ""name"", ""description"", ""stock_quantity"", ""category_id"", ""created_at"", ""updated_at"", ""price"", ""rating"", ""is_active"", ""image_path""
//                           FROM public.""Products""";
//
//         var products = await _adoDbContext.ExecuteQueryAsync(query, reader => new Product
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name")),
//             Description = reader.IsDBNull(reader.GetOrdinal("description"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("description")),
//             StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
//             CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
//             CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
//             UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
//                 ? null
//                 : reader.GetDateTime(reader.GetOrdinal("updated_at")),
//             Price = reader.GetDouble(reader.GetOrdinal("price")),
//             Rating = reader.GetInt32(reader.GetOrdinal("rating")),
//             IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
//             ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("image_path"))
//         });
//
//         foreach (var product in products)
//             product.Category = new Category
//             {
//                 Id = product.CategoryId,
//                 Name = await _categoryManagerService.GetCategoryNameById(product.CategoryId)
//             };
//
//         var productsDto = _mapper.Map<List<ProductDto>>(products);
//
//         return productsDto.Count != 0
//             ? Response<List<ProductDto>>.Success("Ok", productsDto)
//             : Response
//                 <List<ProductDto>>.Fail("No products found!");
//     }
//
//     public async Task<Response<ProductDto>> GetProductByIdAsync(int id)
//     {
//         var query =
//             $@"SELECT ""id"", ""name"", ""description"", ""stock_quantity"", ""category_id"", ""created_at"", ""updated_at"", ""price"", ""rating"", ""is_active"", ""image_path""
//                           FROM public.""Products"" WHERE ""id"" = {id}";
//         var product = await _adoDbContext.ExecuteSingleQueryAsync(query, reader => new Product
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name")),
//             Description = reader.IsDBNull(reader.GetOrdinal("description"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("description")),
//             StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
//             CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
//             CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
//             UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
//                 ? null
//                 : reader.GetDateTime(reader.GetOrdinal("updated_at")),
//             Price = reader.GetDouble(reader.GetOrdinal("price")),
//             Rating = reader.GetInt32(reader.GetOrdinal("rating")),
//             IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
//             ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("image_path"))
//         });
//
//         if (product != null)
//         {
//             product.Category = new Category
//             {
//                 Id = product.CategoryId,
//                 Name = await _categoryManagerService.GetCategoryNameById(product.CategoryId)
//             };
//
//             var productDto = _mapper.Map<ProductDto>(product);
//             return Response<ProductDto>.Success("Ok", productDto);
//         }
//
//         return Response<ProductDto>.Fail("Product not found!");
//     }
//
//     public async Task<Response<ProductDto>> CreateProductAsync(ProductFormDto productFormDto)
//     {
//         var product = _mapper.Map<Product>(productFormDto);
//
//         // Upload the image
//         product.ImagePath = productFormDto.Image != null
//             ? await _imageUploaderService.UploadImage(productFormDto.Image)
//             : "";
//
//         var query = @$"
//     INSERT INTO public.""Products"" (
//         name, description, stock_quantity, category_id, created_at, price, rating, is_active, image_path
//     ) VALUES (
//         @name, @description, @stockQuantity, @categoryId, '{DateTime.Now:yyyy-MM-dd HH:mm:ss}', @price, @rating, @isActive, @imagePath
//     ) RETURNING id, name, description, stock_quantity, category_id, created_at, price, rating, is_active, image_path;
// ";
//         var createdProduct = await _adoDbContext.ExecuteSingleQueryAsync<Product>(query, command =>
//         {
//             command.Parameters.AddWithValue("@name", product.Name);
//             command.Parameters.AddWithValue("@description", (object?)product.Description ?? DBNull.Value);
//             command.Parameters.AddWithValue("@stockQuantity", product.StockQuantity);
//             command.Parameters.AddWithValue("@categoryId", product.CategoryId);
//             command.Parameters.AddWithValue("@createdAt", product.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
//             command.Parameters.AddWithValue("@price", product.Price);
//             command.Parameters.AddWithValue("@rating", product.Rating);
//             command.Parameters.AddWithValue("@isActive", product.IsActive);
//             command.Parameters.AddWithValue("@imagePath", (object?)product.ImagePath ?? DBNull.Value);
//         }, reader => new Product
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name")),
//             Description = reader.IsDBNull(reader.GetOrdinal("description"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("description")),
//             StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
//             CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
//             CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
//             Price = reader.GetDouble(reader.GetOrdinal("price")),
//             Rating = reader.GetInt32(reader.GetOrdinal("rating")),
//             IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
//             ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("image_path"))
//         });
//
//         if (createdProduct != null)
//             createdProduct.Category = new Category
//             {
//                 Id = createdProduct.CategoryId,
//                 Name = await _categoryManagerService.GetCategoryNameById(createdProduct.CategoryId)
//             };
//
//         return createdProduct != null
//             ? Response<ProductDto>.Success("Ok", _mapper.Map<ProductDto>(createdProduct))
//             : Response<ProductDto>.Fail("Failed to create product! Make sure to enter a valid category_id!");
//     }
//
//
//     public async Task<Response<ProductDto>> UpdateProductAsync(int id, ProductFormDto productFormDto)
//     {
//         // Check that the product exists
//         var resp = await GetProductByIdAsync(id);
//         if (resp.Status == "fail")
//             return Response<ProductDto>.Fail("Failed to update product! Product does not exist!");
//
//         var imagePath = productFormDto.Image != null
//             ? await _imageUploaderService.UploadImage(productFormDto.Image)
//             : "";
//
//         var query = @$"
//     UPDATE public.""Products"" 
//     SET 
//         name = @name,
//         description = @description,
//         stock_quantity = @stockQuantity,
//         category_id = @categoryId,
//         updated_at = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',
//         price = @price,
//         rating = @rating,
//         is_active = @isActive,
//         image_path = @imagePath
//     WHERE id = @id
//     RETURNING id, name, description, stock_quantity, category_id, updated_at, price, rating, is_active, image_path;
// ";
//         var updatedProduct = await _adoDbContext.ExecuteSingleQueryAsync<Product>(query, command =>
//         {
//             command.Parameters.AddWithValue("@id", id);
//             command.Parameters.AddWithValue("@name", productFormDto.Name);
//             command.Parameters.AddWithValue("@description", (object?)productFormDto.Description ?? DBNull.Value);
//             command.Parameters.AddWithValue("@stockQuantity", productFormDto.StockQuantity);
//             command.Parameters.AddWithValue("@categoryId", productFormDto.CategoryId);
//             command.Parameters.AddWithValue("@price", productFormDto.Price);
//             command.Parameters.AddWithValue("@rating", productFormDto.Rating);
//             command.Parameters.AddWithValue("@isActive", productFormDto.IsActive);
//             command.Parameters.AddWithValue("@imagePath", imagePath);
//         }, reader => new Product
//         {
//             Id = reader.GetInt32(reader.GetOrdinal("id")),
//             Name = reader.GetString(reader.GetOrdinal("name")),
//             Description = reader.IsDBNull(reader.GetOrdinal("description"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("description")),
//             StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
//             CategoryId = reader.GetInt32(reader.GetOrdinal("category_id")),
//             CreatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
//             Price = reader.GetDouble(reader.GetOrdinal("price")),
//             Rating = reader.GetInt32(reader.GetOrdinal("rating")),
//             IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
//             ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path"))
//                 ? null
//                 : reader.GetString(reader.GetOrdinal("image_path"))
//         });
//
//         if (updatedProduct != null)
//             updatedProduct.Category = new Category
//             {
//                 Id = updatedProduct.CategoryId,
//                 Name = await _categoryManagerService.GetCategoryNameById(updatedProduct.CategoryId)
//             };
//
//         return updatedProduct != null
//             ? Response<ProductDto>.Success("Ok", _mapper.Map<ProductDto>(updatedProduct))
//             : Response<ProductDto>.Fail("Failed to update product! Make sure to enter a valid category_id!");
//     }
//
//     public async Task<Response> DeleteProductAsync(int id)
//     {
//         // Check that the product exists
//         var resp = await GetProductByIdAsync(id);
//         if (resp.Status == "fail") return Response.Fail("Failed to delete product! Product does not exist!");
//
//
//         var query = @"DELETE FROM public.""Products"" WHERE ""id"" = @id";
//         var rowsAffected =
//             await _adoDbContext.ExecuteNonQueryAsync(query, command => { command.Parameters.AddWithValue("@id", id); });
//
//         return rowsAffected > 0 ? Response.Success("Ok") : Response.Fail("Failed to delete product!");
//     }
// }

