using System;
using FluentAssertions;
using FluentAutomation;
using FluentAutomation.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace AutomationWithChromeExtension
{
    [TestClass]
    public class SwitchCountryTest : FluentTest
    {

        private const string BetterNetCrxPath = "gjknjjomckknofjidppipffbpoekiipm.crx";

        private const string BetterNetExtensionUrl =
            "chrome-extension://gjknjjomckknofjidppipffbpoekiipm/panel/index.html";

        private const string TargetUrl =
            "http://www.google.com";


        [TestInitialize]
        public void TestInitialize()
        {
            SeleniumWebDriver.Bootstrap( SeleniumWebDriver.Browser.Chrome , TimeSpan.FromSeconds( 15 ) );

            LoadBetterNetExtension();
        }

        [TestMethod]
        public void TestWithoutBetterNet()
        {
            InteractWithBetterNet( isTurnOn: false , country: string.Empty );

            CheckFooterText( expectedText: "台灣" );
        }

        [TestMethod]
        public void TestSwitchToIndia()
        {
            InteractWithBetterNet( isTurnOn: true , country: "India" );

            CheckFooterText( expectedText: "India" );
        }

        [TestMethod]
        public void TestSwitchToGermany()
        {
            InteractWithBetterNet( isTurnOn: true , country: "Germany" );

            CheckFooterText( expectedText: "Deutschland" );
        }


        private void CheckFooterText( string expectedText )
        {
            I.Open( TargetUrl ).Find( "span.Q8LRLc" ).Element.Text.Should().Be( expectedText );
        }

        private void LoadBetterNetExtension()
        {
            FluentSettings.Current.ContainerRegistration = ( container ) =>
            {
                container.Register<ICommandProvider , CommandProvider>();
                container.Register<IAssertProvider , AssertProvider>();
                container.Register<IFileStoreProvider , LocalFileStoreProvider>();


                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument( "start-maximized" );
                chromeOptions.AddExtension( BetterNetCrxPath );

                container.Register<IWebDriver>( ( c , o ) => new ChromeDriver( chromeOptions ) );
            };
        }

        private void InteractWithBetterNet( bool isTurnOn , string country )
        {
            TurnOnOffBetterNet( isTurnOn );
            SwitchToCountry( country );
        }

        private void SwitchToCountry( string country )
        {
            if( string.IsNullOrEmpty( country ) == true )
            {
                return;
            }

            I.Open( BetterNetExtensionUrl )
                .Click( "div .globe" )
                .Click( $"div .locationsContainer span:contains('{country}')" )
                .Wait( TimeSpan.FromSeconds( 3 ) );
        }

        private void TurnOnOffBetterNet( bool isTurnOn )
        {
            bool isConnected = I.Open( BetterNetExtensionUrl )
                 .Find( "div .button" ).Element.Text == "DISCONNECT";

            if( isTurnOn == isConnected )
            {
                return;
            }

            I.Click( isTurnOn ? "div.button.disconnected" : "div.button.connected" )
                .Wait( TimeSpan.FromSeconds( 5 ) );
        }


    }
}
