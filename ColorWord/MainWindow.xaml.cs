using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Speech.Recognition;
using System.Windows.Threading;

namespace ColorWord
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Page
    {
        private SpeechRecognitionEngine speechEngine;  //語音辨識引擎
        private Random random;


        private DispatcherTimer readyTimer;

        private string language = "zh-TW";

        private string[] words = new string[] { "紅", "橙", "黃", "綠", "藍", "靛", "紫", "黑", "白", "青", "橘" };
        private string[] colors = new string[] { "#FFFF0000", "#FF00FF00", "#FF0000FF", "#FF000000", "#FFFFFF00", "#FF9500C5", "#FFFFA400" };

        private int backTime = 70;  // 一回合時間(秒)  

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FileManager fileManager = new FileManager();
            //fileManager.ConfigReader(GameDefinition.SettingFilePath);
            //SettingData data = new SettingData();
            //data = fileManager.GetSettingData();

            double width = Application.Current.Windows[0].Width;
            double height = Application.Current.Windows[0].Height;
            this.Width = width;
            this.Height = height;

            //this.speechEngine = new SpeechRecognitionEngine();
            this.CreateSpeechRecongnition();

            this.random = new Random();

            this.WaitSRInitTimer();                     //等待語音辨識(SR)初始化設定
        }

        private void CreateSpeechRecongnition()
        {
            //Initialize speech recognition            
            var recognizerInfo = (from a in SpeechRecognitionEngine.InstalledRecognizers()
                                  where a.Culture.Name == this.language
                                  select a).FirstOrDefault();

            if (recognizerInfo != null)
            {
                this.speechEngine = new SpeechRecognitionEngine(recognizerInfo.Id);
                Choices recognizerString = new Choices();

                recognizerString.Add(this.words);

                GrammarBuilder grammarBuilder = new GrammarBuilder();

                //Specify the culture to match the recognizer in case we are running in a different culture.                                 
                grammarBuilder.Culture = recognizerInfo.Culture;
                grammarBuilder.Append(recognizerString);

                // Create the actual Grammar instance, and then load it into the speech recognizer.
                var grammar = new Grammar(grammarBuilder);

                //載入辨識字串
                this.speechEngine.LoadGrammarAsync(grammar);
                this.speechEngine.SpeechRecognized += SreSpeechRecognized;

                this.speechEngine.SetInputToDefaultAudioDevice();
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            this.SRColorName.Text = e.Result.Text.ToString();
            
            //this.ConfidenceText.Text = "準確度：" + e.Result.Confidence.ToString();
            //if (e.Result.Confidence < this.confidenceValue)//肯定度低於0.75，判為錯誤語句
            //{
            //    //this.textBlock1.Text = e.Result.Text.ToString();
            //    return;
            //}

            //foreach (var name in imageNameList)
            //{
            //    if (name.Equals(e.Result.Text))
            //    {
            //        if (this.DeleteImage(name))
            //        {
            //            this.totalScore++;
            //            ScoreText.Text = "Success：" + this.totalScore.ToString();
            //            this.PlaySound(this.successSoundPlayer);
            //        }
            //    }
            //}
        }

        void WaitSRInitTimer()
        {
            this.readyTimer = new DispatcherTimer();
            this.readyTimer.Interval = new TimeSpan(0, 0, 4);   //等待4秒，語音辨識(SR)初始化時間
            this.readyTimer.Tick += ReadyTimerTick;
            this.readyTimer.Start();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            Canvas.SetLeft(this.SpeechStatus, (e.NewSize.Width - this.SpeechStatus.Width) / 2);
            Canvas.SetTop(this.SpeechStatus, (e.NewSize.Height - this.SpeechStatus.Height) / 2);
        }

        #region 倒數計時功能

        private DateTime currentDateTime = new DateTime();
        private DateTime oldDateTime;
        private TimeSpan deltaTime = new TimeSpan();
        private float backTime_Temp;

        private void BackTimer()
        {
            this.currentDateTime = DateTime.Now;
            this.deltaTime = this.currentDateTime - this.oldDateTime;
            this.oldDateTime = this.currentDateTime;
            this.backTime_Temp -= (float)this.deltaTime.TotalSeconds;
            this.backTimer_Text.Text = this.backTime_Temp.ToString("00");
        }

        #endregion
        
        /// <summary>
        /// 仿遊戲迴圈(FPS=60)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Update(object sender, EventArgs e)
        {
            
            this.BackTimer();
        }

        /// <summary>
        /// 仿遊戲初始化設定(只執行一次)
        /// </summary>
        private void Start()
        {
            //this.speechEngine.SetInputToDefaultAudioDevice();
            //this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        void ReadyTimerTick(object sender, EventArgs e)
        {
            CompositionTarget.Rendering += Update;
            this.oldDateTime = DateTime.Now;                //倒數計時開始，時間設置
            this.backTime_Temp = this.backTime;


            this.Start();                                   //遊戲初始化設定(只執行一次)
            //this.label1.Content = "語音識別裝置已就緒";
            this.SpeechStatus.Text = "";
            this.readyTimer.Stop();
            this.readyTimer = null;
        }



        private int ColorRandomNumber;
        private void RandomWordButton_Click(object sender, RoutedEventArgs e)
        {
            //改變文字
            var num = this.random.Next(0, words.Count());
            this.ColorWordText.Text = this.words[num];
            this.textBlock1.Text = num.ToString();

            //改變文字顏色
            this.ColorRandomNumber = this.random.Next(0, colors.Count());
            this.ColorWordText.Foreground = (Brush)new BrushConverter().ConvertFrom(this.colors[this.ColorRandomNumber]);
            this.textBlock2.Text = this.ColorRandomNumber.ToString();
            //this.ColorWordText.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255));

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("HomeMenu.xaml", UriKind.Relative));
        }

        
    }
}
