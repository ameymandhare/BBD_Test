using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimpingAPI.Controllers
{
    [Route("template")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly TemplateService templateService;
        private readonly AppDbContext appDbContext;
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly YIPLCrimping.Helper.YIPLCrimping.Helper.Logger logger = YIPLCrimping.Helper.YIPLCrimping.Helper.Logger.Instance;

        [NonAction]
        public override void InitializeController()
        {
        }

        public TemplateController(AppDbContext DBContext, TemplateService templateService, IWebHostEnvironment hostingEnvironment)
        {
            this.appDbContext = DBContext;
            this.templateService = templateService;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("add")]
        [AllowAnonymous]
        public async Task<JsonResult> Add([FromForm] MTemplateFileDto templateDto)
        {
            try
            {
                var response = await templateService.Add(templateDto, hostingEnvironment.WebRootPath);
                if (response == null)
                    return await base.FinalizeEmpty();
                else
                    return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        [HttpPut("update")]
        [AllowAnonymous]
        public async Task<JsonResult> Update([FromForm] MTemplateFileDto templateDto)
        {
            try
            {
                var response = await templateService.Update(templateDto, hostingEnvironment.WebRootPath);
                if (response == null)
                    return await base.FinalizeEmpty();
                else
                    return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        [HttpPut("delete")]
        [AllowAnonymous]
        public async Task<JsonResult> Delete([FromForm] int id, [FromForm] int modifiedBy)
        {
            try
            {
                var response = await templateService.Delete(id, modifiedBy);
                if (response == null)
                    return await base.FinalizeEmpty();
                else
                    return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }

        [HttpGet("get")]
        [AllowAnonymous]
        public async Task<JsonResult> Get([FromForm] string masterName)
        {
            try
            {
                var response = await templateService.Get(masterName, hostingEnvironment.WebRootPath);
                if (response == null)
                    return await base.FinalizeEmpty();
                else
                    return await base.FinalizeSingle(response);
            }
            catch (Exception ex)
            {
                return await base.FinalizeException(ex);
            }
        }
    }
}