using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using YoutubeExtractor;

namespace Concurso
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<int> Preguntas = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 99 };
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

            ListOrden.MouseDoubleClick += new MouseButtonEventHandler(ListBox1_MouseDoubleClick);

            Turno = 0;
            Ronda = 0;
        }

        private void ListBox1_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (ListOrden.SelectedItem != null)
            {
                ((Participante)ListOrden.SelectedItem).Puntos = Convert.ToInt32(txtPuntos.Text);
            }

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
                case 10:
                    Debug.WriteLine("Respuesta: Tiburon ballena");
                    result = MessageBox.Show("¿Cual es el tiburon más grande del mundo?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 11:
                    Debug.WriteLine("Respuesta: Ben Franklin");
                    result = MessageBox.Show("¿Quien invento los anteojos?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 12:
                    Debug.WriteLine("Respuesta: Reina Isabel II");
                    result = MessageBox.Show("¿Quien ha reinado gran bretaña por mas tiempo?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 13:
                    Debug.WriteLine("Respuesta: Así es como se les llama a las personas originarias de Río de Janeiro, en Brasil");
                    result = MessageBox.Show("¿A qué personas se las conoce como cariocas?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 14:
                    Debug.WriteLine("Respuesta: El edificio Burj Khalifa, situado en Dubai, es el edificio más alto del mundo con 828 metros");
                    result = MessageBox.Show("¿Cuál es el edificio más alto del mundo?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 15:
                    Debug.WriteLine("Respuesta: Japón fue el país sobre el que cayó la primera bomba atómica. Fue durante la Segunda Guerra Mundial y el impacto se produjo sobre la ciudad de Hiroshima");
                    result = MessageBox.Show("¿En qué país se utilizó la primera bomba atómica en un contexto de combate?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 16:
                    Debug.WriteLine("Respuesta: El nombre que recibe la agencia de inteligencia inglesa es MI5");
                    result = MessageBox.Show("¿Cómo se llama la agencia de inteligencia británica?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 17:
                    Debug.WriteLine("Respuesta: Esta energía es conocida como energía nuclear");
                    result = MessageBox.Show("¿Qué nombre recibe la energía que contiene el núcleo de los átomos?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 18:
                    Debug.WriteLine("Respuesta: El Concorde es un modelo de avión supersónico, que hasta su retirada en 2003 fue el avión de pasajeros comercial más rápido del mundo");
                    result = MessageBox.Show("¿Qué era el Concorde?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 19:
                    Debug.WriteLine("Respuesta: El desierto de Lut, en Irán, es donde se ha detectado la temperatura más alta jamás registrada en la Tierra. En una zona del desierto conocida como Gandom Beryan se llegó a los 70,7 grados centígrados");
                    result = MessageBox.Show("¿Cuál es el lugar más caluroso del planeta?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 20:
                    Debug.WriteLine("Respuesta: El gusto es uno de nuestros cinco sentidos. Los sabores primarios son dulce, amargo, ácido, salado y umami");
                    result = MessageBox.Show("¿Cuáles son los cinco tipos de sabores primarios?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 21:
                    Debug.WriteLine("Respuesta: El padre del psicoanálisis es Sigmund Freud");
                    result = MessageBox.Show("¿Quién es el padre del psicoanálisis?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 22:
                    Debug.WriteLine("Respuesta: En Guatemala, el producto que más se cultiva es el café");
                    result = MessageBox.Show("¿Qué producto cultiva más Guatemala?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 23:
                    Debug.WriteLine("Respuesta: El ahora jugador del Manchester United es sueco");
                    result = MessageBox.Show("¿De qué país es el futbolista Zlatan Ibrahimović?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 24:
                    Debug.WriteLine("Respuesta: La estación espacial rusa recibe el nombre de Mir");
                    result = MessageBox.Show("¿Cómo se llama la estación espacial rusa?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 25:
                    Debug.WriteLine("Respuesta: El murciélago es un mamífero que tiene la capacidad de volar");
                    result = MessageBox.Show("¿Cuál es el único mamífero capaz de volar?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 26:
                    Debug.WriteLine("Respuesta: Las monoinsaturadas son grasas insaturadas que se encuentran en el aceite de oliva");
                    result = MessageBox.Show("¿Qué grasas hacen tan saludable el aceite de oliva?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 27:
                    Debug.WriteLine("Respuesta: Este famosos tema es del cantante Eric Clapton");
                    result = MessageBox.Show("¿Qué veterano músico es la canción \"Tears in Heaven\"?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 28:
                    Debug.WriteLine("Respuesta: Esta fecha emblemática es el 6 de enero");
                    result = MessageBox.Show("¿Qué día celebran los cristianos la festividad de la Epifanía?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 29:
                    Debug.WriteLine("Respuesta: Aunque algunos piensen que es el oro o el platino, en realidad es el rodio");
                    result = MessageBox.Show("¿Cuál es el metal más caro del mundo?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 30:
                    Debug.WriteLine("Respuesta: La frase se atribuye a Sócrates, pero fue Platón quien la recogió por primera vez, pues su autor no dejó testimonio escrito");
                    result = MessageBox.Show("¿Quién “sabía que no sabía nada?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 31:
                    Debug.WriteLine("Respuesta: António Guterres es el secretario general de la ONU en sustitución de Ban Ki Moon");
                    result = MessageBox.Show("¿Quién es el secretario general de la Organización de Naciones Unidas (ONU)?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 32:
                    Debug.WriteLine("Respuesta: En dos regiones muy alejadas de África y del cercano oriente: Indonesia y la India");
                    result = MessageBox.Show("¿Cuáles son los dos países con mayor cantidad de musulmanes?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 33:
                    Debug.WriteLine("Respuesta: António Guterres es el secretario general de la ONU en sustitución de Ban Ki Moon");
                    result = MessageBox.Show("¿Quién es el secretario general de la Organización de Naciones Unidas (ONU)?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 34:
                    Debug.WriteLine("Respuesta: En el año 1981 apareció la máquina recreativa llamada Donkey Kong, protagonizada por Jumpman, el personaje que poco después, en el año 1985, sería conocido como Mario en el videojuego Super Mario Bros");
                    result = MessageBox.Show("¿En qué año apareció en el mercado el primer videojuego protagonizado por Super Mario?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 35:
                    Debug.WriteLine("Respuesta: Varias especies de cuervo se caracterizan por hablar mejor que los loros");
                    result = MessageBox.Show("¿Cuál es el animal que tiene mayor facilidad para repetir las frases y palabras que escucha?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 36:
                    Debug.WriteLine("Respuesta: Se cree que es el vasco, hablado en una parte de España y de Francia. Dado que es el único idioma de Europa que no tiene una clara relación con ningún otro, los expertos estiman que su origen es anterior incluso al de los pueblos íberos y celtas");
                    result = MessageBox.Show("¿Cuál es el idioma más antiguo de los que sobreviven en Europa?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 37:
                    Debug.WriteLine("Respuesta: Aunque no lo parezca, ese lugar es Australia, lugar en el que estos animales fueron introducidos por el ser humano");
                    result = MessageBox.Show("¿Cuál es el país con más camellos salvajes?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 38:
                    Debug.WriteLine("Respuesta: la medusa Turritopsis nutricula no muere a no ser que la maten o sufra un accidente");
                    result = MessageBox.Show("Existen animales inmortales, ¿Cual?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 39:
                    Debug.WriteLine("Respuesta: La piel es el órgano más grande de nuestro cuerpo");
                    result = MessageBox.Show("¿Cuál es el órgano más grande del cuerpo humano?", "Pregunta", MessageBoxButton.YesNo);
                    break;
                case 40:
                    Debug.WriteLine("Respuesta: Miguel Marcos");
                    result = MessageBox.Show("¿Quien es el marido de Belen Esteban?", "Pregunta", MessageBoxButton.YesNo);
                    break;

                case 99:
                    Debug.WriteLine("Respuesta: Once Upon a Time in Hollywood (Hollywood in Time a Upon Once) / Erase una vez en hollywood (hollywood en vez una erase)");
                    result = MessageBox.Show("¿Cual es la ultima pelicula de Quentin tarantino protagonizada por Brad Pitt y Leonardo DiCaprio?", "Pregunta", MessageBoxButton.YesNo);
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

            int RuedaNum = rnd.Next(6);

            switch (RuedaNum)
            {
                case 0:
                    MessageBox.Show("Habeis ganado un \"Pase de Ángel\", os da otro turno cuando falleis");
                    break;
                case 1:
                    MessageBox.Show("Habeis ganado un \"Basauritarra\", os da la opcion de robarle los puntos a otro participante");
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
                    MessageBox.Show("Habeis ganado un \"Alcalde de Etxebarri\", os da la opcion de robarle el turno a otro participante haciendo ");
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
            Debug.WriteLine(Ronda);
            if (rnd.Next(101) < Ronda)
                MessageBox.Show("Felicidades gente, por que ....." + Seleccionado + " han sido engatusados!");

            CalcularBonificador();

            switch (rnd.Next(6))
            {
                case 0:
                    btnPregunta.IsEnabled = Preguntas.Count > 0;
                    btnGoogle.IsEnabled = true;
                    btnRueda.IsEnabled = false;
                    btnGuango.IsEnabled = false;
                    break;
                case 1:
                    btnPregunta.IsEnabled = Preguntas.Count > 0;
                    btnGoogle.IsEnabled = false;
                    btnRueda.IsEnabled = true;
                    btnGuango.IsEnabled = false;
                    break;
                case 2:
                    btnPregunta.IsEnabled = Preguntas.Count > 0;
                    btnGoogle.IsEnabled = false;
                    btnRueda.IsEnabled = false;
                    btnGuango.IsEnabled = true;
                    break;
                case 3:
                    btnPregunta.IsEnabled = false;
                    btnGoogle.IsEnabled = true;
                    btnRueda.IsEnabled = true;
                    btnGuango.IsEnabled = false;
                    break;
                case 4:
                    btnPregunta.IsEnabled = false;
                    btnGoogle.IsEnabled = true;
                    btnRueda.IsEnabled = false;
                    btnGuango.IsEnabled = true;
                    break;
                case 5:
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
