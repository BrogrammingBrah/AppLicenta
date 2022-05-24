///HAVE 3DTOOLS PACKAGE INSTALLED!


using _3DTools;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using MathNet.Numerics; no need for thisd
using static _3DTools.ScreenSpaceLines3D;

namespace GraficFunctii2Var

{
    public partial class MainWindow : Window
    {
        private TranslateTransform trans;

        private TransformGroup group;
        public MainWindow()
        {
            InitializeComponent();

           
        }


        double r;


        double X, Y, a, b, m, M;
        const double dx = 0.05;
        const double dz = 0.05;

        //    double xmin = -3, xmax = 3;
        //  double zmin = -3, zmax = 3;
        double xmin, xmax, zmin, zmax;
        private double CameraR = 18.0;

        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private PerspectiveCamera TheCamera;
        private double CameraPhi = Math.PI / 6.0;
        private double CameraTheta = Math.PI / 6.0;
        //pentru deplasarea camerei folosind tastele directionale si +/-
        private const double CameraDPhi = 0.1;
        private const double CameraDTheta = 0.1;
        private const double CameraDR = 0.1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // pozitia initiala a camerei
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
        }
        // definim luminile
        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray,
                    new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
        }
        // pozitionam camera.
        private void PositionCamera()
        {
            //calculam pozitia camerei in coordonate Carteziene
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(x, y, z);

            // orientam camera spre origine
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            // setam orientarea camerei
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }
        private void adaugaElementSuprafata(MeshGeometry3D mesh, Point3D point0, Point3D point1, Point3D point2, Point3D point3)
        {

            // adaugam punctele
            mesh.Positions.Add(point0);
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            //adaugam triunghiurile care formeaza acel element de pe interfata

            int nr = mesh.Positions.Count - 1;

            mesh.TriangleIndices.Add(nr - 3);
            mesh.TriangleIndices.Add(nr - 2);
            mesh.TriangleIndices.Add(nr - 1);

            mesh.TriangleIndices.Add(nr - 3);
            mesh.TriangleIndices.Add(nr - 1);
            mesh.TriangleIndices.Add(nr);
        }
        //============================================
        // Creare grafic (definire model 3D)
        private void generareGrafic(Model3DGroup model_group)
        {
            //cream suprafata.
            MeshGeometry3D mesh = new MeshGeometry3D();
            // generam punctele si triunghiurile suprafetei
            for (double x = xmin; x <= xmax - dx; x += dx)
            {
                for (double z = zmin; z <= zmax - dz; z += dx)
                {
                    Point3D p00 = new Point3D(x, F(x, z), z);
                    Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
                    Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
                    Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

                    // adaugam elementul corespunzator
                    adaugaElementSuprafata(mesh, p00, p01, p11, p10);
                }
            }
            // Coloram suprafata cu o culoare mata
            DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.Orange);
            GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);
            // facem suprafata vizibila pe ambele fete
            surface_model.BackMaterial = surface_material;
            // adaugam suprafata
            model_group.Children.Add(surface_model);




            Point3DCollection point3Ds_X = new Point3DCollection();
            point3Ds_X.Add(new Point3D(0, 0, -20));
            point3Ds_X.Add(new Point3D(0, 0, 20));
            ScreenSpaceLines3D X_axis = new ScreenSpaceLines3D() { Points = point3Ds_X, Thickness = 2, Color = Colors.Red };


            Point3DCollection point3Ds_Y = new Point3DCollection();
            point3Ds_Y.Add(new Point3D(0, -20, 0));
            point3Ds_Y.Add(new Point3D(0, 20, 0));
            ScreenSpaceLines3D Y_axis = new ScreenSpaceLines3D() { Points = point3Ds_Y, Thickness = 2, Color = Colors.Green };


            Point3DCollection point3Ds_Z = new Point3DCollection();
            point3Ds_Z.Add(new Point3D(-20, 0, 0));
            point3Ds_Z.Add(new Point3D(20, 0, 0));
            ScreenSpaceLines3D Z_axis = new ScreenSpaceLines3D() { Points = point3Ds_Z, Thickness = 2, Color = Colors.Yellow };

            MainViewport.Children.Add(X_axis);
            MainViewport.Children.Add(Y_axis);
            MainViewport.Children.Add(Z_axis);


        }
        // functia de reprezentat grafic


        /* private void generareGrafic(Model3DGroup model_group)
         {
             //cream suprafata.
             MeshGeometry3D mesh = new MeshGeometry3D();
             // generam punctele si triunghiurile suprafetei
             for (double x = xmin; x <= xmax - dx; x += dx)
             {
                 for (double z = zmin; z <= zmax - dz; z += dx)
                 {
                     Point3D p00 = new Point3D(x, F(x, z), z);
                     Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
                     Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
                     Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

                     // adaugam elementul corespunzator
                     adaugaElementSuprafata(mesh, p00, p01, p11, p10);
                 }
             }
             // Coloram suprafata cu o culoare mata
             DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.Orange);
             GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);
             // facem suprafata vizibila pe ambele fete
             surface_model.BackMaterial = surface_material;
             // adaugam suprafata
             model_group.Children.Add(surface_model);
         }*/


        /// sist de axe 2d,3d
       /* Point daCoord(double x, double y)
        {
            int x0 = (int)((x - a) * raport + 50 + (400 - (b - a) * raport) / 2);
            int y0 = (int)((M - y) * raport + 50 + (400 - (M - m) * raport) / 2);
            return new Point(x0, y0);
        }

        void deseneazaAxe()
        {
            g.Clear(Color.White);
            g.FillRectangle(Brushes.LightGray, 25, 25, 450, 450);
            Pen creion = new Pen(Color.Black, 3);
            g.DrawLine(creion, 25, daCoord(a, 0).Y, 475, daCoord(a, 0).Y);
            g.DrawLine(creion, 469, daCoord(a, 0).Y - 6, 475, daCoord(a, 0).Y);
            g.DrawLine(creion, 469, daCoord(a, 0).Y + 6, 475, daCoord(a, 0).Y);
            g.DrawLine(creion, daCoord(0, m).X, 475, daCoord(0, m).X, 25);
            g.DrawLine(creion, daCoord(0, m).X - 6, 31, daCoord(0, m).X, 25);
            g.DrawLine(creion, daCoord(0, m).X + 6, 31, daCoord(0, m).X, 25);
            p.Refresh();
        }
        void scrieCoordonate()
        {
            System.Drawing.Font f = new System.Drawing.Font("Courier New", 14);
            Pen creion = new Pen(Color.Black, 3);
            g.DrawString("O", f, Brushes.Blue, daCoord(0, 0));
            g.DrawLine(creion, daCoord(a, 0).X, daCoord(a, 0).Y - 6, daCoord(a, 0).X, daCoord(a, 0).Y + 6);
            g.DrawString("a", f, Brushes.Blue, daCoord(a, 0));
            g.DrawLine(creion, daCoord(b, 0).X, daCoord(b, 0).Y - 6, daCoord(b, 0).X, daCoord(b, 0).Y + 6);
            g.DrawString("b", f, Brushes.Blue, daCoord(b, 0));
            g.DrawLine(creion, daCoord(0, M).X - 6, daCoord(0, M).Y, daCoord(0, M).X + 6, daCoord(0, M).Y);
            g.DrawString("M", f, Brushes.Blue, daCoord(0, M).X + 6, daCoord(0, M).Y - 6);
            g.DrawLine(creion, daCoord(0, m).X - 6, daCoord(0, m).Y, daCoord(0, m).X + 6, daCoord(0, m).Y);
            g.DrawString("m", f, Brushes.Blue, daCoord(0, m).X + 6, daCoord(0, m).Y - 6);
            g.DrawString("a=" + a, f, Brushes.Blue, 25, 0);
            g.DrawString("b=" + b, f, Brushes.Blue, 125, 0);
            g.DrawString("m=" + ((int)(m * 100000) / 100000.0), f, Brushes.Blue, 225, 0);
            g.DrawString("M=" + ((int)(M * 100000) / 100000.0), f, Brushes.Blue, 355, 0);
            p.Refresh();
        }

        */

        void test_campuri()
        {//verificam corectitudinea valorilor introduse in campuri
            int temp = 0;
            for (int i = 0; i < functia.Text.Length; i++)
            {
                if (functia.Text[i] == '(') temp++;
                if (functia.Text[i] == ')') temp--;
            }
            if (temp > 0) throw new Exception("Lipsa paranteza )");
            if (temp < 0) throw new Exception("Lipsa paranteza (");
            try
            {
                if (xmic.Text.Trim().Length == 0) a = 0;
                else a = Convert.ToDouble(xmic.Text);
            }
            catch (System.Exception)
            {
                throw new Exception("Valoarea lui x nu este corecta!");
            }
            try
            {
                if (xmare.Text.Trim().Length == 0) b = 0;
                else b = Convert.ToDouble(xmare.Text);
            }
            catch (System.Exception)
            {
                throw new Exception("Valoarea lui y nu este corecta!");
            }
        }



        /* */
        int da_pozitie(String expresie, int codCaracterCautat)
        {//da ultima aparitie pentru caracterul ce are codul codCaracterCautat (sare peste paranteze)
            int temp = 0;
            for (int i = expresie.Length - 1; i >= 0; i--)
            {
                if ((temp == 0) && (expresie[i] == codCaracterCautat)) return i;
                if (expresie[i] == ')') temp++;
                if (expresie[i] == '(') temp--;
            }
            return -1;
        }
        double calculeaza(String expresie)
        {
            if (expresie.Trim().Length == 0) return 0;
            int pozitie;
            pozitie = da_pozitie(expresie, '+');
            if (pozitie >= 0) return calculeaza(expresie.Substring(0, pozitie)) + calculeaza(expresie.Substring(pozitie + 1));
            pozitie = da_pozitie(expresie, '-');
            if (pozitie >= 0) return calculeaza(expresie.Substring(0, pozitie)) - calculeaza(expresie.Substring(pozitie + 1));
            pozitie = da_pozitie(expresie, '*');
            if (pozitie >= 0) return calculeaza(expresie.Substring(0, pozitie)) * calculeaza(expresie.Substring(pozitie + 1));
            pozitie = da_pozitie(expresie, '/');
            if (pozitie >= 0)
            {
                double val1 = calculeaza(expresie.Substring(0, pozitie));
                double val2 = calculeaza(expresie.Substring(pozitie + 1));
                if (val2 != 0) return val1 / val2;
                return Double.MaxValue;
            }
            if (expresie.StartsWith("sin(") && expresie.EndsWith(")")) return Math.Sin(calculeaza(expresie.Substring(4, expresie.Length - 5)));
            if (expresie.StartsWith("cos(") && expresie.EndsWith(")")) return Math.Cos(calculeaza(expresie.Substring(4, expresie.Length - 5)));
            if (expresie.StartsWith("tg(") && expresie.EndsWith(")")) return Math.Tan(calculeaza(expresie.Substring(3, expresie.Length - 4)));
            if (expresie.StartsWith("ln(") && expresie.EndsWith(")")) return Math.Log(calculeaza(expresie.Substring(3, expresie.Length - 4))) / Math.Log(Math.E);
            if (expresie.StartsWith("exp(") && expresie.EndsWith(")")) return Math.Exp(calculeaza(expresie.Substring(4, expresie.Length - 5)));
            if (expresie.StartsWith("(") && expresie.EndsWith(")")) return calculeaza(expresie.Substring(1, expresie.Length - 2));
            if (expresie.ToLower() == "x") return X;
            if (expresie.ToLower() == "y") return Y;
            try
            {
                return Convert.ToDouble(expresie);
            }
            catch (System.Exception)
            {
                throw new Exception("Expresia data nu este corecta!");
            }
        }

        private double F(double x, double z)
        // private double F(double val)
        {
            //            double r2 = x * x + z * z;
            //           return 8 * Math.Cos(r2 / 2) / (2 + r2);
            //   double rr = 8 * Math.Cos((x * x + z * z) / 2) / (2 + x * x + z * z);
            //  return rr;
            //  r = Convert.ToDouble(functia.Text);
            //return r;
            X = x;
            Y = z;
            //            Y=y;
            return calculeaza(functia.Text);
        }






        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*       xmin = -3.5;
                   xmax = 3.5;
                   zmin = -3.5;
                   zmax = 3.5;
              */
            try
            {
                xmin = Convert.ToDouble(xmic.Text);
                xmax = Convert.ToDouble(xmare.Text);
                zmin = Convert.ToDouble(zmic.Text);
                zmax = Convert.ToDouble(zmare.Text);
                CameraR = 13.0;

                MainModel3Dgroup = new Model3DGroup();

                // Create the model.
                generareGrafic(MainModel3Dgroup);

                PositionCamera();
                // Define lights.
                DefineLights();

                // Add the group of models to a ModelVisual3D.
                ModelVisual3D model_visual = new ModelVisual3D();
                model_visual.Content = MainModel3Dgroup;

                // Add the main visual to the viewportt.
                MainViewport.Children.Add(model_visual);
            }
            catch
            {
                MainModel3Dgroup.ClearValue((DependencyProperty)Content);

                //  MainModel3Dgroup

            }
        }

        // raised when the mouse pointer moves.
        // Expands the dimensions of an Ellipse when the mouse moves.
        /*    private void MouseMoveHandler(object sender, MouseEventArgs e)
            {
                // Get the x and y coordinates of the mouse pointer.
                System.Windows.Point position = e.GetPosition(this);
                double pX = position.X;
                double pY = position.Y;

                // Sets the Height/Width of the circle to the mouse coordinates.
                MainModel3Dgroup.Width = pX;
                ellipse.Height = pY;
            }*/

        // modificam pozitia camerei
        /* private void Window_KeyDown(object sender, KeyEventArgs e)
         {
             switch (e.Key)
             {
                 case Key.Up:
                     CameraPhi += CameraDPhi;
                     if (CameraPhi > Math.PI / 2.0)
                         CameraPhi = Math.PI / 2.0;
                     break;
                 case Key.Down:
                     CameraPhi -= CameraDPhi;
                     if (CameraPhi < -Math.PI / 2.0)
                         CameraPhi = -Math.PI / 2.0;
                     break;
                 case Key.Left:
                     CameraTheta += CameraDTheta;
                     break;
                 case Key.Right:
                     CameraTheta -= CameraDTheta;
                     break;
                 case Key.Add:
                 case Key.OemPlus:
                     CameraR -= CameraDR;
                     if (CameraR < CameraDR) CameraR = CameraDR;
                     break;
                 case Key.Subtract:
                 case Key.OemMinus:
                     CameraR += CameraDR;
                     break;
             }

             // actualizam pozitia camerei
             PositionCamera();
         }*/


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
           {
               switch (e.ChangedButton)
               {
                   case MouseButton.XButton2:
                       CameraPhi += CameraDPhi;
                       if (CameraPhi > Math.PI / 2.0)
                           CameraPhi = Math.PI / 2.0;
                       break;
                   case MouseButton.XButton1:
                       CameraPhi -= CameraDPhi;
                       if (CameraPhi < -Math.PI / 2.0)
                           CameraPhi = -Math.PI / 2.0;
                       break;
                   case MouseButton.Left:
                       CameraTheta += CameraDTheta;
                       break;
                   case MouseButton.Right:
                       CameraTheta -= CameraDTheta;
                       break;
                 /*  case Key.Add:
                   case Key.OemPlus:
                       CameraR -= CameraDR;
                       if (CameraR < CameraDR) CameraR = CameraDR;
                       break;
                   case Key.Subtract:
                   case Key.OemMinus:
                       CameraR += CameraDR;
                       break;
               */}

               // actualizam pozitia camerei
               PositionCamera();
           }
           

      


    }

}