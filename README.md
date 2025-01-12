# NnUtils
NnUtils should always be in your `Assets` directory.

## Installation Options

### Submodule(recommended)
Navigate to your git repo and run:
```sh
git submodule add https://www.github.com/nnra6864/NnUtils Assets/NnUtils
git submodule update --init --recursive
```

### Direct Clone
In your `Assets` directory run:
```sh
git clone --recursive https://www.github.com/nnra6864/NnUtils
```

### Selective
If you want only specific modules, you can do the following in your `Assets` directory:
```sh
git clone https://www.github.com/nnra6864/NnUtils
cd NnUtils
git submodule init Modules/JSONUtils Modules/SomeOtherModule
git submodule update
```

## Dependencies
NnUtils also has some dependencies that can't be automatically installed.

### [Newtonsoft JSON](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.2) - Required by [JSONUtils](https://www.github.com/nnra6864/JSONUtils)
Get it by using `Install package by name` in the Unity Package Manager and pasting the following `com.unity.nuget.newtonsoft-json`
