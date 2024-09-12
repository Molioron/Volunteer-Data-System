// See https://aka.ms/new-console-template for more information
using SQLitePCL;
using VDS_Backend.Src.Models.VDS;
using VDS_Backend.Src.Models.VDS.Contexts;
using VDS_Backend.Src.Models.VDS.DataTypes;
using VDS_Backend.Src.Models.VDS.Tables;
using VDS_Backend.Src.Utilities;
using VDS_Backend.Src.VDSServer;


// Initialize SQLitePCL
Batteries_V2.Init();

ServerApp.Run("localhost", 9000);