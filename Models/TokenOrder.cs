namespace OnlineAuctions.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TokenOrder
    {
        public int TokensNumber { get; set; }

        public decimal? Price { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        public int Id { get; set; }

        public int IdUser { get; set; }

        public virtual User User { get; set; }
    }
}
