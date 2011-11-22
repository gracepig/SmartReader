using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Windows.Media.Imaging;

namespace SmartReader.Library.DataContract
{
    [Table]
    public class ArticleImage
    {
        private int _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }

        [Column]
        public string ImageUrl { get; set; }

        [Column(IsVersion = true)]
        private Binary version;

        [Column(DbType = "Image")] 
        public byte[] ImageBytes;
        
        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<Chapter> _chapter;

        [Column]
        public int ChapterId { set; get; }

        [Column]
        public int SequenceId { set; get; }
    }
}
