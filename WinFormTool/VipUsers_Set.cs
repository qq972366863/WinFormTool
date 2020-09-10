namespace WinFormTool
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VipUsers_Set
    {
        public int Id { get; set; }

        public int? VipUserId { get; set; }

        [StringLength(150)]
        public string ProductOffer { get; set; }

        [StringLength(150)]
        public string Linkman { get; set; }

        [StringLength(150)]
        public string Phone { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(150)]
        public string Fax { get; set; }

        [StringLength(150)]
        public string EnterpriseName { get; set; }

        [StringLength(150)]
        public string EnterpriseWeb { get; set; }
    }
}
