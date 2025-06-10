using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;

namespace Core.Interfaces
{
    public interface IFormGridService<T>
    {
        public IUnitOfWork GetUnitOfWork();

        Task<IReadOnlyList<FormGridDetail>> GetFormGridDetails(string FormName, int UserId, string AppRoleCode = null);

        Task<IReadOnlyList<FormGridDetail>> RefreshFormGridDetails(string FormName, int UserId);

        Task<FormGridDetail> UpdateFormGridDetail(string FormName, FormGridDetail FgdData, string UserName);

        Task<IImportExcelData<T>> ImportExcelData(string FormName, AppUser appUser, string fileName, List<string> ExtraFields=null, string datefields=null);

        Task<Byte[]> GetFormDataDownload(string FormName, AppUser appUser, IReadOnlyList<T> data);

        Task<Byte[]> GetTemplateData(string FormName, AppUser ou, Dictionary<string, string> DefaultValues = null, List<string> ExtraFields=null);
    }
}
