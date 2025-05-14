#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#endregion

namespace ThemeInstaller.LogonImage
{
    public class JpgImage : IDisposable
    {
        // Fields
        private readonly MemoryStream _stream = new MemoryStream();

        // Methods

        // Properties
        public Image Image
        {
            get
            {
                return Image.FromStream(_stream);
            }
        }

        public int Quality { get; set; }

        public long Size { get; set; }

        public MemoryStream Stream
        {
            get { return _stream; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _stream.Dispose();
            Size = 0L;
        }

        #endregion

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            foreach (var ie in imageEncoders)
            {
                if (ie.MimeType == mimeType)
                {
                    return ie;
                }
            }
            return null;
        }

        public void WriteImageToStream(Image img)
        {
            var encoder = GetEncoderInfo("image/jpeg");
            if (encoder == null) return;
            var encoderParams = new EncoderParameters(2);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, Quality);
            encoderParams.Param[1] = new EncoderParameter(Encoder.ColorDepth, 0x18L);
            img.Save(Stream, encoder, encoderParams);
            Size = Stream.Length;
        }

        public void WriteStreamToFile(string path, bool createBackup)
        {
            var info = new FileInfo(path);
            if (info.Directory != null && !info.Directory.Exists)
            {
                info.Directory.Create();
            }
            if (createBackup && info.Exists)
            {
                info.CopyTo(info.FullName + ".bak", true);
            }
            using (var stream = new FileStream(path, FileMode.Create))
            {
                _stream.WriteTo(stream);
                stream.Flush();
            }
        }
    }


}
