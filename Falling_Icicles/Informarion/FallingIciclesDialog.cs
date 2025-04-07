using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Falling_Icicles.Information
{
    public class FallingIciclesDialog
    {
        /// <summary>
        /// 質問をダイアログを出し、回答を返す
        /// </summary>
        /// <param name="message">質問内容</param>
        /// <returns></returns>
        public static MessageBoxResult GetOKCancel(string title, string message)
        {
            return MessageBox.Show(
                Application.Current.MainWindow,
                message,
                $"{title} ({PluginInfo.Title})",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question
                );
        }

        /// <summary>
        /// 警告する
        /// </summary>
        /// <param name="message">警告内容</param>
        public static void ShowWarning(string title, string message)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                message,
                $"{title} ({PluginInfo.Title})",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
                );
        }

        /// <summary>
        /// 情報表示する（フツーのダイアログ）
        /// </summary>
        /// <param name="message">内容</param>
        public static void ShowInformation(string title, string message)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                message,
                $"{title} ({PluginInfo.Title})",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }


        /// <summary>
        /// エラー表示する
        /// </summary>
        /// <param name="cont">内容</param>
        public static void ShowError(string title, string cont, string? className, string? methodName)
        {
            MessageBox.Show(
                Application.Current.MainWindow,
                "エラーが発生しましたが、本プラグインで発生したエラーに対するフィードバッグは受け付けておりません☆" +
                "\nエラー内容" +
                "\n---" +
                "\n" + cont +
                "\n---" +
                "\nエラー発生箇所" +
                "\n\tクラス：" + (className ?? "(情報なし)") +
                "\n\tメソッド：" + (methodName ?? "(情報なし)"),
                $"{title} ({PluginInfo.Title})",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
        }

        public static void ShowIntro()
        {
            var result = MessageBox.Show(
                Application.Current.MainWindow,
                "⚠注意書き⚠" +
                "\n以下の内容をしっかり読んで、指示に従ってください。" +
                "\n------" +
                "\nこのゲームは、『東方project』の二次創作です。" +
                "\n本作品のストーリーは原作と一切関係ございません。" +
                "\n" +
                "\nまた、本作品にはメタ的な演出がございますので、もしソフトに問題が発生しても、" +
                "YMM4のフィードバック送信機能やエラー報告機能は絶対に使用しないでください。" +
                "\n" +
                "\nプレイ時はお使いの端末がインターネットに接続されている状態であることを推奨します。" +
                "\n------" +
                "\nわかったら拒否のボタンを押してね。",
                PluginInfo.Title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation
                );

            if (result == MessageBoxResult.Yes)
            {
                PluginInfo.Agree = false;
            }
        }

        public static void ShowNotification(string title, string message)
        {
            NotifyIcon notifyIcon = new()
            {
                Icon = SystemIcons.Information, // アイコン設定
                Visible = true,
                BalloonTipTitle = title,
                BalloonTipText = message,
                BalloonTipIcon = ToolTipIcon.Info
            };

            notifyIcon.ShowBalloonTip(3000); // 3秒表示
        }

        static private string SinBetaKunX => "https://x.com/sinBetaKun";
    }
}
