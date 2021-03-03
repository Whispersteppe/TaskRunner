using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TaskRunner.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        private bool _isChanged;
        public virtual bool IsChanged
        {
            get
            {
                return _isChanged;
            }
            set
            {
                _isChanged = value;
            }
        }


        public ModelBase()
        {
        }

        public virtual void ResetIsChanged()
        {
            _isChanged = false;
        }

        public virtual void RefreshToConfig()
        {

        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (TaskRunnerController.Current.IsLoading == false)
            {
                _isChanged = true;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyyDataErrorInfo

        public bool HasErrors
        {
            get
            {
                return _errors.Count > 0;
            }
        }

        readonly List<ValidationResult> _errors = new List<ValidationResult>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            var propertyErrors = _errors.Where(x => x.MemberNames.Contains(propertyName)).ToList();

            if (propertyErrors.Count() > 0) return propertyErrors;

            return null;

        }

        public void OnError(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }


        public void ValidateProperty(string propertyName)
        {
            var propertyErrors = _errors.Where(x => x.MemberNames.Contains(propertyName)).ToList();
            propertyErrors.ForEach(x => {
                _errors.Remove(x);
            });

            var property = GetType().GetProperty(propertyName);
            var validationResults = new List<ValidationResult>();

            var validationContext = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            var isValid = Validator.TryValidateProperty(property.GetValue(this), validationContext, validationResults);

            if (isValid == false)
            {
                foreach (var rslt in validationResults)
                {
                    _errors.Add(rslt);
                }

                OnError(propertyName);
            }
        }


        #endregion
    }
}
