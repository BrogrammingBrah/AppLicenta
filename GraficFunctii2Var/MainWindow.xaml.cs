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

namespace GraficFunctii2Var

{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
                const double dx = 0.05;
        const double dz = 0.05;

        double xmin = -1.5, xmax = 1.5;
        double zmin = -1.5, zmax = 1.5;
        private double CameraR = 13.0;

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

            // setam oriebtarea camerei
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }
        private void adaugaElementSuprafata(MeshGeometry3D mesh, Point3D point0, Point3D point1, Point3D point2, Point3D point3) {
            
            // adaugam punctele
            mesh.Positions.Add(point0);
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            //adaugam triunghiurile care formeaza acel element de pe interfata

            int nr = mesh.Positions.Count - 1;

            mesh.TriangleIndices.Add(nr-3);
            mesh.TriangleIndices.Add(nr-2);
            mesh.TriangleIndices.Add(nr-1);

            mesh.TriangleIndices.Add(nr-3);
            mesh.TriangleIndices.Add(nr-1);
            mesh.TriangleIndices.Add(nr);
        }
        //============================================
        // Creare grafic (definire model 3D)
        private void generareGrafic(Model3DGroup model_group)
        {
            //cream suprafata.
            MeshGeometry3D mesh = new MeshGeometry3D();
            // generam punctele si triunghiurile suprafetei
            for (double x = xmin; x <= xmax - dx; x += dx){
                for (double z = zmin; z <= zmax - dz; z += dx){
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
        }
        // functia de reprezentat grafic
        private double F(double x, double z)
        {
            double r2 = x * x + z * z;
            return 8 * Math.Cos(r2 / 2) / (2 + r2);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            xmin = -3.5;
            xmax = 3.5;
            zmin = -3.5;
            zmax = 3.5;
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
        // modificam pozitia camerei
        private void Window_KeyDown(object sender, KeyEventArgs e)
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
        }
    }
}

