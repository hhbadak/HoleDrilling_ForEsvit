using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ProductCode
    {
        public int ID { get; set; }
        public byte Number { get; set; }
        public string Definition { get; set; }
        public string Clay { get; set; }
        public string MoldCode { get; set; }
        public string Description { get; set; }
        public string StockCode { get; set; }
        public int Stock { get; set; }
        public string Group { get; set; }
        public string Model { get; set; }
        public string Dimensions { get; set; }
        public string Weight { get; set; }
        public byte LabelFeature { get; set; }
        public string FormCode { get; set; }
    }
}
