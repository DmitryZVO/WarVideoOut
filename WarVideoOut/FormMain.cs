using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace WarVideoOut;

public partial class FormMain : Form
{
    private VideoCapture capture;
    private readonly OpenCvSharp.Size resolution = new(320, 240);
    private readonly Timer timerFrame;
    private readonly Timer timer1Sec;
    private int fps = 30;
    private int frameCount = 0;
    private int payloadSize = 600;
    private int portUdp = 7777;
    private float outSpeed = 0.0f;
    private int outPPs = 0;
    private int ppsCount = 0;
    private int bytesOut = 0;

    private readonly string videoFile = "video.mp4";

    public FormMain()
    {
        InitializeComponent();

        capture = new VideoCapture();
        capture.Open("video.mp4", VideoCaptureAPIs.MSMF);

        Shown += FormMain_Shown;

        timerFrame = new Timer
        {
            Interval = 1000 / fps // Approximately 30 frames per second
        };
        timerFrame.Tick += TimerFrame_Tick; ;
        timerFrame.Start();

        timer1Sec = new Timer
        {
            Interval = 1000
        };
        timer1Sec.Tick += Timer1Sec_Tick;
        timer1Sec.Start();

        trackBarFps.Value = fps;
        trackBarFps.ValueChanged += TrackBarFps_ValueChanged;
        trackBarFrameSize.Value = payloadSize;
        trackBarFrameSize.ValueChanged += TrackBarFrameSize_ValueChanged;
    }

    private void TrackBarFrameSize_ValueChanged(object? sender, EventArgs e)
    {
        payloadSize = (trackBarFrameSize.Value/3)*3;
    }

    private void TrackBarFps_ValueChanged(object? sender, EventArgs e)
    {
        fps = trackBarFps.Value;
        timerFrame.Interval = 1000 / fps;
    }

    private void Timer1Sec_Tick(object? sender, EventArgs e)
    {
        outSpeed = (bytesOut*8) / 1000000.0f;
        outPPs = ppsCount;
        bytesOut = 0;
        ppsCount = 0;
    }

    private void TimerFrame_Tick(object? sender, EventArgs e)
    {
        using Mat frame = new();

        if (capture.Read(frame))
        {
            using Mat frameRes = new();
            Cv2.Resize(frame, frameRes, resolution);
            Cv2.PutText(frameRes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), new OpenCvSharp.Point(frameRes.Width-180, frameRes.Height-5), HersheyFonts.HersheySimplex, 0.4f, Scalar.Blue);
            Cv2.PutText(frameRes, $"FPS: {fps:0}", new OpenCvSharp.Point(5, 15), HersheyFonts.HersheySimplex, 0.4f, Scalar.Blue);
            Cv2.PutText(frameRes, $"FRAME: {frameCount:0}", new OpenCvSharp.Point(5, 30), HersheyFonts.HersheySimplex, 0.4f, Scalar.Blue);
            Cv2.PutText(frameRes, $"Payload: {payloadSize:0}", new OpenCvSharp.Point(5, 45), HersheyFonts.HersheySimplex, 0.4f, Scalar.Blue);
            Cv2.PutText(frameRes, $"OutSpeed: {outSpeed:0.00}(MBit/sec)", new OpenCvSharp.Point(5, 60), HersheyFonts.HersheySimplex, 0.4f, Scalar.Blue);
            Cv2.PutText(frameRes, $"OutPPS: {outPPs:0}(pkt/sec)", new OpenCvSharp.Point(5, 75), HersheyFonts.HersheySimplex, 0.4f, Scalar.Blue);

            // Добавляем кадр в очередь на отправку
            var bytes = new byte[frameRes.Total() * 3];
            Marshal.Copy(frameRes.Data, bytes, 0, bytes.Length);
            AddToSend(bytes, frameCount, (ushort)payloadSize);

            // Отображаем кадр
            Bitmap bitmap = BitmapConverter.ToBitmap(frameRes);
            if (pictureBoxMain.Image != null)
            {
                pictureBoxMain.Image.Dispose();
                pictureBoxMain.Image = null;
            }
            pictureBoxMain.Image = bitmap;
            frameCount++;
        }
        else
        {
            capture.Release();
            capture.Dispose();

            capture = new VideoCapture();
            capture.Open("video.mp4", VideoCaptureAPIs.MSMF);
        }
    }

    private void AddToSend(byte[] data, int frameNumber, ushort plsize)
    {
        // Формат пакета
        // 0x70, 0x70 - ZVO заголовок (2 байта)
        // 0x01 - тип пакета (1 байт) 1 - кадр видео
        // 0x00000000 - номер кадра (4 байта)
        // 0x0000 - текущий номер куска данных (2 байта)
        // 0x0000 - всего кусков данных (2 байта)
        // 0x0000 - текущая длинна полезной нагрузки (2 байта)
        // [N].. тело полезной нагрузки

        const int LenHeader = 2 + 1 + 4 + 2 + 2 + 2;
        ushort all = (ushort)(data.Length / payloadSize);

        using UdpClient udp = new();

        for (ushort i = 0; i < all; i++)
        {
            var dataToSend = new byte[LenHeader + payloadSize];
            dataToSend[0] = 0x70; // Заголовок HI
            dataToSend[1] = 0x70; // Заголовок LO
            dataToSend[2] = 0x01; // Тип пакета
            Array.Copy(BitConverter.GetBytes(frameNumber), 0, dataToSend, 3, 4); // номер кадра
            Array.Copy(BitConverter.GetBytes((ushort)(i+1)), 0, dataToSend, 7, 2); // номер куска
            Array.Copy(BitConverter.GetBytes(all), 0, dataToSend, 9, 2); // всего кусков
            Array.Copy(BitConverter.GetBytes(plsize), 0, dataToSend, 11, 2); // длинна текущей полезной нагрузки
            Array.Copy(data, i * plsize, dataToSend, 13, plsize); // полезная нагрузка

            udp.Send(dataToSend, dataToSend.Length, new IPEndPoint(IPAddress.Parse("255.255.255.255"), 7777));
            bytesOut += dataToSend.Length;
            ppsCount++;
        }
    }

    private void FormMain_Shown(object? sender, EventArgs e)
    {

    }


}
