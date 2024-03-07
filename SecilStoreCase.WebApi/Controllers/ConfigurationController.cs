using Microsoft.AspNetCore.Mvc;
using SecilStoreCase.Business;
using SecilStoreCase.Dal.Interfaces.Repositories;
using SecilStoreCase.Entities.Entities;

namespace SecilStoreCase.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationItemRepository _configurationItemRepository;
        private readonly ConfigurationReader _configurationReader;

        public ConfigurationController(IConfigurationItemRepository configurationItemRepository, ConfigurationReader configurationReader)
        {
            _configurationItemRepository = configurationItemRepository;
            _configurationReader = configurationReader;
            
        }

        [HttpGet("{applicationName}")]
        public async Task<IActionResult> GetAllConfigurations(string applicationName)
        {
            try
            {
                var configurations = await _configurationItemRepository.GetAllConfigurationsAsync(applicationName);
                return Ok(configurations);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllConfigurations()
        {
            try
            {
                var configurations = await _configurationItemRepository.GetAllConfigurationsAsync();
                return Ok(configurations);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("value/{key}")]
        public IActionResult GetConfigurationValue(string key)
        {
            try
            {
                var value = _configurationReader.GetValue<string>(key);
                return Ok(value);
            }
            catch (KeyNotFoundException knfEx)
            {

                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateConfiguration([FromBody] ConfigurationItemModel configurationModel)
        {
            try
            {
                await _configurationItemRepository.CreateConfigurationAsync(configurationModel);
                return CreatedAtAction(nameof(GetAllConfigurations), new { applicationName = configurationModel.ApplicationName }, configurationModel);
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfiguration([FromBody] ConfigurationItemModel configurationItemModel)
        {
            try
            {
                var updateResult = await _configurationItemRepository.UpdateConfigurationAsync(configurationItemModel);
                if (!updateResult)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguration(string id)
        {
            try
            {
                var deleteResult = await _configurationItemRepository.DeleteAsync(id);
                if (!deleteResult)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetConfigurationById/{id}")]
        public async Task<IActionResult> GetConfigurationByIdAsync(string id)
        {

            try
            {
                var configurations = await _configurationItemRepository.GetConfigurationByIdAsync(id);
                return Ok(configurations);

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
