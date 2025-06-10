using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IDocseriesService
    {
         Task<string> GetNewDocno(string Doctype);
      //   Task<DocSeries> UpdateDocno(DocSeries ds);
    }
}