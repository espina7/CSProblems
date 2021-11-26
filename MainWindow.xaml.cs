using System;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace MidtermPlane
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class Airplane
        {
            List<Part> parts = null;
            public ModelVisual3D airplane;
            public Point3D mins = new Point3D(0, 0, 0), maxs = new Point3D(0, 0, 0), camera = new Point3D(0,0,0);
            Vector3D center;

            public Airplane(List<Part> parts = null)
            {
                //Declare Scene Objects
                Viewport3D myViewport3D = new Viewport3D();
                Model3DGroup myModel3DGroup = new Model3DGroup();

                string str = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Parts";

                airplane = new ModelVisual3D();
                int i = 0;
                if (parts != null) this.parts = parts;
                else this.parts = new List<Part>();
                foreach (var file in Directory.EnumerateFiles(str, "*.STL"))
                {
                    Part temp = importBinary(file);
                    Console.WriteLine(temp.name + " part added at: " + i);
                    this.parts.Add(temp);
                    myModel3DGroup.Children.Add(temp.myGeometryModel);
                    i++;
                }

                center = FindCenter();
                camera.X = center.X;
                camera.Y = center.Y;
                camera.Z = center.Z - 200;
                airplane.Content = myModel3DGroup;
                Console.WriteLine("Model Added.");
                Console.WriteLine(camera + " " + ((MainWindow)Application.Current.MainWindow).myPerspectiveCamera.LookDirection);
                ((MainWindow)Application.Current.MainWindow).viewport.Children.Add(airplane);
                ((MainWindow)Application.Current.MainWindow).myPerspectiveCamera.Position = new Point3D(center.X, center.Y, center.Z);
                ((MainWindow)Application.Current.MainWindow).myPerspectiveCamera.LookDirection = new Vector3D(center.X- center.X, center.Y-center.Y, center.Z-camera.Z);
            }

            public int FindPart(String str)
            {
                int partNum = 0;
                foreach(Part part in parts)
                {
                    string temp = part.name;
                    if (temp.Contains(str))
                    {
                        return partNum;
                    }
                    partNum++;
                }

                return -1;
            }

            public Vector3D FindCenter()
            {
                double minX = 0.0, maxX = 0.0, minY = 0.0, maxY = 0.0, minZ = 0.0, maxZ = 0.0;
                int i = 0;
                foreach(Part part in parts)
                {
                    if(i == 0)
                    {
                        minX = part.minX;
                        maxX = part.maxX;
                        minY = part.minY;
                        maxY = part.maxY;
                        minZ = part.minZ;
                        maxZ = part.maxZ;
                    }
                    else
                    {
                        if (minX > part.minX)
                            minX = part.minX;
                        if (maxX < part.maxX)
                            maxX = part.maxX;
                        if (minY > part.minY)
                            minY = part.minY;
                        if (maxY < part.maxY)
                            maxY = part.maxY;
                        if (minZ > part.minZ)
                            minZ = part.minZ;
                        if (maxZ < part.maxZ)
                            maxZ = part.maxZ;
                    }

                    i++;
                }

                Vector3D center = new Vector3D((maxX - minX) / 2, (maxY - minY) / 2, (maxZ - minZ) / 2);
                return center;
            }

            public void userControl(KeyEventArgs e)
            {
                Transform3DGroup myTransform3DGroup = new Transform3DGroup();
                Point3D temp, tempLooking;
                temp = this.camera;
                tempLooking = ((MainWindow)Application.Current.MainWindow).skyboxCam.Position;
                myTransform3DGroup.Children.Add(airplane.Transform);
                int partNum;
                partNum = FindPart("Propeller-2.STL");
                Console.WriteLine(partNum);
                parts[partNum] = parts[partNum].RotatePart();
                partNum = FindPart("Propeller-2-1");
                Console.WriteLine(partNum);
                parts[partNum] = parts[partNum].RotatePart();

                switch (e.Key)
                {
                    case Key.W:
                        TranslateTransform3D myTranslate = new TranslateTransform3D(1, 0, 0);
                        TranslateTransform3D skyboxTrans = new TranslateTransform3D(0, 0, -1);
                        temp = temp * myTranslate.Value;
                        tempLooking = tempLooking * skyboxTrans.Value;
                        myTransform3DGroup.Children.Add(myTranslate);
                        break;
                    case Key.S:
                        TranslateTransform3D myTranslate2 = new TranslateTransform3D(-1, 0, 0);
                        TranslateTransform3D skyboxTrans2 = new TranslateTransform3D(0, 0, 1);
                        temp = temp * myTranslate2.Value;
                        tempLooking = tempLooking * skyboxTrans2.Value;
                        myTransform3DGroup.Children.Add(myTranslate2);
                        break;
                    case Key.Q:
                        RotateTransform3D myRotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 1));
                        temp = temp * myRotate.Value;
                        myTransform3DGroup.Children.Add(myRotate);
                        break;
                    case Key.E:
                        RotateTransform3D myRotate2 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -1));
                        temp = temp * myRotate2.Value;
                        myTransform3DGroup.Children.Add(myRotate2);
                        break;
                    case Key.A:
                        RotateTransform3D myRotate3 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 1));
                        temp = temp * myRotate3.Value;
                        myTransform3DGroup.Children.Add(myRotate3);
                        break;
                    case Key.D:
                        RotateTransform3D myRotate4 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), -1));
                        temp = temp * myRotate4.Value;
                        myTransform3DGroup.Children.Add(myRotate4);
                        break;
                    case Key.Z:
                        RotateTransform3D myRotate5 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 1));
                        temp = temp * myRotate5.Value;
                        myTransform3DGroup.Children.Add(myRotate5);
                        break;
                    case Key.X:
                        RotateTransform3D myRotate6 = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -1));
                        temp = temp * myRotate6.Value;
                        myTransform3DGroup.Children.Add(myRotate6);
                        break;
                    default:
                        break;
                }

                this.camera = temp;
                ((MainWindow)Application.Current.MainWindow).myPerspectiveCamera.Position = this.camera;
                ((MainWindow)Application.Current.MainWindow).skyboxCam.Position = tempLooking;
                airplane.Transform = myTransform3DGroup;
            }
        }
        class Part
        {
            private List<Surface> surfaces = null;
            public MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();
            public Vector3DCollection normals = new Vector3DCollection();
            public Point3DCollection points = new Point3DCollection();
            public PointCollection texture = new PointCollection();
            public Int32Collection triangleIndices = new Int32Collection();
            public GeometryModel3D myGeometryModel = new GeometryModel3D();
            public ModelVisual3D part = new ModelVisual3D();
            public Model3DGroup myModel3DGroup = new Model3DGroup();
            public int index = 0;
            public double minX, minY, minZ, maxX, maxY, maxZ;
            public String name { get; set; } = "";

            public Part(String name, List<Surface> surfaces = null)
            {
                this.name = name;
                if (surfaces != null) this.surfaces = surfaces;
                else this.surfaces = new List<Surface>();

                LinearGradientBrush myHorizontalGradient = new LinearGradientBrush();
                myHorizontalGradient.StartPoint = new Point(0, 0.5);
                myHorizontalGradient.EndPoint = new Point(1, 0.5);
                myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Black, 0.0));
                myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.5));
                myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Maroon, 1.0));

                DiffuseMaterial myMaterial = new DiffuseMaterial(myHorizontalGradient);
                myGeometryModel.Material = myMaterial;
            }

            public void addSurface(Surface surface)
            {
                surfaces.Add(surface);
            }

            public Surface getSurface(int index)
            {
                return surfaces[index];
            }

            public int size()
            {
                return surfaces.Count;
            }

            public void removeSurface(int index)
            {
                surfaces.Remove(surfaces[index]);
            }

            public void removeSurface(Surface surface)
            {
                surfaces.Remove(surface);
            }

            public void recalculateNormals(bool outward = true)
            {
                foreach (Surface sur in surfaces)
                {
                    Vector3D p1 = sur.vertices[0];
                    Vector3D p2 = sur.vertices[1];
                    Vector3D p3 = sur.vertices[2];

                    Vector3D line21 = new Vector3D();
                    Vector3D line23 = new Vector3D();

                    line21.X = p1.X - p2.X;
                    line21.Y = p1.Y - p2.Y;
                    line21.Z = p1.Z - p2.Z;

                    line23.X = p3.X - p2.X;
                    line23.Y = p3.Y - p2.Y;
                    line23.Z = p3.Z - p2.Z;

                    Vector3D norm = new Vector3D();
                    norm.X = line21.Y * line23.Z - line23.Y * line21.Z;
                    norm.Y = line21.Z * line23.X - line23.Z * line21.X;
                    norm.Z = line21.X * line23.Y - line23.X * line21.Y;

                    norm.Normalize();

                    Vector3D surVec = sur.surfaceVector();
                    if (surVec.X * norm.X + surVec.Y * norm.Y + surVec.Z * norm.Z < 0)
                    {
                        norm *= -1.0;
                    }

                    sur.normal = norm;
                }
            }

            public Part RotatePart()
            {
                Console.WriteLine("Rotating Part");
                Part temp;
                temp = this;
                AxisAngleRotation3D axisAngle = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 10);
                RotateTransform3D myRotate = new RotateTransform3D(axisAngle);
                Vector3DAnimation myVectorAnimation = new Vector3DAnimation(new Vector3D(-1, -1, -1), new Duration(TimeSpan.FromMilliseconds(500)));
                myVectorAnimation.RepeatBehavior = RepeatBehavior.Forever;
                myRotate.Rotation.BeginAnimation(AxisAngleRotation3D.AxisProperty, myVectorAnimation);
                Transform3DGroup partGroup = new Transform3DGroup();
                partGroup.Children.Add(this.part.Transform);
                partGroup.Children.Add(myRotate);
                temp.part.Transform = partGroup;

                temp.recalculateNormals();
                return temp;
            }
        }

        class Surface
        {
            public Vector3D normal { get; set; }
            public List<Vector3D> vertices { get; set; } = new List<Vector3D>();

            public Surface()
            {
                init(new Vector3D(), new List<Vector3D>());
            }

            public Surface(Vector3D normal, List<Vector3D> vertices)
            {
                init(normal, vertices);
            }

            public Vector3D surfaceVector()
            {
                Vector3D surVector = new Vector3D();
                surVector.X = 0;
                surVector.Y = 0;
                surVector.Z = 0;

                foreach (Vector3D vec in vertices)
                {
                    surVector += vec;
                }

                return surVector;
            }

            private void init(Vector3D _normal, List<Vector3D> _vertices)
            {
                normal = _normal;
                vertices = _vertices;
            }
        }

        private static Part importBinary(String fileName)
        {
            int scaleFactor = 1;
            Part returnPart = new Part(fileName);
            try
            {
                Object locker = new Object();
                lock (locker)
                {
                    using (BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open)))
                    {
                        byte[] header = br.ReadBytes(80);
                        byte[] lenth = br.ReadBytes(4);

                        int numberOfSurfaces = BitConverter.ToInt32(lenth, 0);
                        string headerInfo = Encoding.UTF8.GetString(header, 0, header.Length).Trim();

                        byte[] block;
                        int surfCount = 0;

                        while ((block = br.ReadBytes(50)) != null && surfCount++ < numberOfSurfaces)
                        {
                            Surface newSurf = new Surface();
                            List<Vector3D> verts = new List<Vector3D>();
                            byte[] xComp = new byte[4];
                            byte[] yComp = new byte[4];
                            byte[] zComp = new byte[4];

                            for (int i = 0; i < 4; i++)
                            {
                                for (int k = 0; k < 12; k++)
                                {
                                    int index = k + i * 12;

                                    if (k < 4)
                                    {
                                        xComp[k] = block[index];
                                    }
                                    else if (k < 8)
                                    {
                                        yComp[k - 4] = block[index];
                                    }
                                    else
                                    {
                                        zComp[k - 8] = block[index];
                                    }
                                }

                                float xCompReal = BitConverter.ToSingle(xComp, 0);
                                float yCompReal = BitConverter.ToSingle(yComp, 0);
                                float zCompReal = BitConverter.ToSingle(zComp, 0);

                                if (i == 0)
                                {
                                    Vector3D norm = new Vector3D();
                                    norm.X = xCompReal;
                                    norm.Y = yCompReal;
                                    norm.Z = zCompReal;

                                    newSurf.normal = norm;
                                    returnPart.normals.Add(norm);
                                }
                                else
                                {
                                    Vector3D vert = new Vector3D();
                                    vert.X = xCompReal * scaleFactor;
                                    vert.Y = yCompReal * scaleFactor;
                                    vert.Z = zCompReal * scaleFactor;
                                    verts.Add(vert);
                                    returnPart.points.Add(new Point3D(vert.X, vert.Y, vert.Z));
                                    returnPart.triangleIndices.Add(returnPart.index++);
                                    returnPart.texture.Add(new Point(returnPart.index % 2, (returnPart.index + 1) % 2));
                                    
                                    if(i == 1)
                                    {
                                        returnPart.minX = returnPart.maxX = vert.X;
                                        returnPart.minY = returnPart.maxY = vert.Y;
                                        returnPart.minZ = returnPart.maxZ = vert.Z;
                                    }
                                    else
                                    {
                                        if (vert.X < returnPart.minX)
                                            returnPart.minX = vert.X;
                                        if (vert.X > returnPart.maxX)
                                            returnPart.maxX = vert.X;
                                        if (vert.Y < returnPart.minY)
                                            returnPart.minY = vert.Y;
                                        if (vert.Y > returnPart.maxX)
                                            returnPart.maxY = vert.Y;
                                        if (vert.Z < returnPart.minZ)
                                            returnPart.minZ = vert.Z;
                                        if (vert.Z > returnPart.maxZ)
                                            returnPart.maxZ = vert.Z;
                                    }
                                }
                            }

                            returnPart.myMeshGeometry3D.Positions = returnPart.points;
                            returnPart.myMeshGeometry3D.Normals = returnPart.normals;
                            newSurf.vertices = verts;
                            returnPart.addSurface(newSurf);
                        }

                        returnPart.myGeometryModel.Geometry = returnPart.myMeshGeometry3D;
                        returnPart.myModel3DGroup.Children.Add(returnPart.myGeometryModel);
                        returnPart.part.Content = returnPart.myModel3DGroup;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("This file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }

            return returnPart;
        }

        private Airplane airplane;

        public MainWindow()
        {
            InitializeComponent();

            airplane = new Airplane();
            Image img = new Image();
            BitmapImage background = new BitmapImage();
            background.BeginInit();
            string str = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\Background\\runway.jpg";
            Console.WriteLine(str);
            background.UriSource = new Uri(str, UriKind.Relative);
            background.EndInit();
            img.Source = background;
            ImageBrush brush = new ImageBrush(img.Source);
            this.difMat.Brush = brush;
            this.difMat1.Brush = brush;
            this.difMat2.Brush = brush;
            this.difMat3.Brush = brush;
            this.difMat4.Brush = brush;
            this.difMat5.Brush = brush;
        }

        public void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            airplane.userControl(e);
        }
    }
}

