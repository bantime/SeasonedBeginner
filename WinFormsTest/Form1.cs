using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WinFormsTest
{
    public class BindPropertyChangedHandle<T> : INotifyPropertyChanged
    {
        private T _Value;

        public T Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public BindPropertyChangedHandle(T value = default)
        {
            _Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Binding ToBind(string propertyName)
        {
            return new Binding(propertyName, this, "Value");
        }

        public Binding ToBind(string propertyName, DataSourceUpdateMode dataSourceUpdateMode)
        {
            return new Binding(propertyName, this, "Value", true, dataSourceUpdateMode);
        }
        public Binding ToBind(string propertyName, DataSourceUpdateMode dataSourceUpdateMode, Func<T, object> onFormat, Func<object, T> onParse)
        {
            var bind = new Binding(propertyName, this, "Value", true, dataSourceUpdateMode);
            bind.Format += (s, e) =>
            {
                if (e.Value is T v)
                {
                    e.Value = onFormat(v);
                }
            };
            bind.Parse += (s, e) =>
            {
                e.Value = onParse(e.Value);
            };
            return bind;
        }

        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            var data = new BindPropertyChangedHandle<string>();
            var check = new BindPropertyChangedHandle<bool>();
            var value = new BindPropertyChangedHandle<int>(1);
            InitializeComponent();
            var flow = new FlowLayoutPanel();
            flow.AutoSize = true;
            flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flow.Dock = DockStyle.Fill;

            var label = new Label();
            var button = new Button();
            var checkBox = new CheckBox();
            button.Text = "Click";
            button.AutoSize = true;
            button.Click += (s, e) =>
            {
                // data.Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                MessageBox.Show(value.Value.ToString());
            };
            flow.Controls.Add(button);
            flow.SetFlowBreak(button, true);
            label.DataBindings.Add(data.ToBind(nameof(Label.Text)));
            label.AutoSize = true;
            flow.Controls.Add(label);
            flow.SetFlowBreak(label, true);
            checkBox.AutoSize = true;
            checkBox.Text = "Check?";
            var bind = value.ToBind(nameof(CheckBox.Checked), DataSourceUpdateMode.OnPropertyChanged,(v) => v % 2 == 1,(o) =>
            {
                if (o is bool i)
                {
                    return i ? 1 : 0;
                }
                return default;
            });
            checkBox.DataBindings.Add(bind);
            flow.Controls.Add(checkBox);

            this.Controls.Add(flow);
            value.RaisePropertyChanged();

        }
    }
}
