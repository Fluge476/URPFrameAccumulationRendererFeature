# URP Frame Accumulation RendererFeature

This RendererFeature allows you to store a copy of your color buffer at any given RenderGraph injection point and mix it into a persistent buffer. This allows for the creation of frame accumulation-based fullscreen effect.

An example shader graph with a simple alpha-blended frame accumulation motion blur is provided.

https://github.com/user-attachments/assets/7844e03a-3dfd-42b3-b695-2dda5daf821a



## Usage
The RendererFeature simply draws the current frame into the persistent buffer using the provided material, then writes that persistent buffer into the main color buffer. To sample the current frame, use the URP Sample Buffer Node.


## Installation
This Renderer Feature requires Unity 6+ and the RenderGraph API.
A .unitypackage file inluding the feature and example effect is provided for your convenience. 

------

This [Unity Discussions post](https://discussions.unity.com/t/introduction-of-render-graph-in-the-universal-render-pipeline-urp/930355/3) was used as a template to better understand the RenderGraph API, along with the Unity Manual/Scripting API.


*NOTE: I may port this to the pre-render graph URP API, however that is not currently a priority.*
