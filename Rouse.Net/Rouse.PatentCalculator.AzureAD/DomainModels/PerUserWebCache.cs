using System;
using System.ComponentModel.DataAnnotations;

namespace Rouse.PatentCalculator.AzureAD.DomainModels
{
    public class PerUserWebCache
    {
        [Key]
        public int EntryId { get; set; }
        public string WebUserUniqueId { get; set; }
        public byte[] CacheBits { get; set; }
        public DateTime LastWrite { get; set; }
    }
}
