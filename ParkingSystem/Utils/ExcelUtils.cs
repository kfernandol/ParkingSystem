using ClosedXML.Excel;
using ParkingSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSystem.Utils
{
    public static class ExcelUtils
    {
        public static void ParkingHistoryExportExcel(List<ParkingHistory> parkingHistory, string savePath)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte");

            //Create header
            worksheet.Cell(1, 1).Value = "Tipo";
            worksheet.Cell(1, 2).Value = "Fecha";
            worksheet.Cell(1, 3).Value = "Parqueos Disponibles";

            //Add data in list
            for (int i = 0; i < parkingHistory.Count; i++)
            {
                if (parkingHistory[i].IsEnter == true)
                    worksheet.Cell(i + 2, 1).Value = "Entrada";
                else
                    worksheet.Cell(i + 2, 1).Value = "Salida";

                worksheet.Cell(i + 2, 2).Value = parkingHistory[i].Date;
                worksheet.Cell(i + 2, 3).Value = parkingHistory[i].SpaceAvailable;
            }

            //Save File
            workbook.SaveAs(savePath);
        }
    }
}
