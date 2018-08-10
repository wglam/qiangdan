using cn.bmob.io;


namespace WpfQiangdan
{
    class BmobUser : BmobTable
    {
        private string _table;

        public BmobUser()
        {
            this._table = "auth_user";
        }
        public override string table
        {
            get
            {
                if (_table != null)
                {
                    return _table;
                }
                return base.table;
            }
        }
        public string user_id { get; set; }
        public string mac { get; set; }
        public BmobInt times { get; set; }


        //读字段信息
        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            this.user_id = input.getString("user_id");
            this.mac = input.getString("mac");
            this.times = input.getInt("times");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);
            output.Put("user_id", this.user_id);
            output.Put("mac", this.mac);
            output.Put("times", this.times);
        }
    }
}
