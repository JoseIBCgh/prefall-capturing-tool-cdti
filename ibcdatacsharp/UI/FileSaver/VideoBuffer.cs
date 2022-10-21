using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace ibcdatacsharp.UI.FileSaver
{
    // Clase para guardar los frames en memoria y guardarlos en disco al final. Mejora algo el rendimiento
    internal class VideoBuffer
    {
        private List<Mat> frames;
        private VideoWriter? writer;
        private int frameWidth;
        private int frameHeight;
        private bool saving;
        private bool framesWithoutResize;
        public VideoBuffer(int frameWidth=640, int frameHeight=480)
        {
            frames = new List<Mat>();
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            saving = false;
            framesWithoutResize = false;
        }
        // Guarda un frame (no hace falta que tenga las dimensiones correctas)
        public void addFrame(Mat frame, bool resize=true)
        {
            if (!saving)
            {
                if (resize)
                {
                    Mat frameResized = frame.Resize(new Size(frameWidth, frameHeight));
                    frames.Add(frameResized);
                }
                else
                {
                    if (!framesWithoutResize)
                    {
                        framesWithoutResize = true;
                    }
                    frames.Add(frame);
                }
            }
        }
        // Inicializa el video writer
        public void initVideoWriter(string path, FourCC codec, int fps = 30)
        {
            writer = new VideoWriter(path, codec, fps, new Size(frameWidth, frameHeight));
        }
        // Graba todos los frames en memoria
        public void saveFrames()
        {
            if (writer == null)
            {
                throw new Exception("video writer is no initialized");
            }
            else
            {
                saving = true;
                foreach (Mat frame in frames)
                {
                    if (framesWithoutResize)
                    {
                        Mat frameResized = frame.Resize(new Size(frameWidth, frameHeight));
                        writer.Write(frameResized);
                    }
                    else
                    {
                        writer.Write(frame);
                    }
                }
                writer.Release();
                writer = null;
                framesWithoutResize = false;
                saving = false;
            }
        }
        // Inicializa un video writer y graba todos los frames en memoria
        public void saveFrames(string path, FourCC codec, int fps = 30)
        {
            saving = true;
            VideoWriter writer = new VideoWriter(path, codec, fps, new Size(frameWidth, frameHeight));
            foreach (Mat frame in frames)
            {
                if (framesWithoutResize)
                {
                    Mat frameResized = frame.Resize(new Size(frameWidth, frameHeight));
                    writer.Write(frameResized);
                }
                else
                {
                    writer.Write(frame);
                }
            }
            frames.Clear();
            writer.Release();
            framesWithoutResize = false;
            saving = false;
        }
    }
}
