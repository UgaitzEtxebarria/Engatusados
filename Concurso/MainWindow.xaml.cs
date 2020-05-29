using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using YoutubeExtractor;

namespace Concurso
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<int> Preguntas = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 99 };
        private ObservableCollection<Participante> _participantes = new ObservableCollection<Participante>();
        public ObservableCollection<Participante> Participantes
        {
            get { return _participantes; }
            set
            {
                _participantes = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Participantes"));
            }
        }

        bool AyudaPregunta = true;
        bool AyudaCartaGoogle = true;
        bool AyudaRuedaCaos = true;
        bool AyudaGuango = true;

        int Turno;
        int Ronda;

        Random rnd = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public string Seleccionado { get => ((Participante)ListOrden.SelectedItem).Nombre; }

        public MainWindow()
        {
            Participantes = new ObservableCollection<Participante>();
            Participantes.Add(new Participante("Asier y Aura", 10));
            Participantes.Add(new Participante("Alvaro y Arrate", 10));
            Participantes.Add(new Participante("Gorka y Paola", 10));
            Participantes.Add(new Participante("Solar y Laura", 10));

            InitializeComponent();
            DataContext = this;

            Turno = 0;
            Ronda = 0;
        }

        private void Orden_Click(object sender, RoutedEventArgs e)
        {

            Participantes = new ObservableCollection<Participante>(Participantes.OrderBy(x => rnd.Next()).ToList());

            for (int i = 0; i < Participantes.Count; i++)
                Participantes[i].Orden = i;

            ListOrden.SelectedIndex = 0;
            NuevoTurno();
        }

        private void NextTurn_Click(object sender, RoutedEventArgs e)
        {
            ListOrden.SelectedIndex = (ListOrden.SelectedIndex + 1) % Participantes.Count;

            NuevoTurno();
        }

        private void btnGoogle_Click(object sender, RoutedEventArgs e)
        {
            if (AyudaCartaGoogle)
            {
                MessageBox.Show("La carta google puede darte o quitarte puntos.");
                AyudaCartaGoogle = false;
            }

            int CartaNum = rnd.Next(4);
            string target;
            switch (CartaNum)
            {
                case 0:
                    MessageBox.Show("Carta \"Dámelo\": Te quedas con los puntos de ....");
                    target = getRestoParticipantes().ElementAt(rnd.Next(3)).Nombre;
                    MessageBox.Show(target);
                    DarPuntos(target, -1, Seleccionado);
                    break;
                case 1:
                    MessageBox.Show("Carta \"Toma esa!\": Regalas todos tus puntos a ....");
                    target = getRestoParticipantes().ElementAt(rnd.Next(3)).Nombre;
                    MessageBox.Show(target);
                    DarPuntos(Seleccionado, -1, target);
                    break;
                case 2:
                    MessageBox.Show("Carta \"Bolsa de revueltos con pistachos\": Pierdas 5 dientes/puntos!");
                    SumaPuntos(Seleccionado, -5);
                    break;
                case 3:
                    MessageBox.Show("Carta \"Pizza time!\": Ganas 5 puntos");
                    SumaPuntos(Seleccionado, 5);
                    break;
            }
        }

        private void btnGuango_Click(object sender, RoutedEventArgs e)
        {
            if (AyudaGuango)
            {
                MessageBox.Show("La carta Guango Travieso determina si vas más alto o más bajo.");
                AyudaGuango = false;
            }

            int tmpBon = ((rnd.Next(0, 2) == 0) ? 1 : -1) * rnd.Next(1, 6);
            Participantes[getPart(Seleccionado).Orden].Bonificador += tmpBon;
            MessageBox.Show("Tu bonificador/Penalizador obtiene " + tmpBon);
            if (Participantes[getPart(Seleccionado).Orden].Bonificador > 0)
                MessageBox.Show("Parece que vas más alto por que tu bonificador está en " + Participantes[getPart(Seleccionado).Orden].Bonificador);
            if (Participantes[getPart(Seleccionado).Orden].Bonificador == 0)
                MessageBox.Show("Parece que no vas más alto ni mas bajo por que tu bonificador está en " + Participantes[getPart(Seleccionado).Orden].Bonificador);
            if (Participantes[getPart(Seleccionado).Orden].Bonificador < 0)
                MessageBox.Show("Parece que vas más bajo por que tu bonificador está en " + Participantes[getPart(Seleccionado).Orden].Bonificador);
        }

        private void reproducir()
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


        private void Pregunta_Click(object sender, RoutedEventArgs e)
        {
            if (AyudaPregunta)
            {
                MessageBox.Show("Acertando las preguntas ganareis puntos. Puede que haya algun plus!!");
                AyudaPregunta = false;
            }

            MessageBoxResult result;
            bool plus = HayPlus(out int plusNum);

            int Preguntanum = (plusNum == -1) ? Preguntas.OrderBy(x => rnd.Next()).First() : plusNum;

            switch (Preguntanum)
            {
                case 0:
                    Debug.WriteLine("Respuesta: Año 1969");
                    result = MessageBox.Show("¿En qué año el hombre pisó la Luna por primera vez?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 1:
                    Debug.WriteLine("Respuesta: Estados Unidos y Rusia");
                    result = MessageBox.Show("¿Entre qué países podemos encontrar el Estrecho de Bering?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 2:
                    Debug.WriteLine("Respuesta: Los montes Urales");
                    result = MessageBox.Show("¿Qué cordillera separa Europa de Asia?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 3:
                    Debug.WriteLine("Respuesta: Manila");
                    result = MessageBox.Show("¿Cuál es la capital de Filipinas?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 4:
                    Debug.WriteLine("Respuesta: Ulan Bator");
                    result = MessageBox.Show("¿Cuál es la capital de Mongolia?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 5:
                    Debug.WriteLine("Respuesta: El Cairo (casi 20 millones de habitantes)");
                    result = MessageBox.Show("¿Cuál es la ciudad más poblada de África?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 6:
                    Debug.WriteLine("Respuesta: Tirana");
                    result = MessageBox.Show("¿Cuál es la capital de Albania?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 7:
                    Debug.WriteLine("Respuesta: Clint Eastwood");
                    result = MessageBox.Show("¿Qué actor, que no era el feo ni el malo, era el bueno?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 8:
                    Debug.WriteLine("Respuesta: Hendaya");
                    result = MessageBox.Show("¿En qué ciudad se entrevistaron Franco y Hitler?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 9:
                    Debug.WriteLine("Respuesta: Caravinieri");
                    result = MessageBox.Show("¿Cómo se conoce a la policía italiana?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 99:
                    Debug.WriteLine("Respuesta: Once Upon a Time in Hollywood (Hollywood in Time a Upon Once) / Erase una vez en hollywood (hollywood en vez una erase)");
                    result = MessageBox.Show("¿Cómo se conoce a la policía italiana?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                default:
                    result = MessageBoxResult.No;
                    break;
            }

            if (result == MessageBoxResult.Yes)
                SumaPuntos(Seleccionado, 3 + (plus ? 2 : 0));

            Preguntas.Remove(Preguntanum);
        }

        private bool HayPlus(out int pregunta)
        {
            pregunta = -1;
            if (rnd.Next(2) == 1)
            {
                int PlusNum = rnd.Next(6);
                switch (PlusNum)
                {
                    case 0:
                        MessageBox.Show("Hay plus por saltar!");
                        break;
                    case 1:
                        MessageBox.Show("Esta será difícil, contenga la respiración.");
                        break;
                    case 2:
                        MessageBox.Show("A girar como si te hubieses bebido una Chang!");
                        break;
                    case 3:
                        MessageBox.Show("Poned cara de resaca de asier");
                        break;
                    case 4:
                        MessageBox.Show("A por el papel de culo!");
                        break;
                    case 5:
                        MessageBox.Show("Hay un plus por decirlo al reves! (Palabras de atrás hacia delante!)");
                        pregunta = 99;
                        break;
                }
                return true;
            }
            return false;
        }

        private void RuedaCaos_Click(object sender, RoutedEventArgs e)
        {
            if (AyudaRuedaCaos)
            {
                MessageBox.Show("La rueda del caos te puede maldecir o bendecir con objetos");
                AyudaRuedaCaos = false;
            }

            int RuedaNum = rnd.Next(10);

            switch (RuedaNum)
            {
                case 0:
                    MessageBox.Show("Habeis ganado un \"Pase de Ángel\", os da otro turno cuando falleis");
                    break;
                case 1:
                    MessageBox.Show("Habeis ganado un \"Basauritarra\", os da la opcion de robarle el turno a otro participante");
                    break;
                case 2:
                    MessageBox.Show("Habeis ganado un \"Kebab mañanero de domingo\", perdeis el próximo turno por ir en barco");
                    break;
                case 3:
                    MessageBox.Show("Habeis ganado un \"Rayo de escarcha de hechicero\", puedes congelar el turno de otro participante de la próxima ronda");
                    break;
                case 4:
                    MessageBox.Show("Habeis ganado un \"Jefe navarro\", la próxima ronda obtienes doble turno. Y pagandoles la mitad de sueldo a tus trabajadores!");
                    break;
                case 5:
                    MessageBox.Show("Respuesta: El Cairo (casi 20 millones de habitantes)");
                    break;
                case 6:
                    MessageBox.Show("Respuesta: Tirana");
                    break;
                case 7:
                    MessageBox.Show("Respuesta: Clint Eastwood");
                    break;
                case 8:
                    MessageBox.Show("Respuesta: Hendaya");
                    break;
                case 9:
                    MessageBox.Show("Respuesta: Caravinieri");
                    break;
                default:
                    MessageBox.Show("Errorcito de uga!! Se ha ganado una cerveza!");
                    break;
            }
        }

        ////////////////////////////////////////////
        private List<Participante> getRestoParticipantes()
        {
            return Participantes.Where(x => x.Nombre != Seleccionado).ToList();
        }

        private void SumaPuntos(string target, int points)
        {
            Participantes[getPart(target).Orden].Puntos += points + Participantes[getPart(target).Orden].Bonificador;
        }

        private void DarPuntos(string source, int points, string target)
        {
            int puntos = (points == -1) ? getPart(source).Puntos : points;

            Participantes[getPart(source).Orden].Puntos -= puntos;
            Participantes[getPart(target).Orden].Puntos += puntos;

        }

        public Participante getPart(String nombre)
        {
            return Participantes.Where(x => x.Nombre == nombre).First();
        }

        private void NuevoTurno()
        {
            Ronda = Convert.ToInt32((Turno++) / 4) + 1;
            MessageBox.Show("Ronda " + Ronda + ": Turno de " + Seleccionado);

            CalcularBonificador();

            switch (rnd.Next(3))
            {
                case 0:
                    btnPregunta.IsEnabled = Preguntas.Count > 0;
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

        private void CalcularBonificador()
        {
            if (Participantes[getPart(Seleccionado).Orden].Bonificador > 0)
                Participantes[getPart(Seleccionado).Orden].Bonificador--;
            else if (Participantes[getPart(Seleccionado).Orden].Bonificador < 0)
                Participantes[getPart(Seleccionado).Orden].Bonificador++;
        }
        ////////////////////////////////////////////
    }
}
