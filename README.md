# SentivePiano
A MIDI redirector for Synthesia to make some types of keyboard lights work.

## Supported Keyboards
- POP Piano 61 Key (PT-611)

## How To Use
0. Install [teVirtualMIDI](http://www.tobias-erichsen.de/software/virtualmidi.html) driver.
1. Connect your keyboard to PC and toggle power on.
2. After driver installed, open SentivePiano.
3. Open Synthesia and go to settings. Set input device as your your keyboard and try. If your keyboard is not working, reconnect it and go to 1.
4. Set output device to use `Sentive Forwarder` (alongside the MIDI Synthesizer) as "Key lights", and set correct light channel (by default ch.3)
5. Test it with a MIDI.

## Demo
This is how it should be:

[![Sentive-with-POP](https://github.com/UlyssesWu/SentivePiano/blob/master/demo/Sentive-POP.jpg)](https://soundcloud.com/le-volume-sur-t/2ftmwpkxvpzy)

## Compile
Due to license reason (no re-distribution), you should put the source code files from [teVirtualMIDI](http://www.tobias-erichsen.de/software/virtualmidi/virtualmidi-sdk.html) by yourself.

## License
**CC BY-NC-SA 4.0**

## Thanks
- Tobias Erichsen for [teVirtualMIDI](http://www.tobias-erichsen.de/software/virtualmidi.html)
- [Maxim Dobroselsky](https://github.com/melanchall) for [DryWetMIDI](https://github.com/melanchall/drywetmidi)

---
by Ulysses