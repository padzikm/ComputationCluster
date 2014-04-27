using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graph;
using DvrpUtils.ProblemDataModel;
using System.IO;
using System.Drawing;

namespace DvrpUtils.ProblemDataModel
{
    class DVRPParesr
    {
        const int MAIN = 1;
        const int DATA = 2;
        const int DEMAND = 3;
        const int LOCATION_COORD = 4;
        const int DEPOT_LOCATION = 5;
        const int VISIT_LOCATION = 6;
        const int DURATION = 7;
        const int DEPOT_TIME_WINDOW = 8;
        const int TIME_AVAIL = 9;
        const int DEPOTS = 10;

        bool mIsBeginState = false;
        int mCurrSect = MAIN;

        public ProblemData Parse()
        {

            int mNumDepots = 1;
            int mNumVisits = 1;
            int mNumVehicles = 1;
            int mMaxCapacity = 999;
  
            ProblemData nProblemData = new ProblemData();
            List<Depot> ndepot = new List<Depot>();
            List<Customer> nCustomer = new List<Customer>();
            Dictionary<int, Point> nLocation = new Dictionary<int, Point>();
            List<Vehicle> nVehicle = new List<Vehicle>();

            String line;
            try
            {
                using (StreamReader sr = new StreamReader("okul12D.vrp"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        checkLine(line);

                        switch (mCurrSect)
                        {
                            case MAIN:
                                if (!mIsBeginState)
                                {
                                    string identifier;
                                    identifier = line.Split(':').First();

                                    if (identifier.CompareTo("NAME") == 0)
                                    {
                                        var name = line.Split(' ').Last();
                                    }
                                    else if (identifier.CompareTo("NUM_DEPOTS") == 0)
                                    {
                                        mNumDepots = Convert.ToInt16(line.Split(' ').Last());

                                    }
                                    else if (identifier.CompareTo("NUM_VISITS") == 0)
                                    {
                                        mNumVisits = Convert.ToInt16(line.Split(' ').Last());


                                    }
                                    else if (identifier.CompareTo("NUM_VEHICLES") == 0)
                                    {
                                        mNumVehicles = Convert.ToInt16(line.Split(' ').Last());

                                    }
                                    else if (identifier.CompareTo("CAPACITIES") == 0)
                                    {
                                        mMaxCapacity = Convert.ToInt16(line.Split(' ').Last());
                                        for (int i = 1; i <= mNumVehicles; i++)
                                        {
                                            var tmp = new Vehicle();
                                            tmp.VehicleId = i;
                                            tmp.MaxCapacity = mMaxCapacity;
                                            nVehicle.Add(tmp);
                                        }
                                    }
                                    else
                                    {
                                        //checkLine(rLine);
                                        //std::cout << "\n Identifier not needed!\n";
                                    }
                                    Console.WriteLine(line);
                                }
                                break;
                            case DATA:
                                var sum = mNumDepots + mNumVisits;
                                Console.WriteLine(line);
                                break;
                            case DEPOTS:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {
                                    int depot_id;

                                    depot_id = Convert.ToInt16(line.Split(' ').Last());
                                    Depot nDepot = new Depot();
                                    nDepot.DepotId = depot_id;

                                    ndepot.Add(nDepot);
                            
                                }
                                Console.WriteLine(line);
                                break;

                            case DEMAND:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {
                                    int visit_id;
                                    int visit_demand;

                                    visit_id = Convert.ToInt16(line.Split(' ')[2]);
                                    visit_demand = Convert.ToInt16(line.Split(' ').Last());

                                    Customer ncustomer = new Customer();
                                    ncustomer.CustomerId = visit_id;
                                    ncustomer.StartDate = visit_demand;//TODO: Check Demand
                                    nCustomer.Add(ncustomer);
                                  
                                }
                                Console.WriteLine(line);
                                break;
                            case LOCATION_COORD:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {

                                    int location_id;
                                    int coord_x, coord_y;

                                    var tmp2 = line.Split(' ');
                                    location_id = Convert.ToInt16(tmp2[2]);
                                    coord_x = Convert.ToInt16(tmp2[3]);
                                    coord_y = Convert.ToInt16(tmp2[4]);

                                    nLocation.Add(location_id, new Point(coord_x, coord_y));
                                 
                                }

                                Console.WriteLine(line);
                                break;
                            case DEPOT_LOCATION:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {
                                    int depot_id;
                                    int location_id;

                                    depot_id = Convert.ToInt16(line.Split(' ')[2]);
                                    location_id = Convert.ToInt16(line.Split(' ').Last());
                                    ndepot.First(d => d.DepotId == depot_id).Location = nLocation[location_id];

                                }
                                Console.WriteLine(line);
                                break;
                            case VISIT_LOCATION:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {
                                    int visit_id;
                                    int location_id;

                                    visit_id = Convert.ToInt16(line.Split(' ')[2]);
                                    location_id = Convert.ToInt16(line.Split(' ').Last());
                                    nCustomer.First(d => d.CustomerId == visit_id).Location = nLocation[location_id];

                                }
                                Console.WriteLine(line);
                                break;
                            case DURATION:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {
                                    int visit_id;
                                    int duration;

                                    visit_id = Convert.ToInt16(line.Split(' ')[2]);
                                    duration = Convert.ToInt16(line.Split(' ').Last());

                                    nCustomer.First(d => d.CustomerId == visit_id).Duration = duration;
                                    
                                }
                                Console.WriteLine(line);
                                break;
                            case DEPOT_TIME_WINDOW:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {

                                    int depot_id;
                                    int lower_bound;
                                    int upper_bound;

                                    var tmp2 = line.Split(' ');
                                    depot_id = Convert.ToInt16(tmp2[2]);
                                    lower_bound = Convert.ToInt16(tmp2[3]);
                                    upper_bound = Convert.ToInt16(tmp2[4]);

                                    // ndepot.First(d=>d.DepotId == depot_id).MinTime=
                                    //ndepot.First(d => d.DepotId == depot_id).MaxTime =
                                   //TODO: Set time
                                }
                                Console.WriteLine(line);
                                break;
                            case TIME_AVAIL:
                                if (mIsBeginState)
                                {
                                    mIsBeginState = false;
                                }
                                else
                                {
                                    int visit_id;
                                    int avail_time;

                                    visit_id = Convert.ToInt16(line.Split(' ')[2]);
                                    avail_time = Convert.ToInt16(line.Split(' ').Last());
                                    nCustomer.First(d => d.CustomerId == visit_id).Duration = avail_time;
                                  
                                }
                                Console.WriteLine(line);
                                break;
                            //Add EOFF

                        }
                    }
                    //Console.WriteLine(line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            nProblemData.Vehicles = nVehicle;
            nProblemData.Depots = ndepot;
            nProblemData.Customers = ReadCustomers(nCustomer);
            return nProblemData;

        }

        IEnumerable<Vehicle> ReadVehicles(List<Vehicle> list)
        {
            foreach (var e in list)
            {
                yield return e;
            }
        }
        IEnumerable<Depot> ReadDepot(List<Depot> list)
        {
            foreach (var e in list)
            {
                yield return e;
            }
        }
        IEnumerable<Customer> ReadCustomers(List<Customer> list)
        {
            foreach (var e in list)
            {
                yield return e;
            }
        }

        public void checkLine(String line)
        {
            if (line.CompareTo("DATA_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DATA;
            }
            else if (line.CompareTo("DEPOTS") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DEPOTS;
            }
            else if (line.CompareTo("DEMAND_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DEMAND;

            }
            else if (line.CompareTo("LOCATION_COORD_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = LOCATION_COORD;

            }
            else if (line.CompareTo("DEPOT_LOCATION_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DEPOT_LOCATION;
              
            }
            else if (line.CompareTo("VISIT_LOCATION_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = VISIT_LOCATION;

            }
            else if (line.CompareTo("DURATION_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DURATION;

            }
            else if (line.CompareTo("DEPOT_TIME_WINDOW_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DEPOT_TIME_WINDOW;

            }
            else if (line.CompareTo("TIME_AVAIL_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = TIME_AVAIL;

            }
            else if (line.CompareTo("EOF") == 0)
            {
                mCurrSect = 11;
            }

        }

        public string Parse(ProblemData problemData)
        {
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendLine(String.Format("NAME: {0}", problemData.Name));
            dataBuilder.AppendLine(String.Format("NUM_DEPOTS: {0}", problemData.Depots.Count()));
            dataBuilder.AppendLine(String.Format("NUM_CAPACITIES: 1"));
            dataBuilder.AppendLine(String.Format("NUM_LOCATIONS: {0}", problemData.Customers.Count()));
            dataBuilder.AppendLine(String.Format("NUM_VEHICLES: {0}", problemData.Vehicles.Count()));
            //The feet of vehicles is homogeneous
            dataBuilder.AppendLine(String.Format("CAPACITIES: {0}", problemData.Vehicles.First().Capacity));
            dataBuilder.AppendLine("DATA_SECTION");

            DemandParse(problemData, ref dataBuilder);
            DurationParse(problemData, ref dataBuilder);
            DepotsParse(problemData, ref dataBuilder);
            LocationParse(problemData, ref dataBuilder);
            DepotTimeParse(problemData, ref dataBuilder);
            CustomerTimeParse(problemData, ref dataBuilder);

            dataBuilder.AppendLine("EOF");
            return dataBuilder.ToString();
        }

        private void LocationParse(ProblemData problemData, ref StringBuilder dataBuilder)
        {
            
            dataBuilder.AppendLine("LOCATION_COORD_SECTION");
            foreach (var depot in problemData.Depots)
            {
                dataBuilder.AppendLine(String.Format("{0} {1} {2}", depot.DepotId, depot.Location.X, depot.Location.Y));
            }
            foreach (var customer in problemData.Customers)
            {
                dataBuilder.AppendLine(String.Format("{0} {1} {2}", customer.CustomerId, customer.Location.X,
                    customer.Location.Y));
            }
            
        }
        private void DepotsParse(ProblemData problemData, ref StringBuilder dataBuilder)
        {
            dataBuilder.AppendLine("DEPOTS");
            foreach (var depot in problemData.Depots)
            {
                dataBuilder.AppendLine(String.Format("{0}", depot.DepotId));
            }
            
        }
        private void DemandParse(ProblemData problemData, ref StringBuilder dataBuilder)
        {
            dataBuilder.AppendLine("DEMAND_SECTION");
            foreach (var customer in problemData.Customers)
            {
                dataBuilder.AppendLine(String.Format("{0} {1}", customer.CustomerId, customer.Size));
            }
        }
        private void DurationParse(ProblemData problemData, ref StringBuilder dataBuilder)
        {
            dataBuilder.AppendLine("DEMAND_SECTION");
            foreach (var customer in problemData.Customers)
            {
                dataBuilder.AppendLine(String.Format("{0} {1}", customer.CustomerId, customer.Duration));
            }

        }
        private void DepotTimeParse(ProblemData problemData, ref StringBuilder dataBuilder)
        {
            dataBuilder.AppendLine("DEPOT_TIME_WINDOW_SECTION");
            foreach (var depot in problemData.Depots)
            {
                dataBuilder.AppendLine(String.Format("{0} {1} {2}", depot.DepotId, depot.StartTime, depot.EndTime));
            }

        }
        private void CustomerTimeParse(ProblemData problemData, ref StringBuilder dataBuilder)
        {
            dataBuilder.AppendLine("TIME_AVAIL_SECTION");
            foreach (var customer in problemData.Customers)
            {
                dataBuilder.AppendLine(String.Format("{0} {1}", customer.CustomerId, customer.TimeAvailable));
            }

        }
    }
}
