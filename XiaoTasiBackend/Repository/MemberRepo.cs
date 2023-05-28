using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface MemberRepo
    {
        Task<MemberEntity> GetById(int id);

        Task<MemberEntity> GetByCode(string code);

        Task<List<MemberEntity>> GetAll();
    }
}
