using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface ViewpointService
    {
        Task<bool> CreateViewpoint(ViewpointEntity viewpointEntity);

        Task<bool> UpdateViewpoint(ViewpointEntity viewpointEntity);

        Task<bool> DeleteViewpoint(int id);

        Task<IEnumerable<ViewpointEntity>> GetViewpointAsync(int id);

        Task<List<ViewpointEntity>> GetViewpointList();

        Task<bool> AddViewpointFromExcel(List<ViewpointEntity> viewpointEntities);
    }
}
