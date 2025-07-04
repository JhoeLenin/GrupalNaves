﻿using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrupalNaves
{
    public enum TipoAvion
    {
        Avion1,
        Avion2,
        Avion3,
        Avion4,
        Avion5
    }

    internal class Naves
    {
        private static string BasePath = Path.GetFullPath(
            Path.Combine(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName,
            "Assets",
            "Naves"
            )
        );
        private readonly string rutaBordes;
        private readonly string rutaColoreados;

        public int PosX { get; set; }
        public int PosY { get; set; }
        public float Escala { get; set; } = 1.0f;
        public float AnguloRotacion { get; set; } = 0f;
        public TipoAvion Tipo { get; private set; }

        // Constructor que recibe el tipo de avión
        public Naves(TipoAvion tipo)
        {
            Tipo = tipo;
            string carpetaAvion = tipo.ToString();

            rutaBordes = Path.Combine(BasePath, carpetaAvion, "bordes.txt");
            rutaColoreados = Path.Combine(BasePath, carpetaAvion, "coloreados.txt");

            // Validar que existan los archivos
            if (!File.Exists(rutaBordes) || !File.Exists(rutaColoreados))
            {
                throw new FileNotFoundException($"Archivos no encontrados para el avión {tipo}");
            }
        }

        public void Dibujar(Graphics g, float Escala)
        {
            var coloreados = LeerColoreados(rutaColoreados);
            var bordes = LeerBordes(rutaBordes);

            // Guardar el estado original del gráfico
            GraphicsState estadoOriginal = g.Save();

            // Aplicar transformaciones
            g.TranslateTransform(PosX, PosY);
            g.RotateTransform(AnguloRotacion);

                // Escala normal para el jugador
            g.ScaleTransform(Escala, Escala);

            // Dibujar coloreados
            foreach (var (color, puntos) in coloreados)
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    foreach (var p in puntos)
                    {
                        g.FillRectangle(brush, p.X, p.Y, 2, 2);
                    }
                }
            }


            // Dibujar bordes
            foreach (var grupo in bordes)
            {
                if (grupo.Count > 1)
                {
                    g.DrawPolygon(Pens.Black, grupo.ToArray());
                }
            }

            // Restaurar el estado original del gráfico
            g.Restore(estadoOriginal);
        }


        private List<(Color color, List<Point> puntos)> LeerColoreados(string ruta)
        {
            var grupos = new List<(Color, List<Point>)>();
            using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                string linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linea) || linea.StartsWith("//")) continue;
                    var partes = linea.Split(' ');
                    var colorPart = partes[0].Split(',');
                    var color = Color.FromArgb(
                        int.Parse(colorPart[0]),
                        int.Parse(colorPart[1]),
                        int.Parse(colorPart[2]));
                    var puntos = partes.Skip(1).Select(p =>
                    {
                        var coords = p.Split(',');
                        return new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                    }).ToList();
                    grupos.Add((color, puntos));
                }
            }
            return grupos;
        }

        private List<List<Point>> LeerBordes(string ruta)
        {
            var grupos = new List<List<Point>>();
            foreach (var linea in File.ReadAllLines(ruta))
            {
                if (string.IsNullOrWhiteSpace(linea) || linea.StartsWith("//")) continue;
                var puntos = linea.Split(' ')
                                  .Select(p =>
                                  {
                                      var coords = p.Split(',');
                                      return new Point(int.Parse(coords[0]), int.Parse(coords[1]));
                                  }).ToList();
                grupos.Add(puntos);
            }
            return grupos;
        }
    }
}
