﻿using System;
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
        bool inRange = false;
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

        public event EventHandler<GeographicLocation> LocationChanged;

        public IBeaconService()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            peripheralDelegate = new BTPeripheralDelegate();
            peripheralMgr = new CBPeripheralManager(peripheralDelegate, DispatchQueue.DefaultGlobalQueue);
        }

        public void StartTracking()
        {
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
                locationMgr.PausesLocationUpdatesAutomatically = false;
                locationMgr.RequestAlwaysAuthorization();
                locationMgr.AllowsBackgroundLocationUpdates = true;

                if (CLLocationManager.LocationServicesEnabled)
                {
                    //set the desired accuracy, in meters 
                    //LocMgr.DesiredAccuracy = 1; 
                    //LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => { 
                    // fire our custom Location Updated event LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1])); }; LocMgr.StartUpdatingLocation(); 
                    //}
                }

                //locationMgr.RequestWhenInUseAuthorization();

                locationMgr.RegionEntered += (object sender, CLRegionEventArgs e) =>
                {
                    Debug.WriteLine("new region");
                    if (e.Region.Identifier == monkeyId)
                    {
                        UILocalNotification notification = new UILocalNotification() { AlertBody = "There's a monkey hiding nearby!" };
                        UIApplication.SharedApplication.PresentLocalNotificationNow(notification);
                    }
                };

                locationMgr.DidRangeBeacons += (object sender, CLRegionBeaconsRangedEventArgs e) =>
                {
                    Debug.WriteLine(e.Beacons.Length);

                    if (e.Beacons.Length > 0)
                    {

                        CLBeacon beacon = e.Beacons[0];
                        string message = "";

                        switch (beacon.Proximity)
                        {
                            case CLProximity.Immediate:
                                message = "Birillooooooooo! Birillooooooo!";

                                //View.BackgroundColor = UIColor.Green;
                                break;
                            case CLProximity.Near:
                                message = "Sei vicino a Birillo!";

                                //View.BackgroundColor = UIColor.Yellow;
                                break;
                            case CLProximity.Far:
                                message = "Birillo si trova in quest'area!";

                                //View.BackgroundColor = UIColor.Blue;
                                break;
                            case CLProximity.Unknown:
                                //message = "I'm not sure how close you are to the monkey";
                                //View.BackgroundColor = UIColor.FromRGB(239,239,239);
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
                    else
                    {
                        //View.BackgroundColor = UIColor.FromRGB(239, 239, 239);
                    }
                };

                locationMgr.StartMonitoring(beaconRegion);
                locationMgr.StartRangingBeacons(beaconRegion);
            }
        }

        public void PauseTracking()
        {

        }

        public void SingleTracking()
        {

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
