namespace WinFormTool
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class VipUsers
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string UserPassword { get; set; }

        [StringLength(50)]
        public string PhoneNum { get; set; }

        [Column(TypeName = "image")]
        public byte[] UserAvatar { get; set; }

        public DateTime? RegistrTime { get; set; }

        public DateTime? ExpireTime { get; set; }

        public int? Authority { get; set; }

        public int? Loginstate { get; set; }

        public int? Enabledstate { get; set; }

        [StringLength(50)]
        public string Access_Token { get; set; }
    }
}
