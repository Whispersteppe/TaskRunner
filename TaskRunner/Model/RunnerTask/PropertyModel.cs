namespace TaskRunner.Model.RunnerTask
{
    public class PropertyModel : ModelBase
    {
        string _name;
        object _value;

        public string Name 
        { 
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }



        public PropertyModel(string name, object value)
        {
            _name = name;
            _value = value;
        }
    }
}
