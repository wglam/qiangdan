using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfQiangdan.bean
{
    class AllGood
    {
        public IEnumerable<GoodItem> presell { get; set; }

        public IEnumerable<GoodItem> sell { get; set; }
    }
}
