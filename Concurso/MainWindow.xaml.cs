using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YoutubeExtractor;

namespace Concurso
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> Participantes = new List<string> { "Asier y Aura", "Alvaro y Arrate", "Gorka y Paola", "Solar y Laura" };
        int Turno;
        int Ronda;

        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            ListOrden.ItemsSource = Participantes;
            DataContext = this;
            Turno = 0;
            Ronda = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Participantes = Participantes.OrderBy(x => rnd.Next()).ToList();

            ListOrden.ItemsSource = Participantes;

            ListOrden.SelectedIndex = 0;
            NuevoTurno();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ListOrden.SelectedIndex = (ListOrden.SelectedIndex + 1) % Participantes.Count;

            NuevoTurno();
        }

        private void NuevoTurno()
        {
            Ronda = Convert.ToInt32((Turno++) / 4) + 1;
            MessageBox.Show("Ronda " + Ronda + ": Turno de " + ListOrden.SelectedItem);

            switch (rnd.Next(3))
            {
                case 0:
                    btnPregunta.IsEnabled = true;
                    btnGoogle.IsEnabled = false;
                    btnRueda.IsEnabled = false;
                    btnGuango.IsEnabled = true;
                    break;
                case 1:
                    btnPregunta.IsEnabled = false;
                    btnGoogle.IsEnabled = true;
                    btnRueda.IsEnabled = false;
                    btnGuango.IsEnabled = true;
                    break;
                case 2:
                    btnPregunta.IsEnabled = false;
                    btnGoogle.IsEnabled = false;
                    btnRueda.IsEnabled = true;
                    btnGuango.IsEnabled = true;
                    break;
                default:
                    MessageBox.Show("Error de uga, no se lo tomeis en cuenta.");
                    break;
            }

        }

        private void btnGoogle_Click(object sender, RoutedEventArgs e)
        {
            int CartaNum = rnd.Next(0);

            switch (CartaNum)
            {
                case 0:
                    MessageBox.Show("Carta dámelo: Te quedas con los puntos de ....");
                    MessageBox.Show(getRestoParticipantes().ElementAt(rnd.Next(3)));
                    break;

            }
        }

        private List<string> getRestoParticipantes()
        {
            return Participantes.Where(x => x != ListOrden.SelectedItem.ToString()).ToList();
        }

        private void btnGuango_Click(object sender, RoutedEventArgs e)
        {
            string link = "https://www.youtube.com/watch?v=XOPrrTRvOh4";

            /*
             * Get the available video formats.
             * We'll work with them in the video and audio download examples.
             */
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
            /*
                * We want the first extractable video with the highest audio quality.
            */
            VideoInfo video = videoInfos
                .Where(info => info.CanExtractAudio)
                .OrderByDescending(info => info.AudioBitrate)
                .First();

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the audio downloader.
             * The first argument is the video where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */
            var audioDownloader = new AudioDownloader(video, System.IO.Path.Combine("D:/Downloads", video.Title + video.AudioExtension));

            // Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
            // because the download will take much longer than the audio extraction.
            audioDownloader.DownloadProgressChanged += (tmpsender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
            audioDownloader.AudioExtractionProgressChanged += (tmpsender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

            /*
             * Execute the audio downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            audioDownloader.Execute();
        }


    }
}
