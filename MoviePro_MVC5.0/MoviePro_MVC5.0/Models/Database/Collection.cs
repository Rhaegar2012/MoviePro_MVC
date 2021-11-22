using System.Collections.Generic;

namespace MoviePro_MVC5._0.Models.Database
{
    public class Collection
    {
        //Primary key
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //Navigation properties
        public ICollection<MovieCollection> MovieCollection { get; set; } = new HashSet<MovieCollection>();
    }
}
