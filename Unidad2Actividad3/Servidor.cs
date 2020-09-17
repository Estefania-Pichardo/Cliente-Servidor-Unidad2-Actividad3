using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;

namespace Unidad2Actividad3
{
    public class Servidor : INotifyPropertyChanged
    {
        HttpListener listener;

        Dispatcher dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Equipo1 { get; set; }
        public string Equipo2 { get; set; }

        public int Marcador1 { get; set; }
        public int Marcador2 { get; set; }

        public Servidor()
        {
            listener = new HttpListener();
            dispatcher = Dispatcher.CurrentDispatcher;

            listener.Prefixes.Add("http://*:80/actividad3/");
            listener.Start();
            listener.BeginGetContext(OnRequest, null);

        }

        private void OnRequest(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(OnRequest, null);

            if (context.Request.Url.LocalPath == "/actividad3/" || context.Request.Url.LocalPath == "/actividad3")
            {
                var buffer = File.ReadAllBytes("index.html");
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                context.Response.StatusCode = 200;
            }
            if (context.Request.Url.LocalPath == "/actividad3/cambiarDatos" && context.Request.HttpMethod == "GET")
            {
                if (context.Request.QueryString["equipo1"] != "" && context.Request.QueryString["equipo2"] != ""
                    && context.Request.QueryString["goles1"] != "" && context.Request.QueryString["goles2"] != "")
                {
                    var nombre1 = context.Request.QueryString["equipo1"];
                    var nombre2 = context.Request.QueryString["equipo2"];

                    var m1 = int.Parse(context.Request.QueryString["goles1"]);
                    var m2 = int.Parse(context.Request.QueryString["goles2"]);

                    ModificarDatos(nombre1, nombre2, m1, m2);

                    context.Response.StatusCode = 200;
                    context.Response.Redirect("/actividad3");
                }
                else if (context.Request.QueryString["equipo1"] == "" && context.Request.QueryString["equipo2"] == ""
                && context.Request.QueryString["goles1"] != "" && context.Request.QueryString["goles2"] != "")
                {
                    var m1 = int.Parse(context.Request.QueryString["goles1"]);
                    var m2 = int.Parse(context.Request.QueryString["goles2"]);

                    ModificarMarcador(m1, m2);
                    context.Response.StatusCode = 200;
                    context.Response.Redirect("/actividad3");

                }
                else if (context.Request.QueryString["equipo1"] == "" && context.Request.QueryString["equipo2"] == ""
                 && context.Request.QueryString["goles1"] == "" && context.Request.QueryString["goles2"] == "")
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "No puedes dejar los datos en blanco";
                    context.Response.OutputStream.Close();
                }

            }
            else
            {
                context.Response.StatusCode = 404;

            }


            context.Response.Close();

        }

        public void OnPropertyChanged(string property)
        {
            if (property != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public void ModificarDatos(string eq1, string eq2, int m1, int m2)
        {
            this.dispatcher.Invoke(() => Equipo1 = eq1);
            this.dispatcher.Invoke(() => Equipo2 = eq2);
            this.dispatcher.Invoke(() => Marcador1 = m1);
            this.dispatcher.Invoke(() => Marcador2 = m2);

            OnPropertyChanged("Equipo1");
            OnPropertyChanged("Equipo2");
            OnPropertyChanged("Marcador1");
            OnPropertyChanged("Marcador2");
        }

        public void ModificarMarcador(int m1, int m2)
        {
            this.dispatcher.Invoke(() => Marcador1 = m1);
            this.dispatcher.Invoke(() => Marcador2 = m2);
            OnPropertyChanged("Marcador1");
            OnPropertyChanged("Marcador2");

        }
    }
}
