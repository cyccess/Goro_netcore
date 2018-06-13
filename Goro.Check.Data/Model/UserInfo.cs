using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Goro.Check.Data.Model
{
    [Table("tm_v_UserInfo")]
    public class UserInfo
    {
        public string FEmpName { get; set; }

        public string FPhoneNumber { get; set; }

        public int? FEmpID { get; set; }

        public string FEmpNumber { get; set; }

        [Key]
        public int FUserGroupID { get; set; }

        public string FUserGroupNumber { get; set; }

        public string FUserGroupName { get; set; }
    }
}
