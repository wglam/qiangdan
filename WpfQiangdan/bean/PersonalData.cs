using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfQiangdan.bean
{
    class PersonalData
    {
        public OrderBubbles orderBubbles { get; set; }

        public int waitConsume
        {
            get
            {
                return orderBubbles != null ? orderBubbles.waitConsume : 0;
            }
        }

        public int waitPay
        {
            get
            {
                return orderBubbles != null ? orderBubbles.waitPay : 0;
            }
        }

    }
}
