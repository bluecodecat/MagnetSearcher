using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnetSearcher
{
    class MagnetLinkItem
    {
        public int No { get; set; }
        public string Name { get; set; }

        public string SizeStr { get; set; }

        public long SizeByte { get; set; }

        public string MagnetLink { get; set; }

        public string Type { get; set; }

        public string CreatedTime { get; set; }

        public int Health { get; set; }

        public int FileCountP { get; set; }
        public string Content { get; set; }
    }
}
