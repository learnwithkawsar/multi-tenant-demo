using Finbuckle.MultiTenant;
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
        private readonly ITenantInfo? _currentTenant;

        public ProductsController(ApplicationDbContext context, ITenantInfo? currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {

            var tenant = _currentTenant.Identifier;

            Console.WriteLine($"Current Tenant: {tenant}");



            // Log or use tenant information
            // Console.WriteLine($"Current Tenant: {currentTenant.Identifier} - {currentTenant.Name}");

            return await _context.Products.ToListAsync();
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
