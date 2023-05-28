using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;
namespace XiaoTasiBackend.Service.Impl
{
    public class AccountServiceImpl : AccountService
    {
        AccountRepo _accountRepo;

        public AccountServiceImpl()
        {
            _accountRepo = new AccountRepoImpl();
        }

        public async Task<IEnumerable<AccountEntity>> GetAccountAsync(string code)
        {
            List<AccountEntity> list = await _accountRepo.GetAll();

            IEnumerable<AccountEntity> accountEntity =
                from account in list
                where account.memberCode == code && account.status == 1
                select account;

            return accountEntity;
        }

        public async Task<IEnumerable<AccountEntity>> GetActivateAccountByPhoneAsync(string memberCode, string phone)
        {
            List<AccountEntity> list = await _accountRepo.GetAll();

            IEnumerable<AccountEntity> accountEntity =
                from account in list
                where account.phone == phone && account.status == 1 && account.memberCode != memberCode
                select account;

            return accountEntity;
        }

        public async Task<bool> UpdateAccountPhoneAsync(AccountEntity accountEntity)
        {
            await _accountRepo.UpdatePhoneAsync(accountEntity);

            return true;
        }
    }
}
