using System;
using System.Collections.Generic;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Template;

namespace TaskRunner.Model.Template
{
    public class PropertyTemplateModel : ModelBase
    {
        public PropertyTemplate Config { get; private set; }
        public PropertyTemplateModel(PropertyTemplate config)
        {
            Config = config;
        }

        public PropertyTemplateType PropertyType
        {
            get
            {
                return Config.PropertyType;
            }
            set
            {
                Config.PropertyType = value;
                OnPropertyChanged(nameof(PropertyType));
            }
        }

        public string Name
        {
            get
            {
                return Config.Name;
            }
            set
            {
                Config.Name = value;
                OnPropertyChanged(nameof(Name));
            }

        }
        public string DefaultValue
        {
            get
            {
                return Config.DefaultValue;
            }
            set
            {
                Config.DefaultValue = value;
                OnPropertyChanged(nameof(DefaultValue));
            }

        }

        public int Order
        {
            get
            {
                return Config.Order;
            }
            set
            {
                Config.Order = value;
                OnPropertyChanged(nameof(Order));
            }
        }


    }
}
