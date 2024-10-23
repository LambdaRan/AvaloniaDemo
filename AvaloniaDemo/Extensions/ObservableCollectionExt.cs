using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaDemo.Extensions
{
    public class ObservableCollectionExt<T> : ObservableCollection<T>
    {
        private static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            _suppressNotification = true;
            foreach (T item in list)
            {
                Items.Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(ResetCollectionChanged);
        }
        public void ClearThenCopyFrom(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            _suppressNotification = true;
            Items.Clear();
            foreach (T item in list)
            {
                Items.Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(ResetCollectionChanged);
        }
    }
}
