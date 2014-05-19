using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

        bool mIsBeginState = false;
        DataSection mCurrSect = DataSection.MAIN;

        public ProblemData Parse(string fileContent)
        {
            int mNumDepots = 1;
            int mNumVisits = 1;
            int mNumVehicles = 1;
            int mMaxCapacity = 999;

            ProblemData nProblemData = new ProblemData();
            List<Depot> ndepot = new List<Depot>();
            List<Customer> nCustomer = new List<Customer>();
            Dictionary<int, Point> nLocation = new Dictionary<int, Point>();

            string line;
            try
            {
                using (StringReader sr = new StringReader(fileContent))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        checkLine(line);

                        if (line.Contains("COMMENT"))
                            continue;
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
                                        nProblemData.Capacity = mMaxCapacity;
                                    }
                                    else if (line.Split(' ').First().CompareTo("VRPTEST") == 0)
                                    {
                                        nProblemData.Name = line.Split(' ')[1];
                                    }
                                    else
                                    {
                                        // Niepotrzebne linie
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
                            case DataSection.EOF:
                                break;
                            default:
                                // Niepotrzebne linie
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            nProblemData.VehiclesCount = mNumVehicles;
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
                ncustomer.Demand = visit_demand;
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

                nCustomer.First(d => d.CustomerId == visit_id).TimeAvailable = avail_time;
            }
        }

        private void checkLine(String line)
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
        }
    }
}
