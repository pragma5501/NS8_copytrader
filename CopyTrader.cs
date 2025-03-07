#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 

namespace CopyTraderUtil
{
	public enum TypePlan
	{
		Full25k 	= 1500,
		Full50k 	= 2500,
		Full75k 	= 2750,
		Full100k 	= 3000,
		Full150k 	= 5000,
		Full250k 	= 6500,
		Full300k 	= 7500,
		Static100k 	= 100000,
	}
}

public class FuturesExpiryConverter : TypeConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {

        var allowedSymbols = new HashSet<string> { "NQ", "MNQ", "ES" };


        var expiries = Instrument.All
            .Where(i => i.MasterInstrument.InstrumentType == InstrumentType.Future &&
                        allowedSymbols.Contains(i.MasterInstrument.Name)) 
            .Select(i => i.FullName)
            .Distinct()
            .ToList();

        return new StandardValuesCollection(expiries);
    }
}

public class AccountListConverter : TypeConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {

        var connectedAccounts = Account.All
            .Where(a => a.ConnectionStatus == ConnectionStatus.Connected)
            .Select(a => a.Name)
            .ToList();

        return new StandardValuesCollection(connectedAccounts);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class CopyTraderAccount
{

    [TypeConverter(typeof(AccountListConverter))]
    [Display(Name = "Account", GroupName = "Account List")]
    public string AccountName { get; set; } = "";

	
	//[TypeConverter(typeof(FuturesExpiryConverter))]
    //[Display(Name = "종목", GroupName = "Setting")]
    //public string InstrumentName { get; set; } = "";
	
	//[Display(Name = "Quantity", GroupName = "Setting")]
	//public int Quantity { get; set; } = 0;
	
	public Account account;

	private string status = "";
	public override string ToString() => string.IsNullOrEmpty(AccountName) ? "None" : AccountName;

	public void SetAccount()
	{
		account = Account.All.FirstOrDefault(
			a => a.Name == AccountName
		);
		
		if (account == null)
		{
			status = "NULL";
		} 
		else 
		{
			status = "Activate : CTA";
		}
	}
	
}



namespace NinjaTrader.NinjaScript.Strategies
{
	using CopyTraderUtil;
	public class CopyTrader : Strategy
	{

		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA1 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA2 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA3 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA4 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA5 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA6 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA7 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA8 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA9 { get; set; } = new CopyTraderAccount();
		[Display(Name="Follower Account",  GroupName = "Account Setting")]
		public CopyTraderAccount FollowerCTA10 { get; set; } = new CopyTraderAccount();

		[Display(Name="Leader Account",  GroupName = "Account Setting")]
		public CopyTraderAccount LeaderCTA { get; set; } = new CopyTraderAccount();
		
		private CopyTraderAccount leaderCTA;
		private List<CopyTraderAccount> followerCTAs = new List<CopyTraderAccount>();
		
		
		private bool LoadCopyTraderInfo()
		{
			leaderCTA = LeaderCTA;
			followerCTAs.Add(FollowerCTA1);
			followerCTAs.Add(FollowerCTA2);
			followerCTAs.Add(FollowerCTA3);
			followerCTAs.Add(FollowerCTA4);
			followerCTAs.Add(FollowerCTA5);
			followerCTAs.Add(FollowerCTA6);
			followerCTAs.Add(FollowerCTA7);
			followerCTAs.Add(FollowerCTA8);
			followerCTAs.Add(FollowerCTA9);
			followerCTAs.Add(FollowerCTA10);
			
			leaderCTA.SetAccount();
			foreach (var followerCTA in followerCTAs) 
			{
				followerCTA.SetAccount();
    				if (followerCTA.AccountName == leaderCTA.AccountName) return false;
			}
			
            if (leaderCTA.account == null)
            {
                return false;
            }

			return true;
		}
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"여기에 전략 대한 설명을 입력하십시오.";
				Name										= "CopyTrader";

				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.Realtime)
			{
				bool ret = LoadCopyTraderInfo();
				if (ret == false) return;
				leaderCTA.account.OrderUpdate += OnLeaderOrderUpdate;
			}
			else if (State == State.Terminated)
			{
				if (leaderCTA.account != null)
				{
					leaderCTA.account.OrderUpdate -= OnLeaderOrderUpdate;
				}
				Print("state terminated");
			}
		}

		protected override void OnBarUpdate()
		{
		}

		private void OnLeaderOrderUpdate(object sender, OrderEventArgs e)
		{
			if (e.Order.OrderState != OrderState.Filled )
				 return;
			
			foreach (var followerCTA in followerCTAs)
			{
				DoCopyOrder(followerCTA, e.Order);
			}
		}
		private void DoCopyOrder(CopyTraderAccount followerCTA, Order leaderOrder) 
		{
			if (followerCTA.account == null || leaderOrder == null) return;
			Order copyedOrder = CopyOrder(followerCTA, leaderOrder);
			followerCTA.account.Submit(new[] {copyedOrder});
				
		}
		private Order CopyOrder(CopyTraderAccount followerCTA, Order order)
		{
			Order copyedOrder = followerCTA.account.CreateOrder(
				order.Instrument,
				order.OrderAction,
				order.OrderType,
				order.OrderEntry,
				order.TimeInForce,
				order.Quantity,
				order.LimitPrice,
				order.StopPrice,
				order.Oco,
				order.Name,
				order.Gtd,
				order.CustomOrder
			);
			
			return copyedOrder;
		}
	}
}

