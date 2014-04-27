using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graph;
using DvrpUtils.ProblemDataModel;
using System.IO;
using System.Drawing;
//using DvrpUtils.ProblemSolutionModel;

namespace DvrpUtils.ProblemDataModel
{
    public class DVRPParser
    {
        enum DataSection
        {
            MAIN,
            DATA,
            DEMAND,
            LOCATION_COORD,
            DEPOT_LOCATION,
            VISIT_LOCATION,
            DURATION,
            DEPOT_TIME_WINDOW,
            TIME_AVAIL,
            DEPOTS,
            EOF,
            ROUTE,
            TOTAL_COST
        }
       

        const string defaultFileName = "okul12D.vrp";

        bool mIsBeginState = false;
        DataSection mCurrSect = DataSection.MAIN;

        public ProblemSolution ParseSolution(string filename)
        {
            ProblemSolution problemSolution = new ProblemSolution();
            List<Route> routes = new List<Route>();
            int totalCost = 0;

            /* example of output (named opt-filename.vrp)
              VRPSOLUTION 1 - jeśli 1 to solution jest, nie ma w p.p. - narazie nie zaimplementowane
              ROUTE #1: 13 14 1 22 84
              ROUTE #2: 2 17 32 2 64
              ROUTE #3: 5 8 21 27 45
              ROUTE #4: 63 35 11 36 58
              TOTAL_COST: 666
             */

            String line;
            try
            {
                using (StreamReader sr = new StreamReader(defaultFileName))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        checkLine(line);

                        switch (mCurrSect)
                        {
                            case DataSection.ROUTE:
                                Route route = new Route();
                                var tmpR = line.Split(' ');

                                for (int i = 2; i < tmpR.Length; ++i)
                                {
                                    route.Locations.Add(Convert.ToInt16(tmpR[i]));
                                }
                                route.RouteID = Convert.ToInt16(tmpR[1][1]);
                                routes.Add(route);
                                break;
                            case DataSection.TOTAL_COST:
                                var tmpC = line.Split(' ');
                                totalCost = Convert.ToInt16(tmpC[1]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:" + e.Message);
            }
            problemSolution.Routes = ReadRoutes(routes);
            problemSolution.TotalCost = totalCost;
            return problemSolution;
        }

        public ProblemData Parse(string filename)
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
                using (StreamReader sr = new StreamReader(defaultFileName))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        checkLine(line);

                        switch (mCurrSect)
                        {
                            case DataSection.MAIN:
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
                                }
                                break;
                            case DataSection.DATA:
                                var sum = mNumDepots + mNumVisits;
                                break;
                            case DataSection.DEPOTS:
                                caseDepots(line, ref ndepot);
                                break;
                            case DataSection.DEMAND:
                                caseDemand(line, ref nCustomer);
                                break;
                            case DataSection.LOCATION_COORD:
                                caseLocationCoord(line, ref nLocation);
                                break;
                            case DataSection.DEPOT_LOCATION:
                                caseDeptLocation(line, ref ndepot, nLocation);
                                break;
                            case DataSection.VISIT_LOCATION:
                                caseVisitLocation(line, ref nCustomer, nLocation);
                                break;
                            case DataSection.DURATION:
                                caseDuration(line, ref nCustomer);
                                break;
                            case DataSection.DEPOT_TIME_WINDOW:
                                caseDepotTime(line, ref ndepot);
                                break;
                            case DataSection.TIME_AVAIL:
                                caseTimeAvail(line, ref nCustomer);
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
            nProblemData.Customers = nCustomer;
            return nProblemData;

        }

        private void caseDepots(string line, ref List<Depot> ndepot)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                int depot_id = Convert.ToInt16(line.Split(' ').Last());
                Depot nDepot = new Depot();
                nDepot.DepotId = depot_id;

                ndepot.Add(nDepot);

            }
        }

