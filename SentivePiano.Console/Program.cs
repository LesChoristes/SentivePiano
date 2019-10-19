using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Smf;
using TobiasErichsen.teVirtualMIDI;

namespace SentivePiano.Console
{
    class Program
    {
        public static int LightChannel = 2;

        static void Main(string[] args)
        {
            foreach (var outputDevice in OutputDevice.GetAll())
            {
                System.Console.WriteLine(outputDevice.Name);
            }

            //Lighten();
            Redirect();
            System.Console.ReadLine();
        }

        private static TeVirtualMIDI _port;
        private static OutputDevice _outputDevice;

        static void Redirect()
        {
            var input = "Sentive Forwarder";
            _port = new TeVirtualMIDI(input);
            Thread thread = new Thread(RedirectorRun);
            thread.Start();
            _outputDevice = OutputDevice.GetAll().First(t => t.Name.Contains("POP"));
            _outputDevice.SendEvent(new NormalSysExEvent(PDevice.BuildPianoVerifyPacket().AddTimestamp()));
            _outputDevice.SendEvent(new NormalSysExEvent(PDevice.BuildQueryPacket().AddTimestamp()));

            var inputDevice = InputDevice.GetByName(input);
            inputDevice.EventReceived += InputDeviceOnEventReceived;
            inputDevice.StartEventsListening();
            //_outputDevice.EventSent += OnEventSent;
            DevicesConnector connector = new DevicesConnector(inputDevice, _outputDevice);
            connector.Connect();

            System.Console.WriteLine("Running...");
            System.Console.ReadLine();

            inputDevice.StopEventsListening();
            connector.Disconnect();
            connector.Dispose();
            inputDevice.Dispose();
            _outputDevice.Dispose();

            thread.Abort();
            _port.shutdown();
            System.Console.WriteLine("Stopped!");
            System.Console.ReadLine();
        }

        private static void RedirectorRun()
        {
            byte[] command;

            try
            {
                while (true)
                {
                    command = _port.getCommand();
                    _port.sendCommand(command);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("thread aborting: " + ex.Message);
            }
        }

        static ConcurrentDictionary<int, PlayHand> LightMap = new ConcurrentDictionary<int, PlayHand>();

        static void Lighten()
        {
            using (var outputDevice = OutputDevice.GetAll().First(t => t.Name.Contains("POP")))
                //using (var inputDevice = InputDevice.GetAll().First(t => t.Name.Contains("POP")))
            {
                //inputDevice.EventReceived += InputDeviceOnEventReceived;
                //inputDevice.StartEventsListening();

                outputDevice.SendEvent(new NormalSysExEvent(PDevice.BuildPianoVerifyPacket().AddTimestamp()));
                //Thread.Sleep(500);
                outputDevice.SendEvent(new NormalSysExEvent(PDevice.BuildQueryPacket().AddTimestamp()));
                //outputDevice.SendEvent(new NormalSysExEvent("8080f04d4c4e454c1100f7".HexStringToBytes()));

                outputDevice.EventSent += OnEventSent;


                System.Console.WriteLine("Waiting...");
                System.Console.ReadLine();
                var midiFile = MidiFile.Read(@"千本桜.mid");
                using (var playback = midiFile.GetPlayback(outputDevice))
                {
                    //playback.Speed = 2.0;
                    playback.Play();
                }

                //inputDevice.StopEventsListening();
            }
        }

        private static void InputDeviceOnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            System.Console.WriteLine("Input: " + e.Event.ToString());
            if (e.Event is SysExEvent s)
            {
                //System.Console.WriteLine("Output: " + s.Data.PrintInHex());
                return;
            }

            byte[] bts = null;
            if (e.Event is NoteOnEvent noteOn)
            {
                if (noteOn.Channel != LightChannel)
                {
                    return;
                }

                bts = PDevice.BuildSingleNoteLightPacket(noteOn.NoteNumber);
                //System.Console.WriteLine(noteOn.NoteNumber);
                //LightMap[noteOn.NoteNumber] = PlayHand.Right;
            }
            else if (e.Event is NoteOffEvent noteOff)
            {
                if (noteOff.Channel != LightChannel)
                {
                    return;
                }

                bts = PDevice.BuildSingleNoteLightPacket(noteOff.NoteNumber, false);
                //LightMap.TryRemove(noteOff.NoteNumber, out _);
            }

            if (bts == null || _outputDevice == null)
            {
                return;
            }
            //var bts = PDevice.BuildLightsPacket(LightMap).AddTimestamp();

            _outputDevice.SendEvent(new NormalSysExEvent(bts.AddTimestamp().ToArray()));
            //System.Console.WriteLine(bts.PrintInHex());
        }

        private static void OnEventSent(object sender, MidiEventSentEventArgs e)
        {
            if (e.Event is SysExEvent s)
            {
                System.Console.WriteLine("Output: " + s.Data.PrintInHex());
                return;
            }

            byte[] bts = null;
            if (e.Event is NoteOnEvent noteOn)
            {
                bts = PDevice.BuildSingleNoteLightPacket(noteOn.NoteNumber);
                //System.Console.WriteLine(noteOn.NoteNumber);
                //LightMap[noteOn.NoteNumber] = PlayHand.Right;
            }
            else if (e.Event is NoteOffEvent noteOff)
            {
                bts = PDevice.BuildSingleNoteLightPacket(noteOff.NoteNumber, false);
                //LightMap.TryRemove(noteOff.NoteNumber, out _);
            }

            if (bts == null)
            {
                return;
            }
            //var bts = PDevice.BuildLightsPacket(LightMap).AddTimestamp();

            var midiDevice = (OutputDevice) sender;
            midiDevice.SendEvent(new NormalSysExEvent(bts.AddTimestamp().ToArray()));
            //System.Console.WriteLine(bts.PrintInHex());
        }
    }
}