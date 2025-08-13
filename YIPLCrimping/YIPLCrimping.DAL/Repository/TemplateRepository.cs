using Microsoft.EntityFrameworkCore;
using YIPLCrimping.DAL.Models;
using YIPLCrimpingAPI.Models;

namespace YIPLCrimping.DAL.Repository
{
    public class TemplateRepository
    {
        private readonly AppDbContext appDbContext;

        //private readonly YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        private readonly Helper.YIPLCrimping.Helper.Logger logger = Helper.YIPLCrimping.Helper.Logger.Instance;

        public TemplateRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<int> Add(MTemplateFile template)
        {
            await appDbContext.MTemplateFiles.AddAsync(template);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> Update(MTemplateFile template)
        {
            appDbContext.MTemplateFiles.Update(template);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<MTemplateFile?> GetById(int id)
        {
            var master = await appDbContext.MTemplateFiles.FirstOrDefaultAsync(master => master.IsActive == true && master.Id == id);
            return master;
        }

        public async Task<MTemplateFile> GetFileByMasterName(string masterName)
        {
            var master = await appDbContext.MTemplateFiles.FirstOrDefaultAsync(master => master.IsActive == true && master.MasterName == $"M_{masterName}");
            return master;
        }
    }
}