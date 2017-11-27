using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.Views;
using StritWalk.iOS;
using CoreGraphics;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLocation;
using CoreBluetooth;
using CoreFoundation;
using AVFoundation;
using MultipeerConnectivity;

[assembly: ExportRenderer(typeof(MonkeyPage), typeof(MonkeyPageRenderer))]

namespace StritWalk.iOS
{
    public class MonkeyPageRenderer : PageRenderer
    {

        static readonly string uuid = "EBEFD083-70A2-47C8-9837-E7B5634DF524";
        static readonly string monkeyId = "SCRAMBLER TOWN";

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        CBPeripheralManager peripheralMgr;
        BTPeripheralDelegate peripheralDelegate;
        CLLocationManager locationMgr;
        CLProximity previousProximity;
        CLBeaconRegion beaconRegion;
        NSMutableDictionary peripheralData;
        float volume = 0.5f;
        float pitch = 1.0f;

        MCSession session;
        MCPeerID peer;
        MCBrowserViewController browser;
        MCAdvertiserAssistant assistant;
        MySessionDelegate sessionDelegate = new MySessionDelegate();
        MyBrowserDelegate browserDelegate = new MyBrowserDelegate();
        NSDictionary dict = new NSDictionary();
        static readonly string serviceType = "FindTheMonkey";

        public MonkeyPageRenderer()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            peripheralDelegate = new BTPeripheralDelegate();
            peripheralMgr = new CBPeripheralManager(peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs el)
        {
            base.OnElementChanged(el);

            if (el.OldElement != null || Element == null)
            {
                return;
            }


            if (!UserInterfaceIdiomIsPhone)
            {
                //power - the received signal strength indicator (RSSI) value (measured in decibels) of the beacon from one meter away
                var power = new NSNumber(-59);
                peripheralData = beaconRegion.GetPeripheralData(power);
                peripheralMgr.StartAdvertising(peripheralData);
            }
            else
            {
                StartMultipeerAdvertiser();
            }

            var monkeyUUID = new NSUuid(uuid);
            beaconRegion = new CLBeaconRegion(monkeyUUID, monkeyId);

            beaconRegion.NotifyEntryStateOnDisplay = true;
            beaconRegion.NotifyOnEntry = true;
            beaconRegion.NotifyOnExit = true;


            if (UserInterfaceIdiomIsPhone)
            {

                //InitPitchAndVolume();

                locationMgr = new CLLocationManager();

                //locationMgr.RequestWhenInUseAuthorization();

                locationMgr.RegionEntered += (object sender, CLRegionEventArgs e) =>
                {
                    Debug.WriteLine("new region");
                    Debug.WriteLine(e.Region.Identifier);
                    if (e.Region.Identifier == monkeyId)
                    {
                        UILocalNotification notification = new UILocalNotification() { AlertBody = "There's a monkey hiding nearby!" };
                        UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
                    }
                };

                locationMgr.DidRangeBeacons += (object sender, CLRegionBeaconsRangedEventArgs e) =>
                {
                    if (e.Beacons.Length > 0)
                    {

                        CLBeacon beacon = e.Beacons[0];
                        string message = "";

                        switch (beacon.Proximity)
                        {
                            case CLProximity.Immediate:
                                message = "You found the monkey!";

                                View.BackgroundColor = UIColor.Green;
                                break;
                            case CLProximity.Near:
                                message = "You're getting warmer";

                                View.BackgroundColor = UIColor.Yellow;
                                break;
                            case CLProximity.Far:
                                message = "You're freezing cold";

                                View.BackgroundColor = UIColor.Blue;
                                break;
                            case CLProximity.Unknown:
                                message = "I'm not sure how close you are to the monkey";
                                View.BackgroundColor = UIColor.Gray;
                                break;
                        }

                        if (previousProximity != beacon.Proximity)
                        {
                            Speak(message);

                            // demo send message using multipeer connectivity
                            if (beacon.Proximity == CLProximity.Immediate)
                                SendMessage();
                        }
                        previousProximity = beacon.Proximity;
                    }
                };

                locationMgr.StartMonitoring(beaconRegion);
                locationMgr.StartRangingBeacons(beaconRegion);
            }


        }



        void Speak(string text)
        {
            var speechSynthesizer = new AVSpeechSynthesizer();

            //          var voices = AVSpeechSynthesisVoice.GetSpeechVoices ();

            var speechUtterance = new AVSpeechUtterance(text)
            {
                Rate = AVSpeechUtterance.MaximumSpeechRate / 4,
                Voice = AVSpeechSynthesisVoice.FromLanguage("en-AU"),
                Volume = volume,
                PitchMultiplier = pitch
            };

            speechSynthesizer.SpeakUtterance(speechUtterance);
        }

        void InitPitchAndVolume()
        {

        }

        void StartMultipeerAdvertiser()
        {
            peer = new MCPeerID("Player1");
            session = new MCSession(peer);
            session.Delegate = sessionDelegate;
            assistant = new MCAdvertiserAssistant(serviceType, dict, session);
            assistant.Start();
        }

        void StartMultipeerBrowser()
        {
            peer = new MCPeerID("Monkey");
            session = new MCSession(peer);
            session.Delegate = sessionDelegate;
            browser = new MCBrowserViewController(serviceType, session);
            browser.Delegate = browserDelegate;
            browser.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            this.PresentViewController(browser, true, null);
        }

        void SendMessage()
        {
            var message = NSData.FromString(
                String.Format("{0} found the monkey", peer.DisplayName));
            NSError error;
            session.SendData(message, session.ConnectedPeers,
                MCSessionSendDataMode.Reliable, out error);
        }


        class MySessionDelegate : MCSessionDelegate
        {
            public override void DidChangeState(MCSession session, MCPeerID peerID, MCSessionState state)
            {
                switch (state)
                {
                    case MCSessionState.Connected:
                        Console.WriteLine("Connected: {0}", peerID.DisplayName);
                        break;
                    case MCSessionState.Connecting:
                        Console.WriteLine("Connecting: {0}", peerID.DisplayName);
                        break;
                    case MCSessionState.NotConnected:
                        Console.WriteLine("Not Connected: {0}", peerID.DisplayName);
                        break;
                }
            }

            public override void DidReceiveData(MCSession session, NSData data, MCPeerID peerID)
            {
                InvokeOnMainThread(() =>
                {
                    //var alert = new UIAlertView("", data.ToString(), null, "OK");
                    //alert.Show();
                });
            }

            public override void DidStartReceivingResource(MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress)
            {
            }

            public override void DidFinishReceivingResource(MCSession session, string resourceName, MCPeerID formPeer, NSUrl localUrl, NSError error)
            {
                error = null;
            }

            public override void DidReceiveStream(MCSession session, NSInputStream stream, string streamName, MCPeerID peerID)
            {
            }
        }

        class MyBrowserDelegate : MCBrowserViewControllerDelegate
        {
            public override void DidFinish(MCBrowserViewController browserViewController)
            {
                InvokeOnMainThread(() =>
                {
                    browserViewController.DismissViewController(true, null);
                });
            }

            public override void WasCancelled(MCBrowserViewController browserViewController)
            {
                InvokeOnMainThread(() =>
                {
                    browserViewController.DismissViewController(true, null);
                });
            }
        }

        class BTPeripheralDelegate : CBPeripheralManagerDelegate
        {
            public override void StateUpdated(CBPeripheralManager peripheral)
            {
                if (peripheral.State == CBPeripheralManagerState.PoweredOn)
                {
                    Console.WriteLine("powered on");
                }
            }
        }


    }
}
