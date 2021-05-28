# A Paint.NET Effect Plugin based on Arbitrary Style Transfer

This plugin makes [Neural Style Transfer](https://en.wikipedia.org/wiki/Neural_Style_Transfer) available in the free and popular Pain.NET image editing software.

## Introduction

In neural style transfer, the style of an image is extracted and transferred to another image. The technique has become popular in recent years and can yield very interesting results. Here's an example using the painting “The Starry Night” by Vincent van Gogh:

![neural style example](StyleTransfer/assets/images/Example.png "Neural style example")

Most implementations use a single style or very limited set of styles and deliver high quality results. This plugin is more of an exploratory tool for art and fun, and uses arbitrary neural style transfer as described in the paper [Exploring the structure of a real-time, arbitrary neural artistic stylization network](https://arxiv.org/abs/1705.06830) by Ghiasi, et. al.
The models where ported from pre-trained versions available in the [Magenta repository](https://github.com/tensorflow/magenta/tree/master/magenta/models/arbitrary_image_stylization). This technique allows interpolating between styles. Instead of blending the original image and the stylised result, the content image style is extracted and can be mixed with the target style at a selectable ratio.

## Installation

The released version contains an installer that copies the plugin and all its required dependencies and data to the Paint.NET-Effects folder automatically.

(If you just want to try it out, download the installer directly from [**HERE**](https://github.com/patlevin/pdn-styletransfer/releases/download/v1.1/StyleTransferEffect-Installer.exe) - requires the latest Paint.NET 4.2.16+)

### System requirements

* Windows 7 or better, **64-bit** version
* **lots** of RAM (16 GB+ recommended, but will work fine with 8GB as well)
* Paint.NET version 4.2 or newer (tested with PDN 4.2.8)
* a fast CPU (the plugin doesn't support GPUs at the moment)

## Using the plugin

After the installation, the plugin can be found in, and selected from Paint.NET's "**Effects**"-menu in the section "**Artistic**":

![plugin location](StyleTransfer/assets/images/plugin_location.png "Plugin: Selecting from menu")

### Selecting presets

![plugin presets](StyleTransfer/assets/images/plugin_presets.png "Plugin: Preset Selection")

The first tab of the plugin lets you select a preset from a selection of styles extracted from random images. To avoid copyright issues, none of the style images are contained in the plugin or its source.
After selecting a style, a sample image with the style applied is shown. This example image features a movable slider for a nice "before and after"-effect:

![plugin preset example](StyleTransfer/assets/images/plugin_preset_example.png "Plugin: Preset Example")

The "Stylize Amount"-slider lets you adjust the ratio between the selected style and the content image style. Note that this is not a simple alpha-blend, though.
Even selecting zero ("0") will change the content image significantly, as the model tries to apply the style extracted from the content to the content itself. This process isn't perfect by any means, so keep that in mind.

"Style Extraction" and "Style Transfer" models can be selected as well. The "High Quality" setting uses a larger model for extracting and transferring styles. The "Fast"-option selects a smaller, "distilled" version of the higher quality one that requires less memory and less CPU power. The results will be very different between the models and depending on the selected style and amount, you might even prefer one over the other.

### Creating custom styles

The "**Custom Style**"-tab allows you to select your own style images and apply them:

![plugin custom style](StyleTransfer/assets/images/plugin_custom_style.png "Plugin: Custom Style")

Clicking the "**Select Style Image**"-button or clicking the style image preview directly, will open a file dialog for image selection. The "**Style Image Size**"-slider determines the relative scaling factor of the style image that is used before the style is analyzed. Think of it as how much of the style image the model is allowed to "see" before the style is extracted.
The colored bar below the slider indicates a size range that will probably yield the best-looking results, with the green arrow pointing at the recommended value.
You can select values outside this range (though RAM and minimum size restrictions still apply), but the results may not be as good.

The plugin will display a warning, if you choose a combination of a big style image and large style size:

![plugin size warning](StyleTransfer/assets/images/plugin_custom_style_warning.png "Plugin: Size warning")

This happens, because the processing requires a large amount of RAM and once you use more RAM than is physically installed in your computer, your system will crawl to a halt, become unresponsive, and eventually even crash Paint.NET.
The plugin will not use your settings, if they would result in too much RAM being used to protect the system from running out of memory.

## How it works

The plugin will first determine whether it can process the content image as a whole. Since the AI models use a lot of RAM, not all image sizes can be processed while still fitting in the computer's memory. Big images will therefore be divided into tiles, which are processed separately. This means, that the style transfer doesn't apply to the image as a whole in such cases, resulting in artifacting at the tile boundaries.
In order to mitigate this effect somewhat, the algorithm creates a set of overlapping tiles that will be blended into each other. This works well in general, but results may vary depending on image size and -content.
The amount of available RAM also affects the resulting quality as larger tiles means fewer tiles, and thus less potential artifacting...

The selected style image also affects the results (duh!), but not necessarily in a way that's obvious. This is actually the beauty of the effect (at least in my eyes), as you never quite get what you'd expect. Part of this comes from the limited training data and -time on the AI models used - specialised models that have been trained on a single image style or a small set of styles, perform much better.

Another factor is the amount of information that can be captures during style extraction. The model uses a 100-element style vector and trying to capture the essence of a mega-pixel artwork in just 100 numbers is... challenging, to say the least.

This is also where the styleimage size setting comes into play. Selecting a smaller value will remove details and focus on "the big picture", while values that are too big might simply fail to capture any meaningful data during the style extraction process.

## Building from source

Building from source requires Visual Studio 2019 (available for free [here](https://visualstudio.microsoft.com/vs/)).
Presets cannot be recreated from the source code just to avoid any potential copyright issues. All dependencies are included, except for the NuGet-packages.
The NuGet-packages will be acquired automatically by Visual Studio.
The included [ONNX runtime](https://microsoft.github.io/onnxruntime/) is a custom build using the CPU-backend with multi-core optimizations enabled.

## Future improvements

I tried to pack as much into the plugin as I deemed necessary to make it useful and fun to play with. There are, however, a few things that I skipped, just to get it out the door first:

* **GPU-support**
* RAM-usage optimisations
* combining multiple styles
* user-created presets
* support for cancel while processing the effect
* advanced options (tiling overlap, blend modes)
* **improved AI models**

## GPU Support Update

The DirectML team has made great progress since I first tested it. 
I also tested the CUDA version of the ONNX Runtime, but at over 160 MB it's ridiculously huge in size and impractical for use.

As it stands now, DirectML is the way to go. The latest version fully supports the models used in this project and peformance is
where it should be:

| Device   |Style extraction|Content transform| Total    |
|:---------|---------------:|----------------:|---------:|
| CPU¹     | 225ms          | 3393ms          | 3619ms   |
| iGPU²    |  64ms          |  882ms          |  947ms   |
| dGPU³    |  31ms          |  508ms          |  540ms   |

¹[Intel® Core™ i7-7500U @ 25W](https://ark.intel.com/content/www/us/en/ark/products/95451/intel-core-i7-7500u-processor-4m-cache-up-to-3-50-ghz.html)
²[Intel® HD Graphics 620](https://ark.intel.com/content/www/us/en/ark/products/95451/intel-core-i7-7500u-processor-4m-cache-up-to-3-50-ghz.html)
³[NVIDIA GeForce GTX 950M, ASUS](https://www.techpowerup.com/gpu-specs/asus-gtx-950m-2-gb.b5469)

Numbers taken from a 10 run average after a single warmup run (see below).

Even using the rather weak integrated GPU yielded very good results: style transfer is 3.8x faster.
The equally weak discrete GPU (comparable to the more modern 150MX/250MX/350MX variants found in modern ultrabooks or AMD APUs) achieves an impressive
6.7x speed-up and is about twice as fast as the iGPU.

There is one small issue, though that I still need to investigate further. Note that the 10 run averages exclude a warm-up run.
This first run is about 50% slower due to JIT and DLL loading - that's OK and still significantly faster than using just the CPU.
The problem arises with the very first(?) run, however. The Intel® iGPU took a shocking 42 seconds to complete the first pass.
I actually had to run it 3 times, because I genuinely thought the app had crashed...
The dGPU's very first(?) run took "just" 12 seconds, which is still unacceptable compared to the CPU performance.

Subsequent runs of the benchmark app (even after OS restart) displayed the expected behaviour of the first run being about 50% slower than the average.

Unfortunately I learned that there's no way to pre-compile shaders from another executable as [per the specs](https://github.com/microsoft/DirectX-Specs/blob/master/d3d/ShaderCache.md). The very first run of the models will therefore always be significantly slower if D3D12 is used.
