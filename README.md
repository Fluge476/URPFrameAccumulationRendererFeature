# URP Frame Accumulation RendererFeature

This Renderer Feature requires Unity 6 / RenderGraph API.
I might port it to the pre-render graph URP API, however, that is not currently a priority. 

Included is a simple fullscreen alpha blending shader graph that allows for a PS2-style accumulation blur to be mimicked.
Simply create a material, plug it into the renderer feature, and you should be good to go.

A .unitypackage file is provided for your convenience. 

This Unity Discussions post was used as a template to better understand the RG API, along with the Unity Manual/Scripting API:

https://discussions.unity.com/t/introduction-of-render-graph-in-the-universal-render-pipeline-urp/930355/3
