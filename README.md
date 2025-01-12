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
