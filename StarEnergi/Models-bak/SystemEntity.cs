using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarEnergi.Models
{
    public class SystemEntity
    {
        public enum color
        {
            RED,
            YELLOW,
            GREEN
        }

        public int id { get; set; }
        public int id_unit { get; set; }
        public string nama { get; set; }
        public string kode { get; set; }
        public string funct_description { get; set; }
        public string failure_scenario { get; set; }
        public string primary_impact { get; set; }
        public string secondary_impact { get; set; }

        public double avail_inherent { get; set; }
        public double avail_operational { get; set; }

        public int equipmentEH { get; set; }
        public int equipmentH { get; set; }
        public int equipmentMH { get; set; }
        public int equipmentM { get; set; }
        public int equipmentLN { get; set; }
        public string mtl 
        { 
            get
            {
                return getTraffic();
            } 
            set
            {             
                mtl = value;
            }
        }

         /**
	     * menampilkan traffic untuk system
	     * monitoring
	     * 		merah: rata-rata < 50 atau performance < 30
	     * 		kuning: 50 <= rata-rata < 70 atau 30 <= performance < 50
	     * 		hijau: rata-rata >= 70 atau performance >= 50
	     * safe operation
	     * 		merah: rata-rata < 70 atau safe guard < 70
	     * 		Kuning: 70 < rata-rata < 80 atau 70 < safe guard < 80
	     *		Hijau: rata-rata > 80 atau safe guard > 80
	     * total: minimum monitoring & safe operation
	     * @param
	     * @result 1=merah, 2=kuning, 3=hijau
	     * 
	     */
	    public string getTraffic()
	    {
            relmon_star_energiEntities db = new relmon_star_energiEntities();
		    int monitoring = (int)color.GREEN;
		    int safeOp = (int)color.GREEN;
		
		    List<equipment_groups> eqGroups = db.equipment_groups.Where(x => x.id_system == id).ToList();
		    foreach(equipment_groups eqGroup in eqGroups){

			    if(monitoring==(int)color.RED && safeOp==(int)color.RED){
				    break;
			    }
			
			    List<equipment> eqs = eqGroup.equipments.ToList();
			    foreach(equipment eq in eqs){

				    if(monitoring==(int)color.RED && safeOp==(int)color.RED){
					    break;
				    }
				
				    //monitoring
				    if(eq.equipment_readiness_nav.First().monitoring<50){
					    monitoring = (int)color.RED;
				    }else if(eq.equipment_readiness_nav.First().monitoring<70){
					    if(monitoring==(int)color.GREEN){
						    monitoring = (int)color.YELLOW;
					    }
				    }
				    if(eq.equipment_readiness_nav.First().performance<30){
					    monitoring = (int)color.RED;
				    }else if(eq.equipment_readiness_nav.First().performance<50){
					    if(monitoring==(int)color.GREEN){
						    monitoring = (int)color.YELLOW;
					    }
				    }
				
				    //safe operation
				    if(eq.equipment_readiness_nav.First().safe_operation<70){
					    safeOp = (int)color.RED;
                    }
                    else if (eq.equipment_readiness_nav.First().safe_operation < 80)
                    {
					    if( safeOp==(int)color.GREEN){
						    safeOp = (int)color.YELLOW;
					    }
				    }
				    if( eq.equipment_readiness_nav.First().safeguard <70){
					    safeOp = (int)color.RED;
                    }
                    else if (eq.equipment_readiness_nav.First().safeguard < 80)
                    {
					    if( safeOp==(int)color.GREEN){
						    safeOp = (int)color.YELLOW;
					    }
				    }
			    }
		    }
            int result = Math.Min(monitoring,  safeOp);
		    if(result == (int)color.RED){
                return "red";
            }
            else if (result == (int)color.YELLOW)
            {
                return "yellow";
            }
            else {
                return "green";
            }
		    
	    }
    }
}