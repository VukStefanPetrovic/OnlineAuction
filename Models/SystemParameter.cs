namespace OnlineAuctions.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SystemParameter
    {
        public decimal? Currency { get; set; }

        public int? Silver { get; set; }

        public int? Gold { get; set; }

        public int? Platnium { get; set; }

        public decimal? TokensValue { get; set; }

        public int ItemsPerPage { get; set; }

        public int Id { get; set; }
    }
}
