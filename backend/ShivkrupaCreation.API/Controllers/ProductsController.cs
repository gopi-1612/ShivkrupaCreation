using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShivkrupaCreation.API.DTOs.Common;
using ShivkrupaCreation.API.DTOs.Product;
using ShivkrupaCreation.API.Models;
using ShivkrupaCreation.API.Services.Interfaces;

namespace ShivkrupaCreation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>Public: search, filter, sort, and paginate products for the storefront.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryDto query)
        {
            // The public storefront must never see inactive products, even if a client
            // tries to pass isActive=false — force it here regardless of query input.
            if (!User.IsInRole(Roles.Admin))
            {
                query.IsActive = true;
            }

            var result = await _productService.GetProductsAsync(query);
            return Ok(ApiResponse<PagedResult<ProductListItemDto>>.SuccessResponse(result));
        }

        /// <summary>Public: product details by SEO-friendly slug (used on the product details page).</summary>
        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var product = await _productService.GetBySlugAsync(slug);
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(product));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(product));
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id },
                ApiResponse<ProductDetailDto>.SuccessResponse(product, "Product created."));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ProductDetailDto>.SuccessResponse(product, "Product updated."));
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SetStatus(int id, [FromQuery] bool isActive)
        {
            await _productService.SetActiveStatusAsync(id, isActive);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, isActive ? "Product activated." : "Product deactivated."));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Product deleted."));
        }

        // ---------- Image upload (admin only) ----------

        /// <summary>Upload a product image (multipart/form-data: file, altText, isPrimary).</summary>
        [HttpPost("{id:int}/images")]
        [Authorize(Roles = Roles.Admin)]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> AddImage(int id, [FromForm] List<IFormFile> files, [FromForm] string? altText, [FromForm] bool isPrimary = false)
        {
            foreach (var file in files)
            {
                var image = await _productService.AddImageAsync(id, file, altText, isPrimary);
            }
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Image uploaded."));
        }

        [HttpDelete("{id:int}/images/{imageId:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RemoveImage(int id, int imageId)
        {
            await _productService.RemoveImageAsync(id, imageId);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Image removed."));
        }

        // ---------- Video upload (admin only) ----------

        /// <summary>Upload a product video (multipart/form-data: file, title).</summary>
        [HttpPost("{id:int}/videos")]
        [Authorize(Roles = Roles.Admin)]
        [RequestSizeLimit(60 * 1024 * 1024)]
        public async Task<IActionResult> AddVideo(int id, [FromForm] IFormFile file, [FromForm] string? title)
        {
            var video = await _productService.AddVideoAsync(id, file, title);
            return Ok(ApiResponse<ProductVideoDto>.SuccessResponse(video, "Video uploaded."));
        }

        [HttpDelete("{id:int}/videos/{videoId:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> RemoveVideo(int id, int videoId)
        {
            await _productService.RemoveVideoAsync(id, videoId);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Video removed."));
        }
    }
}
