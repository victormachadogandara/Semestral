using System;
using System.Collections.Generic;
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
///Librerias para multiprocesamientos
using System.Threading; //Se agrego esta libreria para Threading
using System.Diagnostics; // Para el dispatcher y timers 


namespace Semestral
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stopwatch stopwatch; //Toma el tiempo de ejecución del programa
        TimeSpan tiempoAnterior; //Timespan guarda rangos de tiempo
        Random random = new Random();
        List<Enemigos> enemigos = new List<Enemigos>();
        //int puntos = 100;
        
        enum EstadoJuego { GamePlay, GameOver, GameStart, GameWon };
        EstadoJuego estadoActual = EstadoJuego.GameStart;
        enum Direccion { Arriba, Abajo, Derecha, Izquierda, Ninguna };  //Para aclarar la direccion del jugador
        Direccion direccionJugador = Direccion.Ninguna; //Inicializar
        
        double velocidadNave = 300;

        public MainWindow()
        {
            InitializeComponent();
            canvasGamePlay.Focus();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            tiempoAnterior = stopwatch.Elapsed;

            enemigos.Add(new Enemigos(imgMeteorito));
            enemigos.Add(new Enemigos(imgEnemUno));
            enemigos.Add(new Enemigos(imgEnemDos));

            ThreadStart threadStart = new ThreadStart(actualizar);
            //2..Inicializar el Thread - Dar valores e instrucciones
            Thread threadMoverEnemigos = new Thread(threadStart);
            //3..Ejecutar el Thread
            threadMoverEnemigos.Start();

        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            canvasReglas.Visibility = Visibility.Visible;
        }

        private void btnNextReglas_Click(object sender, RoutedEventArgs e)
        {
            canvasReglas.Visibility = Visibility.Collapsed;
            canvasEnemigos.Visibility = Visibility.Visible;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            canvasEnemigos.Visibility = Visibility.Collapsed;
            canvasReglas.Visibility = Visibility.Visible;
        }

        private void btnNextEnemigos_Click(object sender, RoutedEventArgs e)
        {
            canvasEnemigos.Visibility = Visibility.Collapsed;
            canvasGamePlay.Visibility = Visibility.Visible;
            this.estadoActual = EstadoJuego.GamePlay;
            canvasGamePlay.Focus();
        }

       
        void moverjugador(TimeSpan deltaTime)
        {
            double LeftNaveActual = Canvas.GetLeft(imgNave);
            double bottomNaveActual = Canvas.GetTop(imgNave);

            switch (direccionJugador)
            {
                case Direccion.Arriba:

                    double topNaveActual = Canvas.GetTop(imgNave);
                    //Primero el elemento a mover, Luego los valores a mover                  
                    if (bottomNaveActual - (velocidadNave * deltaTime.TotalSeconds) >= 0)
                    {
                        Canvas.SetTop(imgNave, topNaveActual - (velocidadNave * deltaTime.TotalSeconds));
                    }
                    break;

                case Direccion.Abajo:
                   
                    double nuevaPosicion1 = bottomNaveActual + (velocidadNave * deltaTime.TotalSeconds);
                    if (nuevaPosicion1 + imgNave.Width <= 440)
                    {

                        Canvas.SetTop(imgNave, nuevaPosicion1);
                    }
                  

                    break;

                case Direccion.Izquierda: //Para que no salga por la izquierda

                    if (LeftNaveActual - (velocidadNave * deltaTime.TotalSeconds) >= 0)
                    {
                        Canvas.SetLeft(imgNave, LeftNaveActual - (velocidadNave * deltaTime.TotalSeconds));
                    }
                    break;

                case Direccion.Derecha:

                    double nuevaPosicion = LeftNaveActual + (velocidadNave * deltaTime.TotalSeconds);
                    if (nuevaPosicion + imgNave.Width <= 800)
                    {
                        Canvas.SetLeft(imgNave, nuevaPosicion);
                    }

                    break;

                case Direccion.Ninguna:
                    break;
            }
        }
        void actualizar()
        {   //Invoke lleva de parametro una función
            while (true)
            {
                Dispatcher.Invoke(
                 () => //Se creo una función nueva dentro de otra. La => es para indicar que es otra función
                {
                        var tiempoActuali = stopwatch.Elapsed;
                        var deltaTime = tiempoActuali - tiempoAnterior;

                    if (estadoActual == EstadoJuego.GamePlay)
                        {
                        lblScore.Text = (stopwatch.Elapsed.TotalMinutes.ToString() );
                        //moverjugador
                        //Se agrega al parametro utilizado 
                        moverjugador(deltaTime);
                        movimientoEnemigos(deltaTime);
                         //Intersecciones :(
                         foreach (Enemigos enemigos in enemigos)
                         {
                             double xNave = Canvas.GetLeft(imgNave);
                             double yNave = Canvas.GetTop(imgNave);
                             double xEnemigos = Canvas.GetLeft(enemigos.Imagen);                            
                             double yEnemigos = Canvas.GetTop(enemigos.Imagen);
                             if (xEnemigos + enemigos.Imagen.Width >= xNave && xEnemigos <= xNave + imgNave.Width && yEnemigos + enemigos.Imagen.Height >= yNave && yEnemigos <= yNave + imgNave.Height)
                             {

                                //puntos = puntos - 1;

                                //if (puntos == 0) {
                                    estadoActual = EstadoJuego.GameOver;
                                    canvasGamePlay.Visibility = Visibility.Collapsed;
                                    canvasGameOver.Visibility = Visibility.Visible;
                                //}

                                
                             }

                             if (stopwatch.Elapsed.TotalMinutes >= 0.61)
                            {
                                estadoActual = EstadoJuego.GameWon;
                                canvasGamePlay.Visibility = Visibility.Collapsed;
                                canvasWinner.Visibility = Visibility.Visible;
                            }
                         }
                         }
                         tiempoAnterior = tiempoActuali;
                     }
                );
            }
        }

        void movimientoEnemigos(TimeSpan deltaTime)
        {
            double velocidadMet = 300;
            double velocidadNave1 = 300;
            double velocidadNave2 = 300;
            int randomizador = random.Next(10,350);

            velocidadMet += 2 * deltaTime.TotalSeconds;
            velocidadNave1 += .8 * deltaTime.TotalSeconds;
            velocidadNave2 += .6 * deltaTime.TotalSeconds;



            double leftMeteoritoActual = Canvas.GetLeft(imgMeteorito);
            Canvas.SetLeft(imgMeteorito, leftMeteoritoActual - (velocidadMet * deltaTime.TotalSeconds));
            if (Canvas.GetLeft(imgMeteorito) <= -200)
            {
                Canvas.SetTop(imgMeteorito, randomizador);
                Canvas.SetLeft(imgMeteorito, 800);               
            }


            double leftEnem1Actual = Canvas.GetLeft(imgEnemUno);
            // se mueve 120 pixeles por segundo
            Canvas.SetLeft(imgEnemUno, leftEnem1Actual - (velocidadNave1 * deltaTime.TotalSeconds));
            if (Canvas.GetLeft(imgEnemUno) <= -200)
            {
                Canvas.SetTop(imgEnemUno, randomizador);
                Canvas.SetLeft(imgEnemUno, 800);
            }



            double leftEnem2Actual = Canvas.GetLeft(imgEnemDos);
            // se mueve 120 pixeles por segundo
            Canvas.SetLeft(imgEnemDos, leftEnem2Actual - (velocidadNave2 * deltaTime.TotalSeconds));
            if (Canvas.GetLeft(imgEnemDos) <= -200)
            {
                Canvas.SetTop(imgEnemDos, randomizador);
                Canvas.SetLeft(imgEnemDos, 800);
            }


            //foreach (Enemigos enem in enemigos)
            //{
            //    if (Canvas.GetLeft((imgEnemUno)) <= 170 && Canvas.GetLeft(imgEnemUno) >= -169.99)
            //    {
            //        puntos = puntos - 100;
            //        lblScore.Text = puntos.ToString();
            //    }
            //    if (Canvas.GetLeft((imgEnemDos)) <= 170 && Canvas.GetLeft(imgEnemDos) >= -169.99)
            //    {
            //        puntos = puntos - 150;
            //        lblScore.Text = puntos.ToString();
            //    }
            //    if (Canvas.GetLeft((imgMeteorito)) <= 170 && Canvas.GetLeft(imgMeteorito) >= -169.99)
            //    {
            //        puntos = puntos - 400;
            //        lblScore.Text = puntos.ToString();
            //    }
            
            //}

        }

        private void canvasGamePlay_KeyDown(object sender, KeyEventArgs e)
        {
            if (estadoActual == EstadoJuego.GamePlay)
            {
                if (e.Key == Key.Up)
                {
                    direccionJugador = Direccion.Arriba;
                    
                }
                if (e.Key == Key.Down)
                {
                    direccionJugador = Direccion.Abajo;
                }

                if (e.Key == Key.Left)
                {
                    direccionJugador = Direccion.Izquierda;
                }

                if (e.Key == Key.Right)
                {
                    direccionJugador = Direccion.Derecha;
                }
            }
        }

        private void canvasGamePlay_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && direccionJugador == Direccion.Arriba)
            {
                direccionJugador = Direccion.Arriba;
            }

            if (e.Key == Key.Down && direccionJugador == Direccion.Abajo)
            {
                direccionJugador = Direccion.Abajo;
            }

            if (e.Key == Key.Left && direccionJugador == Direccion.Izquierda)
            {
                direccionJugador = Direccion.Izquierda;
            }

            if (e.Key == Key.Right && direccionJugador == Direccion.Derecha)
            {
                direccionJugador = Direccion.Derecha;
            }

        }
    }
}
