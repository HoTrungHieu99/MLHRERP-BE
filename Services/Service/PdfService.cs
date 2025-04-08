using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DinkToPdf;

namespace Services.Service
{
    using DinkToPdf;
    using DinkToPdf.Contracts;
    using Services.Exceptions;
    using System.Runtime.InteropServices;

    public class PdfService
    {
        private readonly IConverter _converter;
        private static bool _libraryLoaded = false;

        /*public PdfService(IConverter converter)
        {
            // ✅ Load thư viện native nếu chạy trên Linux và chưa load lần nào
            if (!_libraryLoaded && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var path = "/usr/local/lib/libwkhtmltox.so";
                if (File.Exists(path))
                {
                    var context = new CustomAssemblyLoadContext();
                    context.LoadUnmanagedLibrary(path);
                    Console.WriteLine("✅ Đã nạp libwkhtmltox.so thành công");
                    _libraryLoaded = true;
                }
                else
                {
                    Console.WriteLine("❌ Không tìm thấy libwkhtmltox.so tại: " + path);
                }
            }

            _converter = converter;
        }*/

        /*public byte[] GeneratePdf(string htmlContent)
        {
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    DocumentTitle = "Phiếu Nhập Kho"
                },
                Objects = {
                new ObjectSettings
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            return _converter.Convert(doc);
        }*/

        public byte[] GeneratePdf(string htmlContent)
        {
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    DocumentTitle = "Phiếu Nhập Kho"
                },
                Objects = {
                new ObjectSettings
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            return _converter.Convert(doc);
        }
    }

}
