using Falling_Icicles.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YukkuriMovieMaker.Commons;

namespace Falling_Icicles.LogControl
{
    /// <summary>
    /// LogController.xaml の相互作用ロジック
    /// </summary>
    public partial class LogController : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public LogStep Value
        {
            get
            {
                return (LogStep)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
                TextDisplay.Text = value.GetLog();
            }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(LogStep), typeof(LogController), new FrameworkPropertyMetadata(new LogStep(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public LogController()
        {
            InitializeComponent();
            TextDisplay.Text = string.Empty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BeginEdit?.Invoke(this, EventArgs.Empty);
            Value = Value.GetNextLogStep();
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FallingIciclesDialog.ShowWarning(
                "セ ー ブ な ん か ね ぇ よ",
                "データをセーブしたいなら、プロジェクトファイルを保存すればよくない？"
                );
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FallingIciclesDialog.ShowWarning(
                "デ ー タ な ん か ね ぇ よ",
                "セーブデータを削除したいなら、プロジェクトファイルを削除すればよくない？"
                );
        }
    }
}
