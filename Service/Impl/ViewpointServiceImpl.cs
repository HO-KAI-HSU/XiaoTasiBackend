using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class ViewpointServiceImpl : ViewpointService
    {
        ViewpointRepo _viewpointRepo;

        public ViewpointServiceImpl()
        {
            _viewpointRepo = new ViewpointRepoImpl();
        }

        public async Task<bool> CreateViewpoint(ViewpointEntity viewpointEntity)
        {
            await _viewpointRepo.CreateAsync(viewpointEntity);
            return true;
        }

        public async Task<bool> DeleteViewpoint(int id)
        {
            await _viewpointRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ViewpointEntity>> GetViewpointAsync(int id)
        {
            List<ViewpointEntity> list = await _viewpointRepo.GetAll();

            IEnumerable<ViewpointEntity> viewpointEntity =
                from viewpoint in list
                where viewpoint.viewpointId == id
                select viewpoint;

            return viewpointEntity;
        }

        public async Task<List<ViewpointEntity>> GetViewpointList()
        {
            List<ViewpointEntity> list = await _viewpointRepo.GetAll();
            return list;
        }

        public async Task<bool> UpdateViewpoint(ViewpointEntity viewpointEntity)
        {
            await _viewpointRepo.UpdateAsync(viewpointEntity);
            return true;
        }

        public async Task<bool> AddViewpointFromExcel(List<ViewpointEntity> viewpointEntities)
        {
            int lastCount = viewpointEntities.Count - 1;
            int index = 0;
            string sql = "INSERT INTO viewpoint_list (viewpoint_title, viewpoint_city, viewpoint_area, viewpoint_address, viewpoint_content, viewpoint_pic_path) VALUES ";
            foreach (ViewpointEntity viewpointData in viewpointEntities)
            {
                string viewpointTitle = viewpointData.viewpointTitle;
                string viewpointCity = viewpointData.viewpointCity;
                string viewpointArea = viewpointData.viewpointArea;
                string viewpointAddress = viewpointData.viewpointAddress;
                string viewpointContent = viewpointData.viewpointContent;
                if (index == lastCount)
                {
                    sql += "(N'" + viewpointTitle + "', N'" + viewpointCity + "', N'" + viewpointArea + "', N'" + viewpointAddress + "', N'" + viewpointContent + "', '" + "" + "')";
                }
                else
                {
                    sql += "(N'" + viewpointTitle + "', N'" + viewpointCity + "', N'" + viewpointArea + "', N'" + viewpointAddress + "', N'" + viewpointContent + "', '" + "" + "'),";
                }
                index++;
            }

            await _viewpointRepo.MultiAddAsync(sql);

            return true;
        }
    }
}
