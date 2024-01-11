using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace VideoPlayer3000
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Deklarace pracovnich promennych
        bool hraje;
        double vol;
        bool dark;
        Color color;
        TimeSpan _position;
        DispatcherTimer _timer = new DispatcherTimer();
        private WindowState originalWindowState;
        private double originalWidth;
        private double originalHeight;
        double rychlost;

        public MainWindow()
        {
            InitializeComponent();

            // Nastaveni casovace
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(ticktock);
            _timer.Start();

            // Defaultni hlasitost a rychlost
            vol = 1;
            rychlost = 1.0;
            speedTextBox.Text = $"{rychlost}×";

            // Defaultni barvy
            color = Color.FromRgb(28, 37, 25);
            Background = new SolidColorBrush(color);
            dark = true;
            nameLabel.Foreground = new SolidColorBrush(Color.FromRgb(220,230,30));
            speedLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 230, 30));
            volumeLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 230, 30));
            nameLabel.Content = $"{System.IO.Path.GetFileNameWithoutExtension(MediaPathTextBox.Text)}";

            // Text napovedy
            helpTextBlock.Text = "Toto je nápověda ke klávesovému ovládání.\n\n" +
                "Obecné\n" +
                $"{"Načtení videa",-30}Enter\n" +
                $"{"Spuštění/pozastavení videa",-30}K | Space | MediaPlayPause\n" +
                $"{"Stop/Restart",-30}R | MediaStop\n" +
                $"{"Fullscreen",-30}F | Escape (ukončit)\n" +
                "\nZvuk\n" +
                $"{"Zesílení",-30}Šipka nahoru (↑)\n" +
                $"{"Zeslabení",-30}Šipka dolů (↓)\n" +
                $"{"Ztlumení",-30}M\n" +
                "\nČasová osa\n" +
                $"{"Přeskočení (+10s)",-30}Šipka doprava (→)\n" +
                $"{"Vrácení (-10s)",-30}Šipka doleva (←)\n" +
                $"{"Rychlost přehrávání",-30}D1 (0.5×) | D2 (1.0×) | D3 (1.5×)\n" +
                $"{"                   ",-30}D4 (2.0×) | D5 (2.5×) | D6 (3.0×)\n" +
                "\nPokročilé\n" +
                $"{"Tmavý režim on/off",-30}L\n" +
                $"{"Skrytí ovládacích prvků",-30}H\n" +
                $"{"Přidání poznámky",-30}N\n" +
                $"{"Nápověda",-30}I | OemComma \",\"\n" +
                "\nPřejeme Vám příjemné sledování. <3";
        }

        /// <summary>
        /// Casovac
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ticktock(object sender, EventArgs e)
        {
            if (!timeSlider.IsMouseCaptureWithin)
            {
                timeSlider.Value = VideoBox.Position.TotalSeconds;
            }
            timeTextBox.Text = VideoBox.Position.ToString();
        }

        /// <summary>
        /// Nazev mluvi za vse...
        /// </summary>
        private void PlayPause()
        {
            if (hraje == false)
            {
                VideoBox.SpeedRatio = rychlost;
                VideoBox.Play();
                hraje = true;
            }
            else
            {
                VideoBox.SpeedRatio = 1; // Tento krok je z neznameho duvodu nezbytny.
                VideoBox.Pause();
                hraje = false;
            }
        }

        /// <summary>
        /// Nastavi okno do rezimu fullscreen a zpet do puvodniho.
        /// </summary>
        private void ToggleFullscreen()
        {
            if (WindowState == WindowState.Normal)
            {
                // Puvodni hodnoty
                originalWidth = Width; 
                originalHeight = Height;
                originalWindowState = WindowState;
                
                // Fullscreen
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                // Nazpet puvodni hodnoty
                WindowState = originalWindowState;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
                Width = originalWidth;
                Height = originalHeight;
            }
        }

        /// <summary>
        /// Zmeni cestu k videu na cestu ke stejnojmenemu textovemu souboru s poznamkami
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="Extension"></param>
        /// <returns></returns>
        static string ChangeFileExtension(string filePath, string Extension)
        {
            // spolecna cesta
            string directory = System.IO.Path.GetDirectoryName(filePath);
            
            // jmeno souboru
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            
            // cesta k novemu souboru
            string newFilePath = System.IO.Path.Combine(directory, $"{fileName}.{Extension}");

            return newFilePath;
        }

        /// <summary>
        /// Ovlada otevreni a zavreni Popupu
        /// </summary>
        private void PopupControl()
        {
            if (helpPopup.IsOpen)
            {
                helpPopup.IsOpen = false;
            }
            else
            {
                helpPopup.IsOpen = true;
            }
        }


        /// <summary>
        /// Prizpusobeni slideru na dane video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoBox_MediaOpened(object sender, RoutedEventArgs e)
        {
            _position = VideoBox.NaturalDuration.TimeSpan;
            timeSlider.Minimum = 0;
            timeSlider.Maximum = _position.TotalSeconds;
        }

        /// <summary>
        /// Opraveni ovladani hlasitosti pomoci slideru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSlider.IsMouseCaptureWithin)
            {
                VideoBox.Volume = volumeSlider.Value;
            }
        }

        // Zmena pozice jezdce prehravace
        // Kombinace techto dvou metod vychazi z rady na Stackoverflow
        private void timeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(timeSlider.Value);
            VideoBox.Position = new TimeSpan(0, 0, 0, pos, 0);
        }
        private void timeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timeSlider.IsMouseCaptureWithin)
            {
                int pos = Convert.ToInt32(timeSlider.Value);
                VideoBox.Position = new TimeSpan(0, 0, 0, pos, 0);
            }
        }

        /// <summary>
        /// Otevre dialogove okno pro vyber souboru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"c:\";
            ofd.ShowDialog();
            MediaPathTextBox.Text = ofd.FileName;
            nameLabel.Content = $"{System.IO.Path.GetFileNameWithoutExtension(MediaPathTextBox.Text)}";
            /*
            // pridano na prani testeru
            try
            {
                VideoBox.Source = new Uri(MediaPathTextBox.Text);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Nebyl vybrán žádný soubor nebo je vybrán neplatný soubor.\n" + 
                               "({ex.Message})", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        /// <summary>
        /// Nahraje vybrane video.
        /// Stopne video.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OmniButton_Click(object sender, RoutedEventArgs e)
        {
            hraje = false;
            if (MediaPathTextBox.Text.Length <= 0)
            {
                MessageBox.Show("Zadejte validní soubor!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            VideoBox.Source = new Uri(MediaPathTextBox.Text);
            VideoBox.Stop();
            VideoBox.SpeedRatio = rychlost;
        }

        /// <summary>
        /// Otevre Popup s napovedou.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            PopupControl();
        }

        /// <summary>
        /// Kliknutim na video spusti metodu PlayPause()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayPause();
        }

        /// <summary>
        /// Definuje vsechny klavesove zkratky/prikazy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OknoPrehravace_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                // fulscreen
                case Key.F:
                    ToggleFullscreen();
                    break;
                case Key.Escape:
                    if (WindowState == WindowState.Maximized)
                    { ToggleFullscreen(); }
                    break;

                // schovani ovladani
                case Key.H:
                    if (HeaderBorder.Height > 0)
                    {
                        HeaderBorder.Height = 0;
                        FooterBorder.Height = 0;
                    }
                    else
                    {
                        HeaderBorder.Height = 57;
                        FooterBorder.Height = 70;
                    }
                    break;

                // Play Pause
                case Key.Space:
                    PlayPause();
                    break;
                case Key.MediaPlayPause:
                    PlayPause();
                    break;
                case Key.K:
                    PlayPause();
                    break;

                // nacteni souboru ENTERem
                case Key.Return:
                    if (MediaPathTextBox.Text.Length <= 0)
                    {
                        MessageBox.Show("Zadejte validní soubor!");
                        return;
                    }
                    VideoBox.Source = new Uri(MediaPathTextBox.Text);
                    break;

                // posun videa dozadu
                case Key.Left:
                    timeSlider.Value -= 10;
                    int posl = Convert.ToInt32(timeSlider.Value);
                    VideoBox.Position = new TimeSpan(0, 0, 0, posl, 0);
                    break;

                // posun videa dopredu
                case Key.Right:
                    timeSlider.Value += 10;
                    int posh = Convert.ToInt32(timeSlider.Value);
                    VideoBox.Position = new TimeSpan(0, 0, 0, posh, 0);
                    break;

                // snizeni hlasitosti
                case Key.Down:
                    volumeSlider.Value -= volumeSlider.SmallChange;
                    VideoBox.Volume = volumeSlider.Value;
                    break;

                // zvyseni hlasitosti
                case Key.Up:
                    volumeSlider.Value += volumeSlider.SmallChange;
                    VideoBox.Volume = volumeSlider.Value;
                    break;

                // vyuziti klavesy STOP
                case Key.MediaStop:
                    VideoBox.Stop();
                    break;
                case Key.R:
                    VideoBox.Stop();
                    break;
                
                // nastaveni barevneho rezimu    
                case Key.L:
                    // svetly rezim
                    if (dark == true)
                    {
                        color = Color.FromRgb(224,224,224);
                        Background = new SolidColorBrush(color);
                        nameLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        speedLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        volumeLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                        dark = false;
                    }
                    // tmavy rezim
                    else
                    {
                        color = Color.FromRgb(28,37,25);
                        Background = new SolidColorBrush(color);
                        nameLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 230, 30));
                        speedLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 230, 30));
                        volumeLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 230, 30));
                        dark = true;
                    }
                    break;

                // ztlumeni zvuku
                case Key.M:
                    if (VideoBox.Volume > 0)
                    {
                        vol = VideoBox.Volume;
                        VideoBox.Volume = 0;
                        volumeSlider.Value = 0;
                    }
                    else
                    {
                        VideoBox.Volume = vol;
                        volumeSlider.Value = vol;
                    }
                    break;

                // zapsani poznamky
                case Key.N:
                    if (hraje == true)
                    {
                        VideoBox.SpeedRatio = 1;
                        VideoBox.Pause();
                        hraje = false;
                    }

                    Notebook notebook = new Notebook();
                    bool? result = notebook.ShowDialog();

                    if (result == true)
                    {
                        string note = notebook.Poznamka;

                        try
                        {
                            string filePath = ChangeFileExtension(MediaPathTextBox.Text,"txt");
                            using (StreamWriter writer = File.AppendText(filePath))
                            {
                                writer.WriteLine($"{timeTextBox.Text}\t{note}");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Vyskytla se chyba při zápisu do souboru: {ex.Message}" +
                                $"\nProsím, zkuste to znovu.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;

                // popup napovedy
                case Key.OemComma:
                    PopupControl();
                    break;
                case Key.I:
                    PopupControl();
                    break;

                // rychlost prehravani
                case Key.D1:
                    rychlost = 0.5;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                case Key.D2:
                    rychlost = 1.0;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                case Key.D3:
                    rychlost = 1.5;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                case Key.D4:
                    rychlost = 2.0;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                case Key.D5:
                    rychlost = 2.5;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                case Key.D6:
                    rychlost = 3.0;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                case Key.D0:
                    rychlost = 8.0;
                    VideoBox.SpeedRatio = rychlost;
                    speedTextBox.Text = $"{rychlost}×";
                    break;
                default:
                    break;
            }

            //// Kontrola klaves
            // MessageBox.Show($"Stiskl jsi: {e.Key}");
        }

    }
}
