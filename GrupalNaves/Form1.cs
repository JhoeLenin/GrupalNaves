using System;
using System.Drawing;
using System.Windows.Forms;

namespace GrupalNaves
{
    public partial class Form1 : Form
    {
        private Naves naveJugador;
        private TipoAvion avionSeleccionado = TipoAvion.Avion1; // Valor por defecto

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Para evitar parpadeo
            this.ClientSize = new Size(800, 600);
            this.Text = "Visualizador de Naves";

            // Mostrar menú de selección al iniciar
            MostrarMenuSeleccion();

            // Configurar el evento Paint
            this.Paint += DibujarNave;
        }

        private void MostrarMenuSeleccion()
        {
            using (var menu = new Form())
            {
                menu.FormBorderStyle = FormBorderStyle.FixedDialog;
                menu.StartPosition = FormStartPosition.CenterScreen;
                menu.Text = "Selecciona tu nave";
                menu.Width = 300;
                menu.Height = 200;

                var label = new Label()
                {
                    Text = "Elige tu nave:",
                    Left = 50,
                    Top = 20,
                    Width = 200
                };

                var comboBox = new ComboBox()
                {
                    Left = 50,
                    Top = 50,
                    Width = 200
                };

                // Agregar opciones de aviones
                foreach (TipoAvion tipo in Enum.GetValues(typeof(TipoAvion)))
                {
                    comboBox.Items.Add(tipo.ToString());
                }
                comboBox.SelectedIndex = 0;

                var botonAceptar = new Button()
                {
                    Text = "Escoger nave",
                    Left = 50,
                    Top = 100,
                    Width = 200,
                    Height = 30 // Ajusta la altura del boton a
                };

                botonAceptar.Click += (s, e) =>
                {
                    avionSeleccionado = (TipoAvion)comboBox.SelectedIndex;
                    InicializarNave();
                    menu.Close();
                    this.Invalidate(); // Redibujar el formulario
                };

                menu.Controls.Add(label);
                menu.Controls.Add(comboBox);
                menu.Controls.Add(botonAceptar);

                menu.ShowDialog();
            }
        }

        private void InicializarNave()
        {
            // Crear la nave del tipo seleccionado
            naveJugador = new Naves(avionSeleccionado)
            {
                PosX = this.ClientSize.Width / 2,
                PosY = this.ClientSize.Height / 2,
                Escala = 2.0f,
                AnguloRotacion = 0f
            };
        }

        private void DibujarNave(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            // Dibujar la nave del jugador si existe
            naveJugador?.Dibujar(g, naveJugador.Escala);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Opcional: Inicializar la nave por defecto al cargar
            // InicializarNave();
        }
    }
}