<img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg">   <img src="https://forthebadge.com/images/badges/built-with-love.svg">

# pePolymorpher
A tool implementing process hollowing making your PE polymorphic

## Description
This tool uses the XOR operator as well as a RunPE (public) to make any PE polymorphic. No dependency is required.

## Utilisation
Running pePolymorpherBuilder.exe will allow you to make your PE polymorphic :<br/>
<br/><img src="https://image.prntscr.com/image/LRTHfA-uRHGdMaq3dOolVg.png">
<br/>You must enter a x64 PE which is not using .NET Framework.

## Process
We will use putty.exe as an example.
Openning "polymorphic.exe" will show that windows : <br/>
<br/><img src="https://image.prntscr.com/image/5-D8tYDHTne2d0gwjsA3-Q.png">

Every 20secs, all these steps happens:
- `calc.exe` will get closed
- Your PE will get XORed with a random key
- XORed-PE will be injected in a new fresh generated stub
- Stub will get runned
- XORed-PE will get unxored
- UnXOR-ed PE will be loaded in memory then injected to `C:\\Windows\\system32\\calc.exe` (you can change the host in `Program.cs:89`)

All of these steps are making your PE polymorphic since self md5-sum is always different.

## PoC
<img src="https://image.prntscr.com/image/_3brzZbHTHGYjt1ubtly0w.png">
