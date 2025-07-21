using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo.DbContexts;
using MultiTenantDemo.Models;


namespace MultiTenantDemo.Controllers
{
    [ApiController]
    [Route("{tenant}/api/[controller]")] // Support tenant-specific routes
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMultiTenantContextAccessor _tenantAccessor;
        public ProductsController(ApplicationDbContext context, IMultiTenantContextAccessor tenantAccessor, ILogger<ProductsController> logger)
        {
            _context = context;
            _tenantAccessor = tenantAccessor;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {


            // Log tenant info when querying
            var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo as AppTenantInfo;
            _logger.LogInformation($"[ProductService] Querying products for tenant: {tenant?.Name ?? "NULL"}");

            // Log the actual connection string being used
            var connectionString = _context.Database.GetConnectionString();
            _logger.LogInformation($"[ProductService] Using connection: {connectionString?.Substring(0, 50)}...");

            var products = await _context.Products.ToListAsync();

            _logger.LogInformation($"[ProductService] Found {products.Count} products");

            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Get the current tenant from the route
            var tenant = RouteData.Values["tenant"]?.ToString();

            return CreatedAtAction(nameof(GetProduct), new { tenant = tenant, id = product.Id }, product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }


    }
}
