using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class SeoMeta
    {
        public int SeoMetaId { get; set; }
        public string EntityType { get; set; } = string.Empty; 
        public int EntityId { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string UrlSlug { get; set; } = string.Empty;
        public string? Keywords { get; set; }
    }
}
