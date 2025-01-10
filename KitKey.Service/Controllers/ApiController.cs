using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Shared;
using Serilog;
using System.Text;

namespace KitKey.Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly KkServer _keyServer;

        public ApiController(KkServer keyServer)
        {
            _keyServer = keyServer;
        }

        [HttpPost("Set/{storeKey}/{key}")]
        public async Task<IActionResult> Set(string storeKey, string key)
        {
            try
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var bodyValue = await reader.ReadToEndAsync();
                _keyServer.SetValue(storeKey, key, bodyValue);
                return Ok("value stored");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpDelete("Delete/{storeKey}/{key}")]
        public IActionResult Upsert(string storeKey, string key)
        {
            try
            {
                _keyServer.Delete(storeKey, key);
                return Ok("value deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet("Get/String/{storeKey}/{key}")]
        public IActionResult Get(string storeKey, string key)
        {
            try
            {
                var value = _keyServer.GetValue<string>(storeKey, key);
                if (value == null)
                {
                    return NoContent();
                }
                return Ok(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Creates a key-value store with the default configuration.
        /// </summary>
        /// <param name="storeKey"></param>
        /// <returns></returns>
        [HttpPost("StoreCreate/{storeKey}")]
        public IActionResult StoreCreate(string storeKey)
        {
            try
            {
                _keyServer.StoreCreate(new KkStoreConfiguration
                {
                    StoreKey = storeKey
                });
                return Ok("store created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Creates a key-value store with a custom configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("StoreCreate")]
        public IActionResult StoreCreate([FromBody] KkStoreConfiguration config)
        {
            try
            {
                if (config == null || string.IsNullOrEmpty(config.StoreKey))
                {
                    return BadRequest("StoreKey is required.");
                }

                _keyServer.StoreCreate(config);

                return Ok("store created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpDelete("StorePurge/{storeKey}")]
        public IActionResult Purge(string storeKey)
        {
            try
            {
                _keyServer.StorePurge(storeKey);
                return Ok("store purged");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }


        [HttpDelete("StoreDelete/{storeKey}")]
        public IActionResult StoreDelete(string storeKey)
        {
            try
            {
                _keyServer.StoreDelete(storeKey);
                return Ok("store deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
