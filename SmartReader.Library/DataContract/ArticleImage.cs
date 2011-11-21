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

        [Column]
        private int _chapterId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<Chapter> _chapter;


        [Association(Storage = "_chapter", ThisKey = "_chapterId", OtherKey = "Id", IsForeignKey = true)]
        public Chapter Chapter
        {
            get { return _chapter.Entity; }
            set
            {
                _chapter.Entity = value;

                if (value != null)
                {
                    _chapterId = value.Id;
                }
            }
        }
    }
}
