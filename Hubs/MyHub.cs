using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace OnlineAuctions.Hubs
{
    [HubName("myHub")]
    public class MyHub :Hub
    {
        private static Controllers.AuctionController con= new Controllers.AuctionController();
        

        public static void BidRefresh(int idAuction, int tokens, string bidder)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            hub.Clients.All.BidRefresh(idAuction, tokens, bidder);
        }


        public  void CloseAuction(int AuctionId)
        {
             con.Close(AuctionId);
        }
    }
}