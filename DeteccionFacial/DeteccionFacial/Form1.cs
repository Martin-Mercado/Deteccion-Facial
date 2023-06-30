using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace DeteccionFacial
{
    public partial class Form1 : Form
    {

        #region variables

        private Capture videoCapture = null;

        /*
        imagen con colores específicos y tipos de profundidad.En este caso, Bgr representa
        el tipo de color de la imagen, lo que indica que utiliza el modelo de color Azul-Verde-Rojo.
        El tipo Byte indica que la profundidad de cada canal de color es de 8 bits, representando valores de 0 a 255.
        */
        private Image<Bgr, Byte> currentFrame = null;

        //Matriz de pixeles vacia
        Mat frame = new Mat();

        //objeto para detección de objetos basada en clasificadores en cascada haars. En este caso se utiliza para la detección de rostros.
        CascadeClassifier faceCascadeClassifier = new CascadeClassifier(@"..\haarcascade_frontalface_alt.xml");


        #endregion
        public Form1()
        {
            InitializeComponent();
            videoCapture = new Capture();
            videoCapture.ImageGrabbed += ProcessFrame;
            videoCapture.Start();
        }



        private void ProcessFrame(object sender, EventArgs e)
        {
            // capturar cuadro
            videoCapture.Retrieve(frame, 0);
            currentFrame = frame.ToImage<Bgr, Byte>().Resize(picCapture.Width, picCapture.Height, Inter.Cubic);

            //Detectar rostro
   
            //convertir imagen Bgr a escala de grises
            Mat grayImage = new Mat();
            CvInvoke.CvtColor(currentFrame, grayImage, ColorConversion.Bgr2Gray);

            //Ecualización de histograma: distribución más uniforme de las intensidades de píxeles en el rango de escala de grises de la imagen
            CvInvoke.EqualizeHist(grayImage, grayImage);

            //Realizar la deteccion de rostro en grayImage usando faceCascadeClassifier (Cascade Classifier).
            Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.1, 3, Size.Empty, Size.Empty);



            // renderizar cuadro en el picture box
            foreach (var face in faces)
            {
                //diujar un rectangulo ale rededor de la cara actual
                CvInvoke.Rectangle(currentFrame, face, new Bgr(Color.Green).MCvScalar, 2);

                //Agregar persona
                //asignar la cara detectada a la picture box PicDetected
                Image<Bgr, Byte> resultImage = currentFrame.Convert<Bgr, Byte>();
                resultImage.ROI = face;
                picDetected.SizeMode = PictureBoxSizeMode.StretchImage;
                picDetected.Image = resultImage.Bitmap;
            }
                picCapture.Image = currentFrame.Bitmap;
        }
    }
}
