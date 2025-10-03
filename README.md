# URP Frame Accumulation RendererFeature

This RendererFeature allows you to store a copy of your color buffer at any given RenderGraph injection point to be later used for any kind of frame accumulation-based fullscreen effect.
An example shader graph implementing frame accumulation motion blur is provided:

https://github.com/user-attachments/assets/7844e03a-3dfd-42b3-b695-2dda5daf821a



This Renderer Feature requires Unity 6 / RenderGraph API.
I might port it to the pre-render graph URP API, however, that is not currently a priority. 

Included is a simple fullscreen alpha blending shader graph that allows for a PS2-style accumulation blur to be mimicked.
Simply create a material, plug it into the renderer feature, and you should be good to go.

A .unitypackage file is provided for your convenience. 

This Unity Discussions post was used as a template to better understand the RG API, along with the Unity Manual/Scripting API:

https://discussions.unity.com/t/introduction-of-render-graph-in-the-universal-render-pipeline-urp/930355/3
