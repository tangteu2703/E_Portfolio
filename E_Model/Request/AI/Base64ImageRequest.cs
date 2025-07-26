using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.AI
{
    public class Base64ImageRequest
    {
        public string Base64 { get; set; } = string.Empty;
    }

    public class AnnotationResult
    {
        public string Label { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public BoxInfo Box { get; set; } = new BoxInfo();

        public class BoxInfo
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }

}
