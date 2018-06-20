using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Goro.Check.Service.Model
{
    public class SalesOrderDetail
    {
        public DataTable Order { get; set; }

        public List<FieldDisplayed> Field { get; set; }
    }
}
