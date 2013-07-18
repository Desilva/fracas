using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StarEnergi.Models;
using System.Data;

namespace StarEnergi.Utilities.Statistical_Engine{

    public class CalculateSertificate
    {
        private relmon_star_energiEntities db = new relmon_star_energiEntities();

        public CalculateSertificate() { }

        public CalculateSertificate(int idEquipment)
        {
            //konstanta 1 tahun dalam jam
		    double TAHUN = 8760;
		    double value = 0;
		
		     equipment e = db.equipments.Find(idEquipment);

			//hitung selisih sertifikasi dengan sekarang
			TimeSpan selisih = e.sertifikasi.Value.Subtract(DateTime.Now);
			
			//cek sertifikasi > 1 tahun warna hijau (100)
			//1 tahun sampai 6 bulan warna kuning (50)
			//kurang dari 6 bulan merah (0)
			if(selisih.TotalHours >= TAHUN){
				value = 100;
			}else if(selisih.TotalHours >= TAHUN/2){
				value = 50;
			}else{
				value = 0;
			}
			
			//update table readines navigator
            equipment_readiness_nav eqnav = db.equipment_readiness_nav.Where(a => a.id_equipment == idEquipment).FirstOrDefault();
            eqnav.sertifikasi = value;

            db.Entry(eqnav).State = EntityState.Modified;
            db.SaveChanges();
        }

        public string CalculateSertificateRet(int idEquipment)
        {
            //konstanta 1 tahun dalam jam
            double TAHUN = 8760;
            string value = "";

            equipment e = db.equipments.Find(idEquipment);

            //hitung selisih sertifikasi dengan sekarang
            TimeSpan selisih = e.sertifikasi.Value.Subtract(DateTime.Now);

            //cek sertifikasi > 1 tahun warna hijau (100)
            //1 tahun sampai 6 bulan warna kuning (50)
            //kurang dari 6 bulan merah (0)
            if (selisih.TotalHours >= TAHUN)
            {
                value = "green";
            }
            else if (selisih.TotalHours >= TAHUN / 2)
            {
                value = "yellow";
            }
            else
            {
                value = "red";
            }

            return value;
        }


        public string CalculateSertificateRet(DateTime sertifikasi)
        {
            //konstanta 1 tahun dalam jam
            double TAHUN = 8760;
            string value = "";

            //hitung selisih sertifikasi dengan sekarang
            TimeSpan selisih = sertifikasi.Subtract(DateTime.Now);

            //cek sertifikasi > 1 tahun warna hijau (100)
            //1 tahun sampai 6 bulan warna kuning (50)
            //kurang dari 6 bulan merah (0)
            if (selisih.TotalHours >= TAHUN)
            {
                value = "green";
            }
            else if (selisih.TotalHours >= TAHUN / 2)
            {
                value = "yellow";
            }
            else
            {
                value = "red";
            }

            return value;
        }
    }
}