using cn.bmob.io;


namespace WpfQiangdan
{
    class BmobData : BmobTable
    {
        private string _table;

        public BmobData()
        {
            this._table = "upload_data";
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
        public string value { get; set; }
  


        //读字段信息
        public override void readFields(BmobInput input)
        {
            base.readFields(input);
            this.user_id = input.getString("user_id");
            this.value = input.getString("value");
        }

        //写字段信息
        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);
            output.Put("user_id", this.user_id);
            output.Put("value", this.value);
        }
    }
}
