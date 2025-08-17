using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca.Classes
{

    public class ScreenshotEntry
    {
        public byte[] Hash { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }

}
