using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface MemberReservationRepo
    {
        Task<bool> CreateAsync(MemberReservationEntity memberReservationEntity);

        Task<bool> UpdateAsync(MemberReservationEntity memberReservationEntity);

        Task<bool> DeleteAsync(int id);

        Task<bool> DeleteBindAsync(int id);

        Task<bool> DeleteReservationBindAsync(string code);

        Task<List<MemberReservationEntity>> GetByMemberCode(string code);

        Task<List<MemberReservationEntity>> GetByCode(string code);

        Task<MemberReservationEntity> GetByIds(string ids, int type = 1);

        Task<List<MemberReservationEntity>> GetAll();
    }
}
