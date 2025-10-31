using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
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
        outSpeed = bytesOut / 1000000.0f;
        outPPs = bytesOut / payloadSize;
        bytesOut = 0;
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
            AddToSend(bytes);

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

    private void AddToSend(byte[] data)
    {
        bytesOut += data.Length;
    }

    private void FormMain_Shown(object? sender, EventArgs e)
    {

    }


}
