using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface AccountService
    {
        Task<IEnumerable<AccountEntity>> GetAccountAsync(string code);

        Task<IEnumerable<AccountEntity>> GetActivateAccountByPhoneAsync(string memberCode, string phone);

        Task<bool> UpdateAccountPhoneAsync(AccountEntity accountEntity);
    }
}
