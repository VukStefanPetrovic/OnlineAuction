namespace OnlineAuctions.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("Auction")]
    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bids = new HashSet<Bid>();
        }

        [Required]
        public string Name { get; set; }

        public int Duration { get; set; }

        [Required]
        public byte[] Img { get; set; }

        public DateTime? openingTime { get; set; }

        public DateTime? closingTime { get; set; }

        public decimal? currentPrice { get; set; }

        public decimal? startingPrice { get; set; }

        
        public DateTime? createdOn { get; set; }

        [StringLength(10)]
        public string State { get; set; }

        public int Id { get; set; }

       

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bids { get; set; }
    }
}
