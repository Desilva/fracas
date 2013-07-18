using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace StarEnergi.Utilities.Statistical_Engine
{
    public class CalculateEventData
    {
        public CalculateEventData(int id_equipment){
            
          
            SqlConnection myConnection = new SqlConnection(Config.conectionString);
            double[] data = null;
            try
            {
                myConnection.Open();

                DataSet ObjEquipments = new DataSet();
                DataSet ObjPartUnit = new DataSet();

                //get equipment
                #region equipment

                SqlCommand myCommand = new SqlCommand("SELECT equipments.id FROM equipments INNER JOIN equipment_event ON equipment_event.id_equipment = equipments.id WHERE equipments.id="+id_equipment);
                myCommand.Connection = myConnection;
                SqlDataAdapter objAdapater = new SqlDataAdapter();
                objAdapater.SelectCommand = myCommand;
                objAdapater.Fill(ObjEquipments);
                DataRow dr = ObjEquipments.Tables[0].Rows[0];

                //eguipment

                DataSet ObjDatasetEquipment = new DataSet();
                DataSet ObjDatasetPart = new DataSet();
                DataSet ObjPartUnits = new DataSet();
                double uptime = 0; double downtime = 0;


                myCommand = new SqlCommand("SELECT datetime_stop,datetime_ops,durasi,downtime FROM equipment_event WHERE id_equipment = " + dr[0].ToString() + " ORDER BY datetime_stop");
                myCommand.Connection = myConnection;
                objAdapater = new SqlDataAdapter();
                objAdapater.SelectCommand = myCommand;
                objAdapater.Fill(ObjDatasetEquipment);

                myCommand = new SqlCommand("SELECT part_unit_event.id_equipment_part, part_unit_event.datetime_stop, part_unit_event.datetime_ops, part_unit_event.durasi,part_unit_event.downtime FROM part_unit_event JOIN equipment_part on equipment_part.id = part_unit_event.id_equipment_part WHERE id_equipment =" + dr[0].ToString() + " ORDER BY part_unit_event.id_equipment_part, part_unit_event.datetime_stop");
                myCommand.Connection = myConnection;
                objAdapater = new SqlDataAdapter();
                objAdapater.SelectCommand = myCommand;
                objAdapater.Fill(ObjDatasetPart);

                myCommand = new SqlCommand("SELECT id_parts FROM equipment_part WHERE id_equipment = " + dr[0].ToString());
                myCommand.Connection = myConnection;
                objAdapater = new SqlDataAdapter();
                objAdapater.SelectCommand = myCommand;
                objAdapater.Fill(ObjPartUnits);

                    
                //data = new double[0];
                if ((ObjDatasetEquipment.Tables[0].Rows.Count > 0) || (ObjDatasetPart.Tables[0].Rows.Count > 0))
                {
                    #region Availibility Inheritance
                    int e = 0; int p = 0;
                    if (ObjDatasetEquipment.Tables[0].Rows.Count > 0)
                    {
                        e = ObjDatasetEquipment.Tables[0].Rows.Count - 1;
                    }
                    if (ObjDatasetPart.Tables[0].Rows.Count > 0)
                    {
                        p = ObjDatasetPart.Tables[0].Rows.Count - ObjPartUnits.Tables[0].Rows.Count;
                    }
                    data = new double[e + p]; int count = 0;

           

                    for (int i = 0; i < ObjDatasetEquipment.Tables[0].Rows.Count - 1; i++)
                    {
                        int q = ObjDatasetEquipment.Tables[0].Rows.Count;
                        TimeSpan ts = DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i + 1][0].ToString()).Subtract(DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][1].ToString()));
                        int day = ts.Days;
                        int hour = ts.Hours;
                        if (day != 0)
                        {
                            hour += day * 24;
                        }

                        data[i] = hour;
                        count = i;

                    }
                    if(e > 0){
                        count++;
                    }

                    //part
                    string id_part = "";
                    if (ObjDatasetPart.Tables[0].Rows.Count > 0)
                    {
                        id_part = ObjDatasetPart.Tables[0].Rows[0][0].ToString();
                    }
                    for (int j = 0; j < ObjDatasetPart.Tables[0].Rows.Count - 1; j++)
                    {
                        if (id_part != ObjDatasetPart.Tables[0].Rows[j + 1][0].ToString())
                        {
                            j++;
                        }
                        if (j < ObjDatasetPart.Tables[0].Rows.Count - 1)
                        {
                            string datestop = ObjDatasetPart.Tables[0].Rows[j + 1][1].ToString();
                            if (datestop != "")
                            {
                                TimeSpan ts = DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j + 1][1].ToString()).Subtract(DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][2].ToString()));
                                int day = ts.Days;
                                int hour = ts.Hours;
                                if (day != 0)
                                {
                                    hour += day * 24;
                                }
                                data[count] = hour;
                                count++;
                                id_part = ObjDatasetPart.Tables[0].Rows[j + 1][0].ToString();
                            }
                        }
                    }
                    List<double> f = data.ToList<double>();
                    f.Sort();
                    data = new double[data.Length];
                    data = f.ToArray();


                    double mttf = 0; double tetha = 0; double betha = 0; double mean = 0; double varian = 0; double lamda = 0;
                    string distType = "";
                    double[] a = new double[data.Length];
                    a = data;


                    MathFunction temp = new MathFunction();
                        
                    if (a.Length > 2)
                    {
                        double[] distribusi = new double[3];
                        double[] resultNormal = temp.MaxDistribusiNormal(a);
                        double[] resultExp = temp.MaxDistribusiExp(a);
                        double[] resultWeibull = temp.MaxDistribusiWeibull(a);

                        distribusi[0] = temp.getMaxValue(resultNormal);
                        distribusi[1] = temp.getMaxValue(resultExp);
                        distribusi[2] = temp.getMaxValue(resultWeibull);

                        int index = temp.getMaxIndex(distribusi);

                        if (index == 0) //normal
                        {   
                            double t = resultNormal[temp.getMaxIndex(resultNormal)];
                            double failure_rate = temp.FailureRate("n", resultNormal[temp.getMaxIndex(resultNormal)]);
                            //mttf = temp.MTTF("n", failure_rate);
                            distType = "Normal";
                            mean = temp.Mean;
                            varian = temp.Variance;
                            tetha = -1;
                            betha = -1;
                            lamda = -1;
                            mttf = temp.Mean;
                        }
                        else if (index == 1) //exp
                        {
                            double failure_rate = temp.FailureRate("e", resultExp[temp.getMaxIndex(resultExp)]);
                            mttf = temp.MTTF("e", failure_rate);
                            distType = "Exponensial";
                            lamda = failure_rate;
                            mean = -1;
                            varian = -1;
                            tetha = -1;
                            betha = -1;
                        }
                        else //weibull
                        {    
                            mttf = temp.MTTF("w", 0);
                            distType = "Weibull";
                            tetha = temp.Etha;
                            betha = temp.Betha;
                            lamda = -1;
                            mean = -1;
                            varian = -1;
                        }
                    }

                    //hitung MTTR
                    e = ObjDatasetEquipment.Tables[0].Rows.Count;
                    p = ObjDatasetPart.Tables[0].Rows.Count;
                    double[] data_downtime = new double[1];

                    if ((e > 1) || (p > 1))
                    {
                        data = new double[e + p]; count = 0;
                        data_downtime = new double[e + p];
                        if (ObjDatasetEquipment.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 1; i < ObjDatasetEquipment.Tables[0].Rows.Count; i++)
                            {
                                double hour = 0;
                                if (ObjDatasetEquipment.Tables[0].Rows[i][2].ToString() != "")
                                    hour = double.Parse(ObjDatasetEquipment.Tables[0].Rows[i][2].ToString());

                                data[i] = hour;

                                hour = 0;
                                if (ObjDatasetEquipment.Tables[0].Rows[i][3].ToString() != "")
                                    hour = double.Parse(ObjDatasetEquipment.Tables[0].Rows[i][3].ToString());

                                data_downtime[i] = hour;
                                count = i;
                            }
                        }
                        count++;
                        if (ObjDatasetPart.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 1; i < ObjDatasetPart.Tables[0].Rows.Count; i++)
                            {
                                double hour = 0;
                                if (ObjDatasetPart.Tables[0].Rows[i][3].ToString() != "")
                                    hour = double.Parse(ObjDatasetPart.Tables[0].Rows[i][3].ToString());

                                data[count] = hour;

                                hour = 0;
                                if (ObjDatasetPart.Tables[0].Rows[i][4].ToString() != "")
                                    hour = double.Parse(ObjDatasetPart.Tables[0].Rows[i][4].ToString());

                                data_downtime[i] = hour;
                                count++;
                            }
                        }

                        f = data.ToList<double>();
                        f.Sort();
                        data = new double[data.Length];
                        data = f.ToArray();
                    }
                    double mttr = 0;

                    temp = new MathFunction();

                    if (data.Length > 2 && mttf != 0)
                    {
                        double[] distribusi = new double[3];
                        double[] resultNormal = temp.MaxDistribusiNormal(data);
                        double[] resultExp = temp.MaxDistribusiExp(data);
                        double[] resultWeibull = temp.MaxDistribusiWeibull(data);

                        distribusi[0] = temp.getMaxValue(resultNormal);
                        distribusi[1] = temp.getMaxValue(resultExp);
                        distribusi[2] = temp.getMaxValue(resultWeibull);

                        int index = temp.getMaxIndex(distribusi);

                        if (index == 0)
                        {//normal
                            double t = resultNormal[temp.getMaxIndex(resultNormal)];
                            double failure_rate = temp.FailureRate("n", resultNormal[temp.getMaxIndex(resultNormal)]);
                            //mttr = pog.dist.MTTF("n", failure_rate);
                            mttr = temp.Mean;
                        }
                        else if (index == 1)//exp
                        {
                            double failure_rate = temp.FailureRate("e", resultExp[temp.getMaxIndex(resultExp)]);
                            mttr = temp.MTTF("e", failure_rate);
                        }
                        else
                        {//weibull 
                            mttr = temp.MTTF("w", 0);
                        }
                    }

                    //hitung availibility calculated
                    double availibility = mttf / (mttf + mttr) * 100;
                    if(availibility.ToString() == "NaN" ){
                        availibility = 100;
                    }
                    #endregion
                    #region Availibility Operation
                    //hitung availibility measured
                    for (int i = 0; i < ObjDatasetEquipment.Tables[0].Rows.Count; i++)
                    {
                        if (ObjDatasetEquipment.Tables[0].Rows[i][1].ToString() != "")
                        {
                            DateTime ops = DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][1].ToString());
                            DateTime now = DateTime.Now;
                                if (i < ObjDatasetEquipment.Tables[0].Rows.Count - 1)
                                {
                                    DateTime stop = DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i + 1][0].ToString());
                                    TimeSpan ts = DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i + 1][0].ToString()).Subtract(DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][1].ToString()));
                                    double day = ts.Days;
                                    uptime += ts.Hours;
                                    if (day != 0)
                                    {
                                        uptime += day * 24;
                                    }
                                }
                            else
                            {
                                if (ObjDatasetEquipment.Tables[0].Rows[i][1].ToString() != "")
                                {
                                    TimeSpan ts = now.Subtract(DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][1].ToString()));
                                    double day = ts.Days;
                                    uptime += ts.Hours;
                                    if (day != 0)
                                    {
                                        uptime += day * 24;
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < ObjDatasetEquipment.Tables[0].Rows.Count; i++)
                    {
                        if (ObjDatasetEquipment.Tables[0].Rows[i][0].ToString() != "")
                        {
                            DateTime stop = DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][0].ToString());
                            DateTime now = DateTime.Now;
                            if (ObjDatasetEquipment.Tables[0].Rows[i][1].ToString() != "")
                            {
                                TimeSpan ts = DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][1].ToString()).Subtract(DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][0].ToString()));
                                double day = ts.Days;
                                downtime += ts.Hours;
                                if (day != 0)
                                {
                                    downtime += day * 24;
                                }
                            }
                            else
                            {
                                TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(ObjDatasetEquipment.Tables[0].Rows[i][0].ToString()));
                                double day = ts.Days;
                                downtime += ts.Hours;
                                if (day != 0)
                                {
                                    downtime += day * 24;
                                }
                            }
                        }
                    }            

                    id_part = ""; string id_part_temp = "";
                    if (ObjDatasetPart.Tables[0].Rows.Count > 0)
                    {
                        id_part = ObjDatasetPart.Tables[0].Rows[0][0].ToString();
                    }
                    for (int j = 0; j < ObjDatasetPart.Tables[0].Rows.Count; j++)
                    {
                        if ((j + 1) < ObjDatasetPart.Tables[0].Rows.Count)
                        {
                            id_part_temp = ObjDatasetPart.Tables[0].Rows[j + 1][0].ToString();
                        }
                        else
                        {
                            id_part_temp = "";
                        }


                        if (ObjDatasetPart.Tables[0].Rows[j][2].ToString() != "")
                        {
                            DateTime ops = DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][2].ToString());
                            DateTime now = DateTime.Now;
                                if (id_part == id_part_temp)
                                {
                                    DateTime stop = DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j + 1][1].ToString());
                                    TimeSpan ts = DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j + 1][1].ToString()).Subtract(DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][2].ToString()));
                                    double day = ts.Days;
                                    uptime += ts.Hours;
                                    if (day != 0)
                                    {
                                        uptime += day * 24;
                                    }
                                }
                                else
                                {
                                    if (ObjDatasetPart.Tables[0].Rows[j][2].ToString() != "")
                                    {
                                        TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][2].ToString()));
                                        double day = ts.Days;
                                        uptime += ts.Hours;
                                        if (day != 0)
                                        {
                                            uptime += day * 24;
                                        }
                                    }
                                }
                            id_part = id_part_temp;
                        }
                    }

                    id_part = "";id_part_temp = "";
                    if (ObjDatasetPart.Tables[0].Rows.Count > 0)
                    {
                        id_part = ObjDatasetPart.Tables[0].Rows[0][0].ToString();
                    }
                    for (int j = 0; j < ObjDatasetPart.Tables[0].Rows.Count; j++)
                    {
                        if (ObjDatasetPart.Tables[0].Rows[j][1].ToString() != "")
                        {
                            DateTime stop = DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][1].ToString());
                            DateTime now = DateTime.Now;

                            if (ObjDatasetPart.Tables[0].Rows[j][2].ToString() != "")
                            {
                                TimeSpan ts = DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][2].ToString()).Subtract(DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][1].ToString()));
                                double day = ts.Days;
                                downtime += ts.Hours;
                                if (day != 0)
                                {
                                    downtime += day * 24;
                                }
                            }
                            else
                            {
                                TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(ObjDatasetPart.Tables[0].Rows[j][1].ToString()));
                                double day = ts.Days;
                                downtime += ts.Hours;
                                if (day != 0)
                                {
                                    downtime += day * 24;
                                }
                            }
                        }
                    }

                    //hitung availibility measured
                    double availibility_measured = 0;
                    double mdt = 0;

                    if (data_downtime.Length > 1) { 
                        double[] data_downtime_clean = new double[data_downtime.Length-1];
                        for(int i = 0;i<data_downtime_clean.Length;i++){
                            data_downtime_clean[i] = data_downtime[i+1];
                        }
                        mdt = temp.MeanFunction(data_downtime_clean);
                    }
                    if ((mttf > 0) && (mdt > 0))
                        availibility_measured = mttf / (mttf + mdt) * 100;
                        //availibility_measured = uptime / (uptime + downtime) * 100;
                    else
                        availibility_measured = 100;
                    #endregion

                    myCommand = new SqlCommand("UPDATE equipments SET pdf='" + distType + "', tetha=" + temp.ConvertData(tetha) + ", beta=" + temp.ConvertData(betha) + ", mtbf=" + temp.ConvertData(mttf) + ", mttr=" + temp.ConvertData(mttr) + ", mdt=" + temp.ConvertData(mdt) + ", mean=" + temp.ConvertData(mean) + ", varian=" + temp.ConvertData(varian) + ", lamda=" + temp.ConvertData(lamda) + " WHERE id =" + dr[0].ToString());
                    myCommand.Connection = myConnection;
                    myCommand.ExecuteNonQuery();

                    myCommand = new SqlCommand("INSERT INTO equipment_paf (id_equipment, tanggal, avail_calculated, avail_measured) VALUES (" + dr[0].ToString() + ", '" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt") + "', " + temp.ConvertData(availibility) + ", " + temp.ConvertData(availibility_measured) + ")");
                    //Console.WriteLine(myCommand.CommandText);
                    //Console.ReadLine();
                    myCommand.Connection = myConnection;
                    myCommand.ExecuteNonQuery();
                }
                #endregion

                //get list part
                #region list part
                myCommand = new SqlCommand("SELECT equipment_part.id FROM equipment_part INNER JOIN part_unit_event ON part_unit_event.id_equipment_part = equipment_part.id where equipment_part.id_equipment = "+id_equipment);
                myCommand.Connection = myConnection;
                objAdapater = new SqlDataAdapter();
                objAdapater.SelectCommand = myCommand;
                objAdapater.Fill(ObjPartUnit);

                //Part Unit
                foreach (DataRow drp in ObjPartUnit.Tables[0].Rows)
                {
                    #region partunit MTTR MTTF
                    DataSet ObjPartUnitEvents = new DataSet();

                    myCommand = new SqlCommand("SELECT datetime_stop,datetime_ops,durasi FROM part_unit_event WHERE id_equipment_part = " + drp[0].ToString() + " ORDER BY datetime_stop");
                    myCommand.Connection = myConnection;
                    objAdapater = new SqlDataAdapter();
                    objAdapater.SelectCommand = myCommand;
                    objAdapater.Fill(ObjPartUnitEvents);

                    if (ObjPartUnitEvents.Tables[0].Rows.Count > 0)
                    {
                        data = new double[ObjPartUnitEvents.Tables[0].Rows.Count - 1];

                        for (int i = 0; i < ObjPartUnitEvents.Tables[0].Rows.Count - 1; i++)
                        {
                            TimeSpan ts = DateTime.Parse(ObjPartUnitEvents.Tables[0].Rows[i + 1][0].ToString()).Subtract(DateTime.Parse(ObjPartUnitEvents.Tables[0].Rows[i][1].ToString()));
                            int day = ts.Days;
                            int hour = ts.Hours;
                            if (day != 0)
                            {
                                hour += day * 24;
                            }

                            data[i] = hour;

                        }

                        List<double> f = data.ToList<double>();
                        f.Sort();
                        data = new double[data.Length];
                        data = f.ToArray();


                        double mttf = 0; double tetha = 0; double betha = 0;
                        string distType = "";
                        double[] a = new double[data.Length];
                        a = data;

                        MathFunction temp = new MathFunction();

                        if (a.Length > 2)
                        {
                            double[] distribusi = new double[3];
                            double[] resultNormal = temp.MaxDistribusiNormal(a);
                            double[] resultExp = temp.MaxDistribusiExp(a);
                            double[] resultWeibull = temp.MaxDistribusiWeibull(a);

                            distribusi[0] = temp.getMaxValue(resultNormal);
                            distribusi[1] = temp.getMaxValue(resultExp);
                            distribusi[2] = temp.getMaxValue(resultWeibull);

                            int index = temp.getMaxIndex(distribusi);

                            if (index == 0)
                            {//normal
                                double t = resultNormal[temp.getMaxIndex(resultNormal)];
                                double failure_rate = temp.FailureRate("n", resultNormal[temp.getMaxIndex(resultNormal)]);
                                //mttf = temp.MTTF("n", failure_rate);
                                distType = "Normal";
                                tetha = temp.Mean;
                                betha = temp.Variance;
                                mttf = temp.Mean;
                            }
                            else if (index == 1)//exp
                            {
                                double failure_rate = temp.FailureRate("e", resultExp[temp.getMaxIndex(resultExp)]);
                                mttf = temp.MTTF("e", failure_rate);
                                distType = "Exponensial";
                                tetha = temp.Mean;
                                betha = 0;
                            }
                            else
                            {//weibull 
                                mttf = temp.MTTF("w", 0);
                                distType = "Weibull";
                                tetha = temp.Etha;
                                betha = temp.Betha;
                            }

                            //hitung MTTR
                            if (ObjPartUnitEvents.Tables[0].Rows.Count > 1)
                            {
                                data = new double[ObjPartUnitEvents.Tables[0].Rows.Count];

                                for (int i = 1; i < ObjPartUnitEvents.Tables[0].Rows.Count; i++)
                                {
                                    double hour = double.Parse(ObjPartUnitEvents.Tables[0].Rows[i][2].ToString());
                                    data[i] = hour;
                                }

                                f = data.ToList<double>();
                                f.Sort();
                                data = new double[data.Length];
                                data = f.ToArray();
                            }
                            double mttr = 0;

                            temp = new MathFunction();

                            if (data.Length > 2)
                            {
                                distribusi = new double[3];
                                resultNormal = temp.MaxDistribusiNormal(data);
                                resultExp = temp.MaxDistribusiExp(data);
                                resultWeibull = temp.MaxDistribusiWeibull(data);

                                distribusi[0] = temp.getMaxValue(resultNormal);
                                distribusi[1] = temp.getMaxValue(resultExp);
                                distribusi[2] = temp.getMaxValue(resultWeibull);

                                index = temp.getMaxIndex(distribusi);

                                if (index == 0)
                                {//normal
                                    double t = resultNormal[temp.getMaxIndex(resultNormal)];
                                    double failure_rate = temp.FailureRate("n", resultNormal[temp.getMaxIndex(resultNormal)]);
                                    mttr = temp.MTTF("n", failure_rate);
                                }
                                else if (index == 1)//exp
                                {
                                    double failure_rate = temp.FailureRate("e", resultExp[temp.getMaxIndex(resultExp)]);
                                    mttr = temp.MTTF("e", failure_rate);
                                }
                                else
                                {//weibull 
                                    mttr = temp.MTTF("w", 0);
                                }

                            }

                            myCommand = new SqlCommand("UPDATE equipment_part SET mtbf=" + temp.ConvertData(mttf) + ", mttr=" + temp.ConvertData(mttr) + " WHERE id =" + drp[0].ToString());
                            myCommand.Connection = myConnection;
                            myCommand.ExecuteNonQuery();
                        }
                    }
                    #endregion
                }
                #endregion

                //belum
                #region Spare Equipment
                /*DataSet ObjParts = new DataSet();

                //get list parts
                myCommand = new SqlCommand("SELECT id FROM rm_parts");
                myCommand.Connection = myConnection;
                objAdapater = new SqlDataAdapter();
                objAdapater.SelectCommand = myCommand;
                objAdapater.Fill(ObjParts);


                foreach(DataRow dr in ObjParts.Tables[0].Rows)
                {
                    
                    DataSet MTBFPart = new DataSet();
                    DataSet PartEvents = new DataSet();

                    double mttf = 0;
                    double uptimeSpare = 0;
                    int sumPart = 0;

                    myCommand = new SqlCommand("SELECT rm_part_unit.mtbf FROM rm_equipment_parts LEFT JOIN rm_part_unit ON rm_part_unit.id_equipment_parts = rm_equipment_parts.id WHERE rm_equipment_parts.id_parts = "+dr[0].ToString());
                    myCommand.Connection = myConnection;
                    objAdapater = new SqlDataAdapter();
                    objAdapater.SelectCommand = myCommand;
                    objAdapater.Fill(MTBFPart);

                    if (MTBFPart.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in MTBFPart.Tables[0].Rows)
                        {
                            if (r[0].ToString() != "")
                                mttf += double.Parse(r[0].ToString());
                        }

                        mttf = mttf / MTBFPart.Tables[0].Rows.Count; sumPart = MTBFPart.Tables[0].Rows.Count;
                    }
                    
                    myCommand = new SqlCommand("SELECT rm_part_unit.id, datetime_stop, datetime_ops FROM rm_equipment_parts LEFT JOIN rm_part_unit ON rm_part_unit.id_equipment_parts = rm_equipment_parts.id LEFT JOIN rm_part_unit_event ON rm_part_unit_event.id_part_unit = rm_part_unit.id WHERE rm_equipment_parts.id_parts = "+dr[0].ToString()+" ORDER BY datetime_stop");
                    myCommand.Connection = myConnection;
                    objAdapater = new SqlDataAdapter();
                    objAdapater.SelectCommand = myCommand;
                    objAdapater.Fill(PartEvents);

                    if (PartEvents.Tables[0].Rows.Count > 0)
                    {
                        string id_part_temp = "";
                        string id_part = PartEvents.Tables[0].Rows[0][0].ToString();

                        for (int j = 0; j < PartEvents.Tables[0].Rows.Count; j++)
                        {
                            if ((j + 1) < PartEvents.Tables[0].Rows.Count)
                            {
                                id_part_temp = PartEvents.Tables[0].Rows[j + 1][0].ToString();
                            }
                            else
                            {
                                id_part_temp = "";
                            }


                            if (PartEvents.Tables[0].Rows[j][2].ToString() != "")
                            {
                                DateTime ops = DateTime.Parse(PartEvents.Tables[0].Rows[j][2].ToString());
                                DateTime now = DateTime.Now;
                                if (ops.Year == now.Year)
                                {
                                    if (id_part == id_part_temp)
                                    {
                                        DateTime stop = DateTime.Parse(PartEvents.Tables[0].Rows[j + 1][1].ToString());
                                        TimeSpan ts = DateTime.Parse(PartEvents.Tables[0].Rows[j + 1][1].ToString()).Subtract(DateTime.Parse(PartEvents.Tables[0].Rows[j][2].ToString()));
                                        double day = ts.Days;
                                        uptimeSpare += ts.Hours;
                                        if (day != 0)
                                        {
                                            uptimeSpare += day * 24;
                                        }
                                    }
                                    else
                                    {
                                        if (PartEvents.Tables[0].Rows[j][2].ToString() != "")
                                        {
                                            TimeSpan ts = DateTime.Now.Subtract(DateTime.Parse(PartEvents.Tables[0].Rows[j][2].ToString()));
                                            double day = ts.Days;
                                            uptimeSpare += ts.Hours;
                                            if (day != 0)
                                            {
                                                uptimeSpare += day * 24;
                                            }
                                        }
                                    }
                                }
                                id_part = id_part_temp;
                            }
                        }
                    }          
                    //SpareFunction sf = new SpareFunction();
                    //if (mttf > 0)
                        //sf.CalculateAverageSpares(mttf, sumPart, 4392);
                    //else
                        //sf.AverageSpare = 0;
                    //Console.WriteLine("Average Spare : " + sf.AverageSpare);
                    //Console.WriteLine();

                    //sf.AverageSpare = 120;

                    //myCommand = new SqlCommand("SELECT assurance_level FROM rm_parts WHERE id="+dr[0].ToString());
                    //myCommand.Connection = myConnection;
                    //double assuranceLevel = (double)myCommand.ExecuteScalar();

                    //if (sf.AverageSpare > 0)
                        //assuranceLevel = sf.CalculatedNs(assuranceLevel / 100);
                    //else
                        //sf.NS = 0;
                    //myCommand = new SqlCommand("UPDATE rm_parts SET required ="+sf.NS+" WHERE id="+dr[0].ToString());
                    //myCommand.Connection = myConnection;
                    //myCommand.ExecuteNonQuery();
                    
                }//end for*/
                #endregion

                //belum
                #region Calculate RBD
                //AvailibilityRBD countRBD = new AvailibilityRBD();
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}