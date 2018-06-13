using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Goro.Check.Data.Model
{
    [Table("tm_v_SalesReturnNotice")]
    class SalesReturnNotice
    {
        [Key]
        public int FInterID { get; set; }

        public string FBillNo { get; set; }

        public string FCustName { get; set; }
    }
}
