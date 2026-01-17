using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace Modique.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("image")]
        [Authorize(Roles = "Administrator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Invalid file type. Only images are allowed." });

            if (file.Length > 5 * 1024 * 1024) // 5MB max
                return BadRequest(new { message = "File size exceeds 5MB limit." });

            try
            {
                var webRootPath = _environment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                    if (!Directory.Exists(webRootPath))
                    {
                        Directory.CreateDirectory(webRootPath);
                    }
                }

                var uploadsFolder = Path.Combine(webRootPath, "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var imageUrl = $"{baseUrl}/uploads/products/{fileName}";
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, new { message = "An error occurred while uploading the image." });
            }
        }

        [HttpPost("images")]
        [Authorize(Roles = "Administrator")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UploadImages()
        {
            _logger.LogInformation("UploadImages endpoint called");
            var files = Request.Form.Files;
            _logger.LogInformation("Files count: {Count}", files?.Count ?? 0);
            
            if (files == null || files.Count == 0)
                return BadRequest(new { message = "No files uploaded." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    continue;

                if (file.Length > 5 * 1024 * 1024) // 5MB max
                    continue;

                try
                {
                    var webRootPath = _environment.WebRootPath;
                    if (string.IsNullOrEmpty(webRootPath))
                    {
                        webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                    }

                    var uploadsFolder = Path.Combine(webRootPath, "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    imageUrls.Add($"{baseUrl}/uploads/products/{fileName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
                }
            }

            if (imageUrls.Count == 0)
                return BadRequest(new { message = "No valid images were uploaded." });

            return Ok(new { imageUrls });
        }
    }
}
