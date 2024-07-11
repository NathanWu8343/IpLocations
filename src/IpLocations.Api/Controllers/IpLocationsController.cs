using IpLocations.Api.Infra;
using Microsoft.AspNetCore.Mvc;

namespace IpLocations.Api.Controllers
{
    /// <summary>
    /// IP to Country
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IpLocationsController : ControllerBase
    {
        private readonly IpRangeStore _ipRangeStore;

        public IpLocationsController(IpRangeStore ipRangeStore)
        {
            _ipRangeStore = ipRangeStore;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string ip)
        {
            var result = await _ipRangeStore.GetCountryAsync(ip);
            return string.IsNullOrEmpty(result) ? NotFound() : Ok(result);
        }
    }
}