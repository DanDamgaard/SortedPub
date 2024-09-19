using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortedPub.Classes
{
    public class BookClass
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Path { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Auther { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public int OrderNr { get; set; }

    }
}
