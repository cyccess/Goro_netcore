using System.Collections.Generic;
using System.Data;

namespace Goro.Check.Service.Model
{
    public  class SalesReturnNoticeDetail
    {
        public DataTable Order { get; set; }

        public List<FieldDisplayed> Field { get; set; }
    }
}
