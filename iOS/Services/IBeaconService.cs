using System;
using System.Diagnostics;
using CoreBluetooth;
using CoreLocation;
using MultipeerConnectivity;
using UIKit;
using Xamarin.Forms;
using Foundation;
using Xamarin.Forms.Platform.iOS;
using StritWalk;
using StritWalk.Views;
using StritWalk.iOS;
using CoreGraphics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFoundation;
using AVFoundation;

[assembly: Dependency(typeof(StritWalk.iOS.IBeaconService))]

namespace StritWalk.iOS
{
    public class IBeaconService : IIBeaconService
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
        bool inRange = false;
        DateTime lastOpen;

        MCSession session;
        MCPeerID peer;
        MCBrowserViewController browser;
        MCAdvertiserAssistant assistant;
        MySessionDelegate sessionDelegate = new MySessionDelegate();
        MyBrowserDelegate browserDelegate = new MyBrowserDelegate();
        NSDictionary dict = new NSDictionary();
        static readonly string serviceType = "FindTheMonkey";

        public event EventHandler<GeographicLocation> LocationChanged;
        public event EventHandler<string> BeaconRanged;

        public IBeaconService()
        {
            //UIApplication.SharedApplication.IdleTimerDisabled = true;
            peripheralDelegate = new BTPeripheralDelegate();
            peripheralMgr = new CBPeripheralManager(peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
            Initializing();
        }

        public void Initializing()
        {
            var monkeyUUID = new NSUuid(uuid);
            beaconRegion = new CLBeaconRegion(monkeyUUID, monkeyId);
            beaconRegion.NotifyEntryStateOnDisplay = true;
            beaconRegion.NotifyOnEntry = true;
            beaconRegion.NotifyOnExit = true;

            locationMgr = new CLLocationManager();
            locationMgr.RequestAlwaysAuthorization();
            locationMgr.PausesLocationUpdatesAutomatically = false;
            locationMgr.AllowsBackgroundLocationUpdates = true;
            locationMgr.DesiredAccuracy = CLLocation.AccuracyBest;
            locationMgr.DistanceFilter = CLLocationDistance.FilterNone;
            locationMgr.ShowsBackgroundLocationIndicator = false;

            locationMgr.LocationsUpdated +=
                (object sender, CLLocationsUpdatedEventArgs args) =>
                {
                 CLLocationCoordinate2D coordinate = args.Locations[0].Coordinate;
                    EventHandler<GeographicLocation> handler = LocationChanged;
                    if (handler != null)
                    {
                        handler(this, new GeographicLocation(coordinate.Latitude,
                                                             coordinate.Longitude));
                    }
                };

            locationMgr.RegionEntered += (object sender, CLRegionEventArgs e) =>
            {
                UILocalNotification notification = new UILocalNotification() { AlertBody = "You entered this region: " + e.Region.Identifier };
                UIApplication.SharedApplication.PresentLocalNotificationNow(notification);

                locationMgr.StartRangingBeacons(beaconRegion);
            };

            locationMgr.RegionLeft += (object sender, CLRegionEventArgs e) =>
            {
                UILocalNotification notification = new UILocalNotification() { AlertBody = "You left this region: " + e.Region.Identifier };
                UIApplication.SharedApplication.PresentLocalNotificationNow(notification);

                locationMgr.StopRangingBeacons(beaconRegion);

                if (e.Region.Identifier == monkeyId)
                {
                    //
                }
            };


            locationMgr.DidRangeBeacons += (object sender, CLRegionBeaconsRangedEventArgs e) =>
            {
                string message = "";
                string status = "no";

                if (e.Beacons.Length > 0)
                {
                    if (DateTime.UtcNow > Settings.LastBea.AddMinutes(10))
                    {
                        Settings.LastBea = DateTime.UtcNow;
                        UILocalNotification notification = new UILocalNotification() { AlertBody = "There is a SCR nearby!!!" };
                        UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
                    }

                    if (!inRange)
                    {

                    }
                    inRange = true;

                    CLBeacon beacon = e.Beacons[0];

                    switch (beacon.Proximity)
                    {
                        case CLProximity.Immediate:
                            message = "Birillo is here!";
                            status = "here";
                            break;
                        case CLProximity.Near:
                            message = "Sei vicino a Birillo!";
                            status = "nearby";
                            break;
                        case CLProximity.Far:
                            message = "Birillo si trova in quest'area!";
                            status = "far away";
                            break;
                        case CLProximity.Unknown:
                            //message = "I'm not sure how close you are to the monkey";
                            break;
                    }

                    if (previousProximity != beacon.Proximity)
                    {
                        //Speak(message);

                        // demo send message using multipeer connectivity
                        //if (beacon.Proximity == CLProximity.Immediate)
                        //SendMessage();
                    }
                    previousProximity = beacon.Proximity;
                }
                else
                {
                    inRange = false;
                    status = "no";
                }

                EventHandler<string> handler = BeaconRanged;
                if (handler != null)
                {
                    handler(this, status);
                }
            };
        }

        public void StartTrackingBeacons()
        {
            Debug.WriteLine("start monitoring beacons");
            locationMgr.StartMonitoring(beaconRegion);
        }

        public void PauseTrackingBeacons()
        {
            Debug.WriteLine("stop monitoring beacons");
            locationMgr.StopMonitoring(beaconRegion);
        }

        public void StartTracking()
        {
            Debug.WriteLine("start location tracking");
            locationMgr.StartUpdatingLocation();
        }

        public void PauseTracking()
        {
            Debug.WriteLine("stop location tracking");
            locationMgr.StopUpdatingLocation();
        }

        public void SingleTracking()
        {
            Debug.WriteLine("Single Tracking");
            locationMgr.RequestLocation();
        }

        void Speak(string text)
        {
            var speechSynthesizer = new AVSpeechSynthesizer();

            //          var voices = AVSpeechSynthesisVoice.GetSpeechVoices ();

            var speechUtterance = new AVSpeechUtterance(text)
            {
                //Rate = AVSpeechUtterance.MaximumSpeechRate / 16,
                Rate = 0.5f,
                Voice = AVSpeechSynthesisVoice.FromLanguage("it-IT"),
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
