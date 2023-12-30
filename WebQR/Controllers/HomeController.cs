using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;
using StandardizedQR;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using WebQR.Models;
using WebQRCode.Models;

namespace WebQR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Index(QrData qrData)
        {
            var globalUniqueIdentifier = Guid.NewGuid().ToString().Replace("-", string.Empty);

            var merchantPayload = MerchantPayload
                .CreateDynamic(globalUniqueIdentifier, 4111, Iso4217Currency.MexicoPeso.Value.NumericCode, Iso3166Countries.Mexico, "Chocolate Powder", "Mexico City")
                .WithAlternateLanguage(Iso639Languages.SpanishCastilian, "Chocolate en Polvo", "CDMX")
                .WithTransactionAmount(34.95m)
                .WithTipByUser()
                .WithAdditionalData(
                    billNumber: "1234",
                    mobileNumber: "5512341234",
                    storeLabel: "The large store",
                    loyaltyNumber: "A12341234",
                    referenceLabel: "***",
                    terminalLabel: "T12341",
                    purposeOfTransaction: qrData.txtData,
                    additionalConsumerDataRequest: "AME")
                .WithUnreservedTemplate(globalUniqueIdentifier, new Dictionary<int, string>
                {
                    {1, "0255552468845898" },
                    {2, "APGdkjhajklsdhfajkldh" }
                });

            

            var payload = merchantPayload.GeneratePayload();


            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            //qrCode.SetQRCodeData

            return File(qrCode.GetGraphic(10), "image/jpeg"); //Return as file result

            //return View();
        }

        [HttpGet]
        public IActionResult QrParse()
        {
            var qrData = "000201010212263600329327e704e8bf4570b73329b36f981454520441115802MX5916Chocolate Powder6011Mexico City64360002es0118Chocolate en Polvo0204CDMX540534.955303484550201629401041234021055123412340315The large store0409A123412340503***0706T123410812Hasan Monsur0903AME808100329327e704e8bf4570b73329b36f981454011602555524688458980221APGdkjhajklsdhfajkldh6304E1B6";
           //var qrData1 = "00020101021226360032fd77180553174149aea9d7a211c476d4520441115802MX5916Chocolate Powder6011Mexico City64360002es0118Chocolate en Polvo0204CDMX540534.955303484550201628701041234021055123412340315The large store0409A123412340503***0706T123410805Hasan0903AME80670032fd77180553174149aea9d7a211c476d40110Some value0213Another value63046351";
            //var qrData = "00020101021229300012D156000000000510A93FO3230Q31280012D15600000001030812345678520441115802CN5914BEST TRANSPORT6007BEIJING64200002ZH0104最佳运输0202北京540523.7253031565502016233030412340603***0708A60086670902ME91320016A0112233449988770708123456786304A13A";
            //List<(int, char, char)> differences = FindDifferences(qrData, qrData1);
            //int i = 0;
            //foreach (var diff in qrData)
            //{
            //    if (qrData[i] != qrData1[i])
            //    {
            //        var str = qrData[i] + "-->" + qrData1[i];
            //    }
            //    i++;
            //}

            

            //foreach (var diff in differences)
            //{
            //    Console.WriteLine($"Position: {diff.Item1}, Char1: {diff.Item2}, Char2: {diff.Item3}");
            //}


            var payload = MerchantPayload.FromQR(qrData);

            ViewData["data"] = JsonConvert.SerializeObject(payload); //payload.ToString();

            return View();
        }


        static List<(int, char, char)> FindDifferences(string str1, string str2)
        {
            List<(int, char, char)> differences = new List<(int, char, char)>();
            StringComparer comparer = StringComparer.Ordinal;

            for (int i = 0; i < Math.Min(str1.Length, str2.Length); i++)
            {
                if (comparer.Compare(str1[i].ToString(), str2[i].ToString()) != 0)
                {
                    differences.Add((i, str1[i], str2[i]));
                }
            }

            return differences;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}