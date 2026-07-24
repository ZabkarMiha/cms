using Microsoft.AspNetCore.Mvc;
using CMSApi.Business;
using Microsoft.AspNetCore.Authorization;

namespace CMSApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class BlobController : ControllerBase
    {
        private readonly IBlobBusinessLayer _blobBusinessLayer;
        public BlobController(IBlobBusinessLayer blobBusinessLayer){
            _blobBusinessLayer = blobBusinessLayer;
        }

        [Route("CreateContainer/{containerName}")]
        [HttpPost]
        public async Task<IActionResult> CreateContainerAsync(string containerName){
            await _blobBusinessLayer.CreateContainer(containerName);
            return Ok();
        }

        [Route("GetContainers")]
        [HttpGet]
        public async Task<List<string>> GetContainersAsync(){
            return await _blobBusinessLayer.ListContainers();
        }
    }
}