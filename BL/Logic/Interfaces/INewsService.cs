using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;

namespace BL.Logic.Interfaces
{
    public interface INewsService
    {
        public Task<NewItem> GetNewById(string id);
        public Task<IEnumerable<NewItemResult>> GetAllNews();
    }
}
