# CyberBiology [![Build status](https://ci.appveyor.com/api/projects/status/m6sueqwgqc1rtbfm?svg=true)](https://ci.appveyor.com/project/ihtfw/cyberbiology)

After article https://habr.com/post/418545/ decided to make version using WPF .Net. 
Each bot has own consciousness that stores 64 actions. Each iteration bot executes next action. On bot division some actions could be randomly changed.

Some bots are relatives, depends on consciousness. If it pretty much close to each other than bot are relatives.

## Current list of actions

- Photosynthesis  
- Accumulate minerals
- Skip some actions
- Move
- Rotate
- Look
- Eat (Organic, Relative bot or Other bot)
- Share (Only to relative)
- Give (Only to relative)
- Skip next action if surrounded
- Convert minerals to health
- Mutate (change own consciousness)
- Gene Attack (change other bot consciousness)
- Bot division 

## Screenshots:

![Alt CyberBiology screenshot 1](https://raw.githubusercontent.com/ihtfw/CyberBiology/master/images/screenshot1.png) ![Alt CyberBiology screenshot 2](https://raw.githubusercontent.com/ihtfw/CyberBiology/master/images/screenshot2.png) ![Alt CyberBiology screenshot 3](https://raw.githubusercontent.com/ihtfw/CyberBiology/master/images/screenshot3.png) 

## Other

- Deploy using [Squirrel.Windows](https://github.com/Squirrel/Squirrel.Windows)
- Drawing is done by using [WriteableBitmapEx](https://github.com/teichgraf/WriteableBitmapEx)
- Save load is done using serializing to Json ([Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)) and then packing to zip archieve ([DotNetZip](https://github.com/haf/DotNetZip.Semverd))
- [Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro) as Mvvm Framework
- [PropertyChanged.Fody](https://github.com/Fody/PropertyChanged) to reduce dumb code for INotifyPropertyChanged implementation
