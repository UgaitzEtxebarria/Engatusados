using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Concurso
{
    public class Participante : INotifyPropertyChanged
    {
        private string _nombre;
        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (_nombre == value) return;
                _nombre = value;
                RaisePropertyChanged("Nombre");
            }
        }
        public int _orden;
        public int Orden
        {
            get { return _orden; }
            set
            {
                if (_orden == value) return;
                _orden = value;
                RaisePropertyChanged("Orden");
            }
        }
        public int _puntos;
        public int Puntos
        {
            get { return _puntos; }
            set
            {
                if (_puntos == value) return;
                _puntos = value;
                RaisePropertyChanged("Puntos");
            }
        }

        public int _bonificador;
        public int Bonificador
        {
            get { return _bonificador; }
            set
            {
                if (_bonificador == value) return;
                _bonificador = value;
                RaisePropertyChanged("Bonificador");
            }
        }

        public Participante(string Nombre, int Puntos)
        {
            Bonificador = 0;
            this.Nombre = Nombre;
            this.Puntos = Puntos;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }
        public class ProcessComparer : IComparer<DictionaryEntry>
        {
            public int Compare(DictionaryEntry entry1, DictionaryEntry entry2)
            {
                Participante ptX = (Participante)entry1.Value;
                Participante ptY = (Participante)entry2.Value;

                return ptX.Orden - ptY.Orden;
            }
        }
    }
}
