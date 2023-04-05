using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsTest
{
    public class AsyncForm : Form
    {

        public AsyncForm()
        {
            var panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            var button = new Button();
            button.AutoSize = true;
            button.Text = "LoadBigFile";
            button.Click += async (s, e) =>
            {
                await Task.Delay(1000).ConfigureAwait(false);
                /*
                var path = @"D:\資深菜鳥工程師\BigFile.txt";
                using var file = File.OpenRead(path);
                var buffer= new byte[1];*/
            };
            var time = new BindPropertyChangedHandle<string>(DateTime.Now.ToString());
            var label = new Label();
            label.DataBindings.Add(time.ToBind(nameof(Label.Text)));
            label.AutoSize = true;
            panel.Controls.Add(button);
            panel.SetFlowBreak(button, true);
            panel.Controls.Add(label);
            this.Controls.Add(panel);
            time.RaisePropertyChanged();
            var timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, e) =>
            {
                time.Value = DateTime.Now.ToString();
            };
            timer.Start();
        }
    }
}
