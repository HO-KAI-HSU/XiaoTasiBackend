using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface AccountRepo
    {
        Task<AccountEntity> GetByCode(string code);

        Task<AccountEntity> GetByPhone(string phone);

        Task<List<AccountEntity>> GetAll();

        Task<bool> CreateAsync(AccountEntity seatEntity);

        Task<bool> UpdatePhoneAsync(AccountEntity accountEntity);

        Task<bool> UpdateStatusAsync(AccountEntity accountEntity);

        Task<bool> DeleteAsync(int id);
    }
}