        private void caseDemand(string line, ref List<Customer> nCustomer)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                int visit_id = Convert.ToInt16(line.Split(' ')[2]);
                int visit_demand = Convert.ToInt16(line.Split(' ').Last());

                Customer ncustomer = new Customer();
                ncustomer.CustomerId = visit_id;
                ncustomer.StartDate = visit_demand;
                nCustomer.Add(ncustomer);

            }
        }

        private void caseLocationCoord(string line, ref Dictionary<int, Point> nLocation)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                var tmp2 = line.Split(' ');
                int location_id = Convert.ToInt16(tmp2[2]);
                int coord_x = Convert.ToInt16(tmp2[3]);
                int coord_y = Convert.ToInt16(tmp2[4]);

                nLocation.Add(location_id, new Point(coord_x, coord_y));

            }
        }

        private void caseDeptLocation(string line, ref List<Depot> ndepot, Dictionary<int, Point> nLocation)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                int depot_id = Convert.ToInt16(line.Split(' ')[2]);
                int location_id = Convert.ToInt16(line.Split(' ').Last());
                ndepot.First(d => d.DepotId == depot_id).Location = nLocation[location_id];

            }
        }

        private void caseVisitLocation(string line, ref List<Customer> nCustomer, Dictionary<int, Point> nLocation)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                int visit_id = Convert.ToInt16(line.Split(' ')[2]);
                int location_id = Convert.ToInt16(line.Split(' ').Last());
                nCustomer.First(d => d.CustomerId == visit_id).Location = nLocation[location_id];

            }
        }

        private void caseDuration(string line, ref List<Customer> nCustomer)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                int visit_id = Convert.ToInt16(line.Split(' ')[2]);
                int duration = Convert.ToInt16(line.Split(' ').Last());

                nCustomer.First(d => d.CustomerId == visit_id).Duration = duration;

            }
        }

        private void caseDepotTime(string line, ref List<Depot> ndepot)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {
                var tmp2 = line.Split(' ');
                int depot_id = Convert.ToInt16(tmp2[2]);

                ndepot.First(d => d.DepotId == depot_id).StartTime = Convert.ToInt16(tmp2[3]);
                ndepot.First(d => d.DepotId == depot_id).EndTime = Convert.ToInt16(tmp2[4]);

            }
        }

        private void caseTimeAvail(string line, ref List<Customer> nCustomer)
        {
            if (mIsBeginState)
            {
                mIsBeginState = false;
            }
            else
            {

                int visit_id = Convert.ToInt16(line.Split(' ')[2]);
                int avail_time = Convert.ToInt16(line.Split(' ').Last());
                nCustomer.First(d => d.CustomerId == visit_id).Duration = avail_time;

            }
        }


        IEnumerable<Route> ReadRoutes(List<Route> list)
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
                mCurrSect = DataSection.DATA;
            }
            else if (line.CompareTo("DEPOTS") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.DEPOTS;
            }
            else if (line.CompareTo("DEMAND_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.DEMAND;

            }
            else if (line.CompareTo("LOCATION_COORD_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.LOCATION_COORD;

            }
            else if (line.CompareTo("DEPOT_LOCATION_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.DEPOT_LOCATION;

            }
            else if (line.CompareTo("VISIT_LOCATION_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.VISIT_LOCATION;

            }
            else if (line.CompareTo("DURATION_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.DURATION;

            }
            else if (line.CompareTo("DEPOT_TIME_WINDOW_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.DEPOT_TIME_WINDOW;

            }
            else if (line.CompareTo("TIME_AVAIL_SECTION") == 0)
            {
                mIsBeginState = true;
                mCurrSect = DataSection.TIME_AVAIL;

            }
            else if (line.CompareTo("EOF") == 0)
            {
                mCurrSect = DataSection.EOF;
            }
            else if (line.CompareTo("ROUTE") == 0)
            {
                mCurrSect = DataSection.ROUTE;
            }
            else if (line.CompareTo("TOTAL_COST") == 0)
            {
                mCurrSect = DataSection.TOTAL_COST;
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
