using System;
using System.Collections.Generic;
//using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace StiLib
{
    public class TIFF
    {
        public static void ConvertBitmapsToTiff(List<ExtendedBitmap> exBms, string fileName)
        {

            string path = fileName;
            Bitmap img = exBms[0].Bm;
            ImageCodecInfo inf = getEncoderInfo("image/tiff");
            Encoder saveFlag = Encoder.SaveFlag;
            // Save first page
            EncoderParameters eParams = new EncoderParameters(1);
            eParams.Param[0] = new EncoderParameter(saveFlag, (long)EncoderValue.MultiFrame);
            img.Save(path, inf, eParams);
            // Save other pages
            eParams.Param[0] = new EncoderParameter(saveFlag, (long)EncoderValue.FrameDimensionPage);
            for (int i = 1; i < exBms.Count; i++)
            {
                img.SaveAdd(exBms[i].Bm, eParams);
            }
            eParams.Param[0] = new EncoderParameter(saveFlag, (long)EncoderValue.Flush);
            img.SaveAdd(eParams); 
        }

        public static List<Bitmap> ConvertTiffToBitmaps(string fileName)
        {
            List<Bitmap> result = new List<Bitmap>();
            Bitmap multiFrame = new Bitmap(fileName);
            int frameCount = multiFrame.GetFrameCount(FrameDimension.Page);
            for (int i = 0; i < frameCount; i++)
            {
                multiFrame.SelectActiveFrame(FrameDimension.Page, i);
                result.Add((Bitmap)multiFrame.Clone());
            }
            return result;
        }

        public static ImageCodecInfo getEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
